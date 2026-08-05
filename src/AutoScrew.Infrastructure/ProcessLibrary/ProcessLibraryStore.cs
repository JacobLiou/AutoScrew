using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Lan;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.ProcessLibrary;

public sealed class ProcessLibraryStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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

        var connectErr = _lan.EnsureConnected();
        if (connectErr is not null && !string.IsNullOrWhiteSpace(_lan.ResolveLanRoot()))
            throw new IOException($"无法连接局域网工艺库：{connectErr}");

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

        var manifestPath = Path.Combine(productDir, "product.json");
        await using (var stream = File.Create(manifestPath))
        {
            await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation(
            "Process card saved product={ProductPn} slot={SlotId} screw={ScrewPn}",
            safePn, parsed.SlotId, parsed.ScrewPn);

        return new ProcessLibrarySlotInfo(slot.SlotId, slot.ScrewPn, slot.FileName, slot.DisplayName);
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
            var manifestPath = Path.Combine(productDir, "product.json");
            await using var stream = File.Create(manifestPath);
            await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
        }
    }

    public string ResolveSlotFilePath(string productPn, ProcessLibrarySlotInfo slot)
    {
        var safePn = SanitizeFileName(productPn);
        var productDir = Path.Combine(ResolveProcessRoot(), safePn);
        return Path.Combine(productDir, slot.FileName.Replace('/', Path.DirectorySeparatorChar));
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
                return doc;
        }

        return new ProcessProductManifestDocument { ProductPn = safePn, Slots = [] };
    }

    private ProcessLibraryProductSummary RebuildManifestFromFiles(string safePn, string dir)
    {
        var screws = Path.Combine(dir, "screws");
        var slots = new List<ProcessLibrarySlotInfo>();
        if (Directory.Exists(screws))
        {
            foreach (var file in Directory.EnumerateFiles(screws, "*.txt"))
            {
                try
                {
                    var parsed = ProcessCardTxtParser.ParseFile(file);
                    var relative = Path.GetRelativePath(dir, file).Replace('\\', '/');
                    slots.Add(new ProcessLibrarySlotInfo(
                        parsed.SlotId, parsed.ScrewPn, relative, parsed.ScrewPn));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skip unreadable process card {File}", file);
                }
            }
        }

        slots.Sort(static (a, b) => a.SlotId.CompareTo(b.SlotId));
        return new ProcessLibraryProductSummary(safePn, null, slots);
    }

    private static ProcessLibraryProductSummary ToSummary(ProcessProductManifestDocument doc) =>
        new(
            doc.ProductPn,
            doc.UpdatedUtc,
            doc.Slots
                .OrderBy(s => s.SlotId)
                .Select(s => new ProcessLibrarySlotInfo(s.SlotId, s.ScrewPn, s.FileName, s.DisplayName))
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
