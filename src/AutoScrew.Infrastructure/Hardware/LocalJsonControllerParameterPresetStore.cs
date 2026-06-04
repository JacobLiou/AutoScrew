using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class LocalJsonControllerParameterPresetStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _directory;

    public LocalJsonControllerParameterPresetStore(IOptions<AutoScrewAppOptions> options)
    {
        var root = string.IsNullOrWhiteSpace(options.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : options.Value.DataDirectory;
        _directory = Path.Combine(root, "controller-parameters");
        Directory.CreateDirectory(_directory);
    }

    public Task<IReadOnlyList<ControllerParameterPresetDocument>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var list = new List<ControllerParameterPresetDocument>();
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
                // Skip unreadable files.
            }
        }

        list.Sort(static (a, b) => a.ParameterId.CompareTo(b.ParameterId));
        return Task.FromResult<IReadOnlyList<ControllerParameterPresetDocument>>(list);
    }

    public async Task<TighteningParameterTemplate> LoadAsync(int parameterId, CancellationToken cancellationToken)
    {
        var path = GetPath(parameterId);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Local preset {parameterId} not found.", path);

        cancellationToken.ThrowIfCancellationRequested();
        await using var stream = File.OpenRead(path);
        var doc = await JsonSerializer.DeserializeAsync<ControllerParameterPresetDocument>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidDataException($"Preset file {path} is empty.");

        return doc.ToTemplate();
    }

    public async Task SaveAsync(TighteningParameterTemplate template, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(template);
        template.ApplyCoreToRaw();
        var path = GetPath(template.ParameterId);
        var doc = ControllerParameterPresetDocument.FromTemplate(template);
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteAsync(int parameterId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = GetPath(parameterId);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    public async Task<TighteningParameterTemplate> ImportFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var stream = File.OpenRead(filePath);
        var doc = await JsonSerializer.DeserializeAsync<ControllerParameterPresetDocument>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidDataException($"Preset file {filePath} is empty.");
        return doc.ToTemplate();
    }

    public async Task ExportToFileAsync(TighteningParameterTemplate template, string filePath, CancellationToken cancellationToken)
    {
        template.ApplyCoreToRaw();
        var doc = ControllerParameterPresetDocument.FromTemplate(template);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, doc, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private string GetPath(int parameterId) => Path.Combine(_directory, $"{parameterId}.json");

    private static ControllerParameterPresetDocument? LoadFile(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ControllerParameterPresetDocument>(json, JsonOptions);
    }
}

public sealed class ControllerParameterPresetDocument
{
    public int ParameterId { get; set; } = 1;
    public int ToolIndex { get; set; }
    public int[] RawBlock { get; set; } = TighteningParameterTemplate.CreateEmptyRawBlock();
    public TighteningParameterCore? Core { get; set; }

    public TighteningParameterTemplate ToTemplate()
    {
        if (RawBlock.Length != ModbusRegisterMap.ParameterBlockWordCount)
            RawBlock = TighteningParameterTemplate.CreateEmptyRawBlock();

        var template = new TighteningParameterTemplate
        {
            ParameterId = ParameterId,
            ToolIndex = ToolIndex,
            RawBlock = RawBlock,
            Core = Core ?? new TighteningParameterCore(),
        };
        if (Core is null)
            template.SyncCoreFromRaw();
        else
            template.ApplyCoreToRaw();
        return template;
    }

    public static ControllerParameterPresetDocument FromTemplate(TighteningParameterTemplate template) => new()
    {
        ParameterId = template.ParameterId,
        ToolIndex = template.ToolIndex,
        RawBlock = template.RawBlock,
        Core = template.Core,
    };
}
