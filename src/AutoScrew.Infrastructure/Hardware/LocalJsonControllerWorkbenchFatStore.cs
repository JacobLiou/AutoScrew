using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class LocalJsonControllerWorkbenchFatStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _filePath;

    public LocalJsonControllerWorkbenchFatStore(IOptions<AutoScrewAppOptions> options)
    {
        var root = string.IsNullOrWhiteSpace(options.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : options.Value.DataDirectory;
        Directory.CreateDirectory(root);
        _filePath = Path.Combine(root, "controller-workbench-fat.json");
    }

    public async Task<ControllerWorkbenchFatDocument> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
            return new ControllerWorkbenchFatDocument();

        cancellationToken.ThrowIfCancellationRequested();
        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<ControllerWorkbenchFatDocument>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false) ?? new ControllerWorkbenchFatDocument();
    }

    public async Task SaveAsync(ControllerWorkbenchFatDocument document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, document, JsonOptions, cancellationToken).ConfigureAwait(false);
    }
}
