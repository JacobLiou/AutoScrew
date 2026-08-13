using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Hardware;
using AutoScrew.Infrastructure.Lan;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

public sealed class ProcessLibraryStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static readonly JsonSerializerOptions SequenceJsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly LanShareAccess _lan;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ILogger<ProcessLibraryStore> _logger;

    public ProcessLibraryStore(
        LanShareAccess lan,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<ProcessLibraryStore> logger)
    {
        _lan = lan;
        _appOptions = appOptions;
        _logger = logger;
    }

    public string ResolveProcessRoot()
    {
        var lanRoot = _lan.ResolveLanRoot();
        if (!string.IsNullOrWhiteSpace(lanRoot))
        {
            var connectErr = _lan.EnsureConnected();
            if (connectErr is not null)
                _logger.LogWarning("LAN process root connect: {Error}; still using path {Root}", connectErr, lanRoot);
            return Path.Combine(lanRoot.TrimEnd('\\', '/'), "process");
        }

        var data = string.IsNullOrWhiteSpace(_appOptions.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : _appOptions.Value.DataDirectory;
        return Path.Combine(data, "process");
    }

    public void EnsureRoot()
    {
        var root = ResolveProcessRoot();
        Directory.CreateDirectory(root);
    }

    public IReadOnlyList<string> ListProductPns()
    {
        EnsureRoot();
        var root = ResolveProcessRoot();
        if (!Directory.Exists(root))
            return [];

        return Directory.EnumerateDirectories(root)
            .Select(Path.GetFileName)
            .Where(static n => !string.IsNullOrWhiteSpace(n))
            .Select(static n => n!)
            .OrderBy(static n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public ProcessLibraryProductSummary? LoadProduct(string productPn)
    {
        var safePn = SanitizeFileName(productPn);
        var dir = Path.Combine(ResolveProcessRoot(), safePn);
        var manifestPath = Path.Combine(dir, "product.json");
        if (!File.Exists(manifestPath))
        {
            if (!Directory.Exists(dir))
                return null;
            return RebuildManifestFromFiles(safePn, dir);
        }

        var doc = JsonSerializer.Deserialize<ProcessProductManifestDocument>(
            File.ReadAllText(manifestPath), JsonOptions);
        if (doc is null)
            return null;

        doc.Sequences ??= [];
        doc.Slots ??= [];
        return ToSummary(doc);
    }

    public async Task<ProcessLibrarySlotInfo> SaveProcessCardAsync(
        string productPn,
        string sourceFilePath,
        ProcessCardParseResult parsed,
        CancellationToken cancellationToken)
    {
        EnsureRoot();
        var safePn = SanitizeFileName(productPn);
        if (string.IsNullOrWhiteSpace(safePn))
            throw new ArgumentException("产品 PN 无效。", nameof(productPn));

        EnsureLanConnected();

        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        var screwsDir = Path.Combine(productDir, "screws");
        Directory.CreateDirectory(screwsDir);

        var fileName = $"screws/{parsed.SlotId:D2}.txt";
        var destPath = Path.Combine(productDir, fileName.Replace('/', Path.DirectorySeparatorChar));
        cancellationToken.ThrowIfCancellationRequested();
        File.Copy(sourceFilePath, destPath, overwrite: true);

        var doc = LoadOrCreateManifest(safePn, productDir);
        doc.ProductPn = safePn;
        doc.UpdatedUtc = DateTimeOffset.UtcNow;
        var wasUpdate = doc.Slots.Any(s => s.SlotId == parsed.SlotId);
        doc.Slots.RemoveAll(s => s.SlotId == parsed.SlotId);
        var slot = new ProcessSlotDocument
        {
            SlotId = parsed.SlotId,
            ScrewPn = parsed.ScrewPn,
            FileName = fileName,
            DisplayName = parsed.ScrewPn,
        };
        doc.Slots.Add(slot);
        doc.Slots.Sort(static (a, b) => a.SlotId.CompareTo(b.SlotId));

        await SaveManifestAsync(productDir, doc, cancellationToken).ConfigureAwait(false);

        var deviceId = ProcessParameterCode.ToDeviceParameterId(slot.SlotId);
        _logger.LogInformation(
            "Process card saved product={ProductPn} slot={SlotId} deviceId={DeviceId} screw={ScrewPn} wasUpdate={WasUpdate}",
            safePn, parsed.SlotId, deviceId, parsed.ScrewPn, wasUpdate);

        return new ProcessLibrarySlotInfo(
            slot.SlotId,
            slot.ScrewPn,
            slot.FileName,
            slot.DisplayName,
            deviceId,
            wasUpdate);
    }

    public async Task RemoveSlotAsync(string productPn, int slotId, CancellationToken cancellationToken)
    {
        var safePn = SanitizeFileName(productPn);
        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        var doc = LoadOrCreateManifest(safePn, productDir);
        var existing = doc.Slots.FirstOrDefault(s => s.SlotId == slotId);
        if (existing is not null)
        {
            var path = Path.Combine(productDir, existing.FileName.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(path))
                File.Delete(path);
            doc.Slots.RemoveAll(s => s.SlotId == slotId);
            doc.UpdatedUtc = DateTimeOffset.UtcNow;
            await SaveManifestAsync(productDir, doc, cancellationToken).ConfigureAwait(false);
        }
    }

    public string ResolveSlotFilePath(string productPn, ProcessLibrarySlotInfo slot)
    {
        var safePn = SanitizeFileName(productPn);
        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        return Path.Combine(productDir, slot.FileName.Replace('/', Path.DirectorySeparatorChar));
    }

    public async Task<ProcessLibrarySequenceInfo> SaveSequenceAsync(
        string productPn,
        TighteningSequencePackage package,
        CancellationToken cancellationToken)
    {
        EnsureRoot();
        var safePn = SanitizeFileName(productPn);
        if (string.IsNullOrWhiteSpace(safePn))
            throw new ArgumentException("产品 PN 无效。", nameof(productPn));

        EnsureLanConnected();

        ArgumentNullException.ThrowIfNull(package);
        package.ApplyCoreToRaw();

        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        var seqDir = Path.Combine(productDir, "sequences");
        Directory.CreateDirectory(seqDir);

        var fileName = $"sequences/{package.SequenceId:D2}.json";
        var destPath = Path.Combine(productDir, fileName.Replace('/', Path.DirectorySeparatorChar));
        var docFile = ControllerSequencePresetDocument.FromPackage(package);
        cancellationToken.ThrowIfCancellationRequested();
        await using (var stream = File.Create(destPath))
        {
            await JsonSerializer.SerializeAsync(stream, docFile, SequenceJsonOptions, cancellationToken)
                .ConfigureAwait(false);
        }

        var displayName = string.IsNullOrWhiteSpace(package.Core.Name)
            ? package.SequenceId.ToString()
            : package.Core.Name.Trim();

        var doc = LoadOrCreateManifest(safePn, productDir);
        doc.ProductPn = safePn;
        doc.UpdatedUtc = DateTimeOffset.UtcNow;
        doc.Sequences.RemoveAll(s => s.SequenceId == package.SequenceId);
        var entry = new ProcessSequenceDocument
        {
            SequenceId = package.SequenceId,
            FileName = fileName,
            DisplayName = displayName,
        };
        doc.Sequences.Add(entry);
        doc.Sequences.Sort(static (a, b) => a.SequenceId.CompareTo(b.SequenceId));

        await SaveManifestAsync(productDir, doc, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Process sequence saved product={ProductPn} sequenceId={SequenceId}",
            safePn, package.SequenceId);

        return new ProcessLibrarySequenceInfo(entry.SequenceId, entry.FileName, entry.DisplayName);
    }

    public async Task RemoveSequenceAsync(string productPn, int sequenceId, CancellationToken cancellationToken)
    {
        var safePn = SanitizeFileName(productPn);
        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        var doc = LoadOrCreateManifest(safePn, productDir);
        var existing = doc.Sequences.FirstOrDefault(s => s.SequenceId == sequenceId);
        if (existing is not null)
        {
            var path = Path.Combine(productDir, existing.FileName.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(path))
                File.Delete(path);
            doc.Sequences.RemoveAll(s => s.SequenceId == sequenceId);
            doc.UpdatedUtc = DateTimeOffset.UtcNow;
            await SaveManifestAsync(productDir, doc, cancellationToken).ConfigureAwait(false);
        }
    }

    public string ResolveSequenceFilePath(string productPn, ProcessLibrarySequenceInfo sequence)
    {
        var safePn = SanitizeFileName(productPn);
        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        return Path.Combine(productDir, sequence.FileName.Replace('/', Path.DirectorySeparatorChar));
    }

    public TighteningSequencePackage LoadSequencePackage(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var doc = JsonSerializer.Deserialize<ControllerSequencePresetDocument>(json, SequenceJsonOptions)
            ?? throw new InvalidDataException($"Sequence file {filePath} is empty.");
        return doc.ToPackage();
    }

    private void EnsureLanConnected()
    {
        var connectErr = _lan.EnsureConnected();
        if (connectErr is not null && !string.IsNullOrWhiteSpace(_lan.ResolveLanRoot()))
            throw new IOException($"无法连接局域网工艺库：{connectErr}");
    }

    private async Task SaveManifestAsync(
        string productDir,
        ProcessProductManifestDocument doc,
        CancellationToken cancellationToken)
    {
        var manifestPath = Path.Combine(productDir, "product.json");
        await using var stream = File.Create(manifestPath);
        await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private ProcessProductManifestDocument LoadOrCreateManifest(string safePn, string productDir)
    {
        Directory.CreateDirectory(productDir);
        var manifestPath = Path.Combine(productDir, "product.json");
        if (File.Exists(manifestPath))
        {
            var doc = JsonSerializer.Deserialize<ProcessProductManifestDocument>(
                File.ReadAllText(manifestPath), JsonOptions);
            if (doc is not null)
            {
                doc.Slots ??= [];
                doc.Sequences ??= [];
                return doc;
            }
        }

        return new ProcessProductManifestDocument { ProductPn = safePn, Slots = [], Sequences = [] };
    }

    private ProcessLibraryProductSummary RebuildManifestFromFiles(string safePn, string dir)
    {
        var slots = new List<ProcessLibrarySlotInfo>();
        var screws = Path.Combine(dir, "screws");
        if (Directory.Exists(screws))
        {
            foreach (var file in Directory.EnumerateFiles(screws, "*.txt"))
            {
                try
                {
                    var parsed = ProcessCardTxtParser.ParseFile(file);
                    var relative = Path.GetRelativePath(dir, file).Replace('\\', '/');
                    slots.Add(new ProcessLibrarySlotInfo(
                        parsed.SlotId,
                        parsed.ScrewPn,
                        relative,
                        parsed.ScrewPn,
                        ProcessParameterCode.ToDeviceParameterId(parsed.SlotId)));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skip unreadable process card {File}", file);
                }
            }
        }

        var sequences = new List<ProcessLibrarySequenceInfo>();
        var seqDir = Path.Combine(dir, "sequences");
        if (Directory.Exists(seqDir))
        {
            foreach (var file in Directory.EnumerateFiles(seqDir, "*.json"))
            {
                try
                {
                    var pkg = LoadSequencePackage(file);
                    var relative = Path.GetRelativePath(dir, file).Replace('\\', '/');
                    var name = string.IsNullOrWhiteSpace(pkg.Core.Name)
                        ? pkg.SequenceId.ToString()
                        : pkg.Core.Name.Trim();
                    sequences.Add(new ProcessLibrarySequenceInfo(pkg.SequenceId, relative, name));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skip unreadable sequence {File}", file);
                }
            }
        }

        slots.Sort(static (a, b) => a.SlotId.CompareTo(b.SlotId));
        sequences.Sort(static (a, b) => a.SequenceId.CompareTo(b.SequenceId));
        return new ProcessLibraryProductSummary(safePn, null, slots, sequences);
    }

    private static ProcessLibraryProductSummary ToSummary(ProcessProductManifestDocument doc) =>
        new(
            doc.ProductPn,
            doc.UpdatedUtc,
            doc.Slots
                .OrderBy(s => s.SlotId)
                .Select(s => new ProcessLibrarySlotInfo(
                    s.SlotId,
                    s.ScrewPn,
                    s.FileName,
                    s.DisplayName,
                    ProcessParameterCode.ToDeviceParameterId(s.SlotId)))
                .ToList(),
            (doc.Sequences ?? [])
                .OrderBy(s => s.SequenceId)
                .Select(s => new ProcessLibrarySequenceInfo(s.SequenceId, s.FileName, s.DisplayName))
                .ToList());

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;
        var trimmed = name.Trim();
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", trimmed.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    }
}
