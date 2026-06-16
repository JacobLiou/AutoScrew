using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class LocalJsonControllerSequencePresetStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _directory;

    public LocalJsonControllerSequencePresetStore(IOptions<AutoScrewAppOptions> options)
    {
        var root = string.IsNullOrWhiteSpace(options.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : options.Value.DataDirectory;
        _directory = Path.Combine(root, "controller-sequences");
        Directory.CreateDirectory(_directory);
    }

    public Task<IReadOnlyList<ControllerSequencePresetDocument>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var list = new List<ControllerSequencePresetDocument>();
        foreach (var file in Directory.EnumerateFiles(_directory, "*.json"))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var doc = LoadFile(file);
                if (doc is not null)
                    list.Add(doc);
            }
            catch (IOException)
            {
            }
        }

        list.Sort(static (a, b) => a.SequenceId.CompareTo(b.SequenceId));
        return Task.FromResult<IReadOnlyList<ControllerSequencePresetDocument>>(list);
    }

    public async Task<TighteningSequencePackage> LoadAsync(int sequenceId, CancellationToken cancellationToken)
    {
        var path = GetPath(sequenceId);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Local sequence preset {sequenceId} not found.", path);

        cancellationToken.ThrowIfCancellationRequested();
        await using var stream = File.OpenRead(path);
        var doc = await JsonSerializer.DeserializeAsync<ControllerSequencePresetDocument>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidDataException($"Sequence preset file {path} is empty.");
        return doc.ToPackage();
    }

    public async Task SaveAsync(TighteningSequencePackage package, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(package);
        package.ApplyCoreToRaw();
        var path = GetPath(package.SequenceId);
        var doc = ControllerSequencePresetDocument.FromPackage(package);
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteAsync(int sequenceId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = GetPath(sequenceId);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    public async Task<TighteningSequencePackage> ImportFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var stream = File.OpenRead(filePath);
        var doc = await JsonSerializer.DeserializeAsync<ControllerSequencePresetDocument>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidDataException($"Sequence preset file {filePath} is empty.");
        return doc.ToPackage();
    }

    public async Task ExportToFileAsync(TighteningSequencePackage package, string filePath, CancellationToken cancellationToken)
    {
        package.ApplyCoreToRaw();
        var doc = ControllerSequencePresetDocument.FromPackage(package);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private string GetPath(int sequenceId) => Path.Combine(_directory, $"{sequenceId}.json");

    private static ControllerSequencePresetDocument? LoadFile(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ControllerSequencePresetDocument>(json, JsonOptions);
    }
}

public sealed class ControllerSequencePresetDocument
{
    public int SequenceId { get; set; } = 1;
    public int[] MainRawBlock { get; set; } = TighteningSequencePackage.CreateMainRawBlock();
    public int[] NavigatorRawBlock { get; set; } = TighteningSequencePackage.CreateNavigatorRawBlock();
    public int[] NavigatorImageRawBlock { get; set; } = TighteningSequencePackage.CreateNavigatorImageRawBlock();
    public int[] PositioningArmRawBlock { get; set; } = TighteningSequencePackage.CreatePositioningArmRawBlock();
    public TighteningSequenceCore? Core { get; set; }
    public NavigatorCoordinateCore? NavigatorCoordinates { get; set; }
    public NavigatorImageCodeCore? NavigatorImageCodes { get; set; }
    public PositioningArmCoordinateCore? PositioningArmCoordinates { get; set; }

    public TighteningSequencePackage ToPackage()
    {
        var pkg = new TighteningSequencePackage
        {
            SequenceId = SequenceId,
            MainRawBlock = Normalize(MainRawBlock, TighteningSequenceTemplate.SequenceBlockWordCount),
            NavigatorRawBlock = Normalize(NavigatorRawBlock, TighteningSequencePackage.CreateNavigatorRawBlock().Length),
            NavigatorImageRawBlock = Normalize(NavigatorImageRawBlock, TighteningSequencePackage.CreateNavigatorImageRawBlock().Length),
            PositioningArmRawBlock = Normalize(PositioningArmRawBlock, TighteningSequencePackage.CreatePositioningArmRawBlock().Length),
            Core = Core ?? new TighteningSequenceCore(),
            NavigatorCoordinates = NavigatorCoordinates ?? new NavigatorCoordinateCore(),
            NavigatorImageCodes = NavigatorImageCodes ?? new NavigatorImageCodeCore(),
            PositioningArmCoordinates = PositioningArmCoordinates ?? new PositioningArmCoordinateCore(),
        };
        if (Core is null)
            pkg.ExtractCoreFromRaw();
        else
            pkg.ApplyCoreToRaw();
        return pkg;
    }

    public static ControllerSequencePresetDocument FromPackage(TighteningSequencePackage package) => new()
    {
        SequenceId = package.SequenceId,
        MainRawBlock = package.MainRawBlock,
        NavigatorRawBlock = package.NavigatorRawBlock,
        NavigatorImageRawBlock = package.NavigatorImageRawBlock,
        PositioningArmRawBlock = package.PositioningArmRawBlock,
        Core = package.Core,
        NavigatorCoordinates = package.NavigatorCoordinates,
        NavigatorImageCodes = package.NavigatorImageCodes,
        PositioningArmCoordinates = package.PositioningArmCoordinates,
    };

    private static int[] Normalize(int[]? raw, int expected)
    {
        if (raw is null || raw.Length != expected)
            return new int[expected];
        return raw;
    }
}
