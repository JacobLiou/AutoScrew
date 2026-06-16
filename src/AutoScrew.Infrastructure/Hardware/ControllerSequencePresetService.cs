using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class ControllerSequencePresetService : IControllerSequencePresetService
{
    private readonly LocalJsonControllerSequencePresetStore _store;
    private readonly IStationDeviceService _devices;
    private readonly ILogger<ControllerSequencePresetService> _logger;

    public ControllerSequencePresetService(
        LocalJsonControllerSequencePresetStore store,
        IStationDeviceService devices,
        ILogger<ControllerSequencePresetService> logger)
    {
        _store = store;
        _devices = devices;
        _logger = logger;
    }

    public bool IsDeviceAvailable => _devices.IsRuntimeDeviceAvailable;

    public async Task<IReadOnlyList<ControllerSequencePresetSummary>> ListLocalPresetsAsync(
        CancellationToken cancellationToken = default)
    {
        var docs = await _store.ListAsync(cancellationToken).ConfigureAwait(false);
        return docs
            .Select(d =>
            {
                var pkg = d.ToPackage();
                return new ControllerSequencePresetSummary(
                    pkg.SequenceId,
                    pkg.Core.Name,
                    pkg.Core.Steps.Count);
            })
            .ToList();
    }

    public Task<TighteningSequencePackage> LoadLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default) =>
        _store.LoadAsync(sequenceId, cancellationToken);

    public Task SaveLocalPresetAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default) =>
        _store.SaveAsync(package, cancellationToken);

    public Task DeleteLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default) =>
        _store.DeleteAsync(sequenceId, cancellationToken);

    public Task<TighteningSequencePackage> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default) =>
        _store.ImportFromFileAsync(filePath, cancellationToken);

    public Task ExportToFileAsync(TighteningSequencePackage package, string filePath, CancellationToken cancellationToken = default) =>
        _store.ExportToFileAsync(package, filePath, cancellationToken);

    public async Task<TighteningSequencePackage> ReadFromDeviceAsync(
        int sequenceId,
        CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var main = await client.ReadSequenceAsync(sequenceId, cancellationToken).ConfigureAwait(false);
        var nav = await client.ReadNavigatorCoordinatesAsync(
            sequenceId,
            (uint)TighteningSequencePackage.CreateNavigatorRawBlock().Length,
            cancellationToken).ConfigureAwait(false);
        var images = await client.ReadNavigatorImageCodesAsync(
            sequenceId,
            (uint)TighteningSequencePackage.CreateNavigatorImageRawBlock().Length,
            cancellationToken).ConfigureAwait(false);
        var arm = await client.ReadPositioningArmCoordinatesAsync(
            sequenceId,
            (uint)TighteningSequencePackage.CreatePositioningArmRawBlock().Length,
            cancellationToken).ConfigureAwait(false);

        var pkg = new TighteningSequencePackage
        {
            SequenceId = sequenceId,
            MainRawBlock = main.RawBlock,
            NavigatorRawBlock = nav,
            NavigatorImageRawBlock = images,
            PositioningArmRawBlock = arm,
        };
        pkg.ExtractCoreFromRaw();
        _logger.LogInformation("Read sequence {SeqId} from IEMD-SD", sequenceId);
        return pkg;
    }

    public async Task WriteToDeviceAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(package);
        package.ApplyCoreToRaw();
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.WriteSequenceAsync(package.ToTemplate(), cancellationToken).ConfigureAwait(false);
        await client.WriteNavigatorCoordinatesAsync(package.SequenceId, package.NavigatorRawBlock, cancellationToken)
            .ConfigureAwait(false);
        await client.WriteNavigatorImageCodesAsync(package.SequenceId, package.NavigatorImageRawBlock, cancellationToken)
            .ConfigureAwait(false);
        await client.WritePositioningArmCoordinatesAsync(package.SequenceId, package.PositioningArmRawBlock, cancellationToken)
            .ConfigureAwait(false);
        _logger.LogInformation("Wrote sequence {SeqId} to IEMD-SD", package.SequenceId);
    }

    public async Task ActivateOnDeviceAsync(int sequenceId, CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.SwitchSequenceUnderManualAsync(sequenceId, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Activated sequence {SeqId} on IEMD-SD (#303)", sequenceId);
    }

    private async Task<IIemdSdClient> RequireClientAsync(CancellationToken cancellationToken)
    {
        if (!_devices.IsRuntimeDeviceAvailable)
            throw new InvalidOperationException("IEMD-SD device is not available in the current configuration.");

        await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
        return _devices.GetClient()
               ?? throw new InvalidOperationException("IEMD-SD device is not connected. Configure it on the Device Connection page.");
    }
}
