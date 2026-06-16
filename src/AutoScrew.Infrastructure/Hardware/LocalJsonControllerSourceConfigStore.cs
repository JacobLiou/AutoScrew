using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class LocalJsonControllerSourceConfigStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _filePath;

    public LocalJsonControllerSourceConfigStore(IOptions<AutoScrewAppOptions> options)
    {
        var root = string.IsNullOrWhiteSpace(options.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : options.Value.DataDirectory;
        Directory.CreateDirectory(root);
        _filePath = Path.Combine(root, "controller-source.json");
    }

    public async Task<ControllerSourceConfigDocument> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
            return new ControllerSourceConfigDocument();

        cancellationToken.ThrowIfCancellationRequested();
        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<ControllerSourceConfigDocument>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false) ?? new ControllerSourceConfigDocument();
    }

    public async Task SaveAsync(ControllerSourceConfigDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, document, JsonOptions, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class ControllerSourceConfigDocument
{
    public ProductionTighteningMode ProductionControlMode { get; set; } = ProductionTighteningMode.HostGuided;

    public TighteningSourceModeCore Mode { get; set; } = new();

    public TighteningSourceContentCore Content { get; set; } = new();
}
