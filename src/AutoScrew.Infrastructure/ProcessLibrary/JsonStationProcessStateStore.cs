using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.ProcessLibrary;

public sealed class JsonStationProcessStateStore : IStationProcessStateStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _path;
    private readonly object _gate = new();

    public JsonStationProcessStateStore(IOptions<AutoScrewAppOptions> options)
    {
        var root = string.IsNullOrWhiteSpace(options.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : options.Value.DataDirectory;
        Directory.CreateDirectory(root);
        _path = Path.Combine(root, "station-process-state.json");
    }

    public StationProcessState? Load()
    {
        lock (_gate)
        {
            if (!File.Exists(_path))
                return null;

            try
            {
                var json = File.ReadAllText(_path);
                var doc = JsonSerializer.Deserialize<StationProcessStateDocument>(json, JsonOptions);
                if (doc is null || string.IsNullOrWhiteSpace(doc.ProductPn))
                    return null;

                return new StationProcessState(
                    doc.ProductPn.Trim(),
                    doc.UpdatedUtc,
                    doc.DeployedUtc ?? DateTimeOffset.UtcNow,
                    doc.ActiveSequenceId is > 0 ? doc.ActiveSequenceId : null);
            }
            catch
            {
                return null;
            }
        }
    }

    public void Save(StationProcessState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (string.IsNullOrWhiteSpace(state.ProductPn))
            throw new ArgumentException("Product PN is required.", nameof(state));

        lock (_gate)
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var doc = new StationProcessStateDocument
            {
                ProductPn = state.ProductPn.Trim(),
                UpdatedUtc = state.UpdatedUtc,
                DeployedUtc = state.DeployedUtc,
                ActiveSequenceId = state.ActiveSequenceId is > 0 ? state.ActiveSequenceId : null,
            };
            var json = JsonSerializer.Serialize(doc, JsonOptions);
            File.WriteAllText(_path, json);
        }
    }

    private sealed class StationProcessStateDocument
    {
        public string ProductPn { get; set; } = string.Empty;
        public DateTimeOffset? UpdatedUtc { get; set; }
        public DateTimeOffset? DeployedUtc { get; set; }
        public int? ActiveSequenceId { get; set; }
    }
}
