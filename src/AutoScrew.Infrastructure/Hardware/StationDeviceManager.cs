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
    private IIemdSdClient? _activeClient;
    private int _activeClientSlot = -1;

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

    public string StationId { get; }

    public bool IsSimulatedHardware => _useSimulatedHardware;

    public bool IsRuntimeDeviceAvailable
    {
        get
        {
            if (_useSimulatedHardware)
                return false;

            var active = _cachedConfig?.GetActiveDevice();
            return active is { Enabled: true };
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
        }
    }

    public async Task<TestConnectionResult> TestConnectionAsync(int slotIndex, CancellationToken cancellationToken = default)
    {
        if (_useSimulatedHardware)
            return new TestConnectionResult(false, "Simulation mode: set AutoScrew:UseSimulatedHardware=false to test devices.");

        var config = await LoadAsync(cancellationToken).ConfigureAwait(false);
        if (slotIndex < 0 || slotIndex >= config.Devices.Count)
            return new TestConnectionResult(false, "Invalid device slot.");

        var endpoint = config.Devices[slotIndex];
        if (!endpoint.Enabled)
            return new TestConnectionResult(false, "Device slot is disabled.");

        await using var client = _clientFactory.Create(endpoint);
        try
        {
            await client.ConnectAsync(cancellationToken).ConfigureAwait(false);
            await client.InitializeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return new TestConnectionResult(true, $"Connected to {endpoint.DisplayName} ({endpoint.DescribeConnection()}).");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Test connection failed for slot {Slot}", slotIndex);
            return new TestConnectionResult(false, ex.Message);
        }
    }

    public async Task<TestConnectionResult> ApplyActiveDeviceAsync(CancellationToken cancellationToken = default)
    {
        if (_useSimulatedHardware)
            return new TestConnectionResult(false, "Simulation mode: set AutoScrew:UseSimulatedHardware=false to apply device.");

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await DisposeActiveClientCoreAsync().ConfigureAwait(false);
            _cachedConfig ??= await _store.LoadAsync(StationId, cancellationToken).ConfigureAwait(false);

            var active = _cachedConfig.GetActiveDevice();
            if (active is null || !active.Enabled)
                return new TestConnectionResult(false, "No enabled active device configured.");

            _activeClient = _clientFactory.Create(active);
            _activeClientSlot = active.SlotIndex;
            await _activeClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
            await _activeClient.InitializeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return new TestConnectionResult(true, $"Applied {active.DisplayName} ({active.DescribeConnection()}).");
        }
        catch (Exception ex)
        {
            await DisposeActiveClientCoreAsync().ConfigureAwait(false);
            _logger.LogWarning(ex, "Apply active device failed");
            return new TestConnectionResult(false, ex.Message);
        }
        finally
        {
            _gate.Release();
        }
    }

    public ActiveDeviceSummary? GetActiveDeviceSummary()
    {
        var config = _cachedConfig;
        if (config is null)
            return null;

        var active = config.GetActiveDevice();
        if (active is null)
            return null;

        return new ActiveDeviceSummary(
            config.StationId,
            active.SlotIndex,
            active.DisplayName,
            active.DescribeConnection(),
            active.Enabled);
    }

    public IIemdSdClient? GetActiveClient()
    {
        if (_useSimulatedHardware)
            return null;

        return _activeClient;
    }

    public async Task EnsureActiveClientAsync(CancellationToken cancellationToken = default)
    {
        if (_useSimulatedHardware)
            return;

        if (_activeClient is not null)
            return;

        await ApplyActiveDeviceAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisposeActiveClientCoreAsync().ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }

    private async Task DisposeActiveClientCoreAsync()
    {
        if (_activeClient is null)
            return;

        await _activeClient.DisposeAsync().ConfigureAwait(false);
        _activeClient = null;
        _activeClientSlot = -1;
    }

    private static StationDeviceConfiguration CloneConfiguration(StationDeviceConfiguration source)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(source);
        return System.Text.Json.JsonSerializer.Deserialize<StationDeviceConfiguration>(json)
               ?? new StationDeviceConfiguration { StationId = source.StationId };
    }
}
