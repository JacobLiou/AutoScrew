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
        await using var stream = File.OpenRead(path);
        var config = await JsonSerializer.DeserializeAsync<StationDeviceConfiguration>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? CreateSeedConfiguration(stationId);

        Normalize(config, stationId);
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
        var devices = StationDeviceConfiguration.CreateDefaultDevices();
        var slot0 = devices[0];
        slot0.Enabled = legacy.Enabled;
        slot0.Host = legacy.Host;
        slot0.Port = legacy.Port;
        slot0.ToolIndex = legacy.ToolIndex;
        slot0.TriggerMode = legacy.TriggerMode;
        slot0.AutoLockOnInit = legacy.AutoLockOnInit;
        slot0.SendUnlockAfterCycle = legacy.SendUnlockAfterCycle;
        slot0.UseLegacyFinishRegister = legacy.UseLegacyFinishRegister;
        slot0.CommandTimeoutMs = legacy.CommandTimeoutMs;
        slot0.DisplayName = "Device 1";

        return new StationDeviceConfiguration
        {
            StationId = stationId,
            ActiveDeviceSlot = 0,
            Devices = devices,
        };
    }

    private static void Normalize(StationDeviceConfiguration config, string stationId)
    {
        config.StationId = stationId;
        if (config.Devices.Count != StationDeviceEndpoint.MaxSlots)
        {
            var defaults = StationDeviceConfiguration.CreateDefaultDevices();
            for (var i = 0; i < StationDeviceEndpoint.MaxSlots; i++)
            {
                if (i < config.Devices.Count)
                    config.Devices[i].SlotIndex = i;
                else
                    config.Devices.Add(defaults[i]);
            }

            if (config.Devices.Count > StationDeviceEndpoint.MaxSlots)
                config.Devices = config.Devices.Take(StationDeviceEndpoint.MaxSlots).ToList();
        }

        for (var i = 0; i < config.Devices.Count; i++)
            config.Devices[i].SlotIndex = i;

        if (config.ActiveDeviceSlot < 0 || config.ActiveDeviceSlot >= StationDeviceEndpoint.MaxSlots)
            config.ActiveDeviceSlot = 0;
    }
}
