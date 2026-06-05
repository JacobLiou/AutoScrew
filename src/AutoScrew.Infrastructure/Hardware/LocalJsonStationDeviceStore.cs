using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class LocalJsonStationDeviceStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _dataRoot;
    private readonly IConfiguration _configuration;

    public LocalJsonStationDeviceStore(IOptions<AutoScrewAppOptions> appOptions, IConfiguration configuration)
    {
        _configuration = configuration;
        var root = string.IsNullOrWhiteSpace(appOptions.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : appOptions.Value.DataDirectory;
        _dataRoot = root;
        Directory.CreateDirectory(_dataRoot);
    }

    public async Task<StationDeviceConfiguration> LoadAsync(string stationId, CancellationToken cancellationToken)
    {
        var path = GetPath(stationId);
        if (!File.Exists(path))
            return CreateSeedConfiguration(stationId);

        cancellationToken.ThrowIfCancellationRequested();
        var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        using var document = JsonDocument.Parse(json);

        var migratedFromLegacy = document.RootElement.TryGetProperty("Devices", out _);
        var config = migratedFromLegacy
            ? TryParseLegacy(document, stationId) ?? CreateSeedConfiguration(stationId)
            : JsonSerializer.Deserialize<StationDeviceConfiguration>(json, JsonOptions)
              ?? CreateSeedConfiguration(stationId);

        Normalize(config, stationId);

        if (migratedFromLegacy)
            await SaveAsync(config, cancellationToken).ConfigureAwait(false);

        return config;
    }

    public async Task SaveAsync(StationDeviceConfiguration configuration, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        Normalize(configuration, configuration.StationId);
        var dir = Path.GetDirectoryName(GetPath(configuration.StationId))!;
        Directory.CreateDirectory(dir);
        await using var stream = File.Create(GetPath(configuration.StationId));
        await JsonSerializer.SerializeAsync(stream, configuration, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private string GetPath(string stationId) =>
        Path.Combine(_dataRoot, "stations", stationId, "devices.json");

    private StationDeviceConfiguration CreateSeedConfiguration(string stationId)
    {
        var legacy = _configuration.GetSection(IemdSdOptions.SectionName).Get<IemdSdOptions>() ?? new IemdSdOptions();
        var device = StationDeviceConfiguration.CreateDefaultDevice();
        device.Enabled = legacy.Enabled;
        device.Host = legacy.Host;
        device.Port = legacy.Port;
        device.ToolIndex = legacy.ToolIndex;
        device.TriggerMode = legacy.TriggerMode;
        device.AutoLockOnInit = legacy.AutoLockOnInit;
        device.SendUnlockAfterCycle = legacy.SendUnlockAfterCycle;
        device.UseLegacyFinishRegister = legacy.UseLegacyFinishRegister;
        device.CommandTimeoutMs = legacy.CommandTimeoutMs;

        return new StationDeviceConfiguration
        {
            StationId = stationId,
            Device = device,
        };
    }

    private static StationDeviceConfiguration? TryParseLegacy(JsonDocument document, string stationId)
    {
        var root = document.RootElement;
        if (!root.TryGetProperty("Devices", out var devicesElement) || devicesElement.ValueKind != JsonValueKind.Array)
            return null;

        var activeSlot = 0;
        if (root.TryGetProperty("ActiveDeviceSlot", out var activeSlotElement) && activeSlotElement.TryGetInt32(out var parsed))
            activeSlot = parsed;

        var devices = new List<StationDeviceEndpoint>();
        foreach (var item in devicesElement.EnumerateArray())
        {
            var endpoint = item.Deserialize<StationDeviceEndpoint>(JsonOptions);
            if (endpoint is not null)
                devices.Add(endpoint);
        }

        if (devices.Count == 0)
            return null;

        var selected = activeSlot >= 0 && activeSlot < devices.Count
            ? devices[activeSlot]
            : devices.FirstOrDefault(d => d.Enabled) ?? devices[0];

        return new StationDeviceConfiguration
        {
            StationId = stationId,
            Device = selected,
        };
    }

    private static void Normalize(StationDeviceConfiguration config, string stationId)
    {
        config.StationId = stationId;
        config.Device ??= StationDeviceConfiguration.CreateDefaultDevice();
    }
}
