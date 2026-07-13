using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class StationDeviceManager : IStationDeviceService, IAsyncDisposable
{
    private readonly LocalJsonStationDeviceStore _store;
    private readonly IemdSdClientFactory _clientFactory;
    private readonly ILogger<StationDeviceManager> _logger;
    private readonly bool _useSimulatedHardware;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private StationDeviceConfiguration? _cachedConfig;
    private IIemdSdClient? _client;

    public StationDeviceManager(
        LocalJsonStationDeviceStore store,
        IemdSdClientFactory clientFactory,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<StationDeviceManager> logger)
    {
        _store = store;
        _clientFactory = clientFactory;
        _logger = logger;
        StationId = appOptions.Value.StationId;
        _useSimulatedHardware = appOptions.Value.UseSimulatedHardware;
    }

    public event Action? DeviceConnectionChanged;

    public string StationId { get; }

    public bool IsSimulatedHardware => _useSimulatedHardware;

    public bool IsRuntimeDeviceAvailable
    {
        get
        {
            if (_useSimulatedHardware)
                return false;

            var device = _cachedConfig?.GetDevice();
            return device is { Enabled: true };
        }
    }

    public async Task<StationDeviceConfiguration> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _cachedConfig ??= await _store.LoadAsync(StationId, cancellationToken).ConfigureAwait(false);
            return CloneConfiguration(_cachedConfig);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(StationDeviceConfiguration configuration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.StationId = StationId;
        await _store.SaveAsync(configuration, cancellationToken).ConfigureAwait(false);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _cachedConfig = configuration;
        }
        finally
        {
            _gate.Release();
            NotifyDeviceConnectionChanged();
        }
    }

    public async Task<TestConnectionResult> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_useSimulatedHardware)
            return new TestConnectionResult(false, "Simulation mode: set AutoScrew:UseSimulatedHardware=false to test devices.");

        var config = await LoadAsync(cancellationToken).ConfigureAwait(false);
        var endpoint = config.Device;
        if (!endpoint.Enabled)
            return new TestConnectionResult(false, "Device connection is disabled.");

        await using var client = _clientFactory.Create(endpoint);
        try
        {
            await client.ConnectAsync(cancellationToken).ConfigureAwait(false);
            await client.InitializeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            await IemdSdProductionSetup.EnsureManualSourceAsync(client, _logger, cancellationToken).ConfigureAwait(false);
            return new TestConnectionResult(true, $"Connected to {endpoint.DisplayName} ({endpoint.DescribeConnection()}).");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Test connection failed for station device");
            return new TestConnectionResult(false, ex.Message);
        }
    }

    public async Task<TestConnectionResult> ApplyDeviceAsync(CancellationToken cancellationToken = default)
    {
        if (_useSimulatedHardware)
            return new TestConnectionResult(false, "Simulation mode: set AutoScrew:UseSimulatedHardware=false to apply device.");

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await DisposeClientCoreAsync().ConfigureAwait(false);
            _cachedConfig ??= await _store.LoadAsync(StationId, cancellationToken).ConfigureAwait(false);

            var device = _cachedConfig.GetDevice();
            if (device is null || !device.Enabled)
                return new TestConnectionResult(false, "No enabled device configured.");

            _client = _clientFactory.Create(device);
            await _client.ConnectAsync(cancellationToken).ConfigureAwait(false);
            await _client.InitializeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            await IemdSdProductionSetup.EnsureManualSourceAsync(_client, _logger, cancellationToken).ConfigureAwait(false);
            return new TestConnectionResult(true, $"Applied {device.DisplayName} ({device.DescribeConnection()}).");
        }
        catch (Exception ex)
        {
            await DisposeClientCoreAsync().ConfigureAwait(false);
            _logger.LogWarning(ex, "Apply device failed");
            return new TestConnectionResult(false, ex.Message);
        }
        finally
        {
            _gate.Release();
            NotifyDeviceConnectionChanged();
        }
    }

    public DeviceSummary? GetDeviceSummary()
    {
        var config = _cachedConfig;
        if (config is null)
            return null;

        var device = config.GetDevice();
        if (device is null)
            return null;

        return new DeviceSummary(
            config.StationId,
            device.DisplayName,
            device.DescribeConnection(),
            device.Enabled);
    }

    public IIemdSdClient? GetClient()
    {
        if (_useSimulatedHardware)
            return null;

        return _client;
    }

    public async Task EnsureClientAsync(CancellationToken cancellationToken = default)
    {
        if (_useSimulatedHardware)
            return;

        if (_client is not null)
            return;

        await ApplyDeviceAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisposeClientCoreAsync().ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }

    private async Task DisposeClientCoreAsync()
    {
        if (_client is null)
            return;

        await _client.DisposeAsync().ConfigureAwait(false);
        _client = null;
        NotifyDeviceConnectionChanged();
    }

    private void NotifyDeviceConnectionChanged() => DeviceConnectionChanged?.Invoke();

    private static StationDeviceConfiguration CloneConfiguration(StationDeviceConfiguration source)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(source);
        return System.Text.Json.JsonSerializer.Deserialize<StationDeviceConfiguration>(json)
               ?? new StationDeviceConfiguration { StationId = source.StationId };
    }
}
