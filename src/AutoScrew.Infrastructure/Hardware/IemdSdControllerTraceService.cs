using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class IemdSdControllerTraceService : IControllerTraceService
{
    private readonly IStationDeviceService _devices;
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly ILogger<IemdSdControllerTraceService> _logger;

    public IemdSdControllerTraceService(
        IStationDeviceService devices,
        IOptions<AutoScrewAppOptions> options,
        ILogger<IemdSdControllerTraceService> logger)
    {
        _devices = devices;
        _options = options;
        _logger = logger;
    }

    public async Task WriteSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serialNumber);

        if (!_options.Value.WriteSnToController || _devices.IsSimulatedHardware)
            return;

        try
        {
            await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
            var client = _devices.GetClient()
                         ?? throw new InvalidOperationException("IEMD-SD device is not connected.");
            await client.WriteBarcodeAsync(serialNumber.Trim(), cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Wrote SN barcode to controller (#401): {SerialNumber}", serialNumber);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write SN barcode to controller for {SerialNumber}", serialNumber);
            if (_options.Value.StrictSnToController)
                throw;
        }
    }
}
