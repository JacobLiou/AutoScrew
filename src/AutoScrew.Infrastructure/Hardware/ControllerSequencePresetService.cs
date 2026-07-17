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
                var steps = pkg.Core.Steps;
                var bitId = steps.Count > 0 ? steps[0].BitId : 0;
                return new ControllerSequencePresetSummary(
                    pkg.SequenceId,
                    pkg.Core.Name,
                    steps.Count,
                    bitId);
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

    public async Task<IReadOnlyList<int>> ListDeviceSequenceIdsAsync(CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var words = await client
            .ListSequencesAsync((uint)TighteningSequenceRegisterMap.MaxSteps, cancellationToken)
            .ConfigureAwait(false);
        var snapshot = new ParameterListSnapshot { RawWords = words };
        return snapshot.GetConfiguredIds()
            .Where(id => id is >= 1 and <= TighteningSequenceRegisterMap.MaxSteps)
            .ToList();
    }

    public async Task<TighteningSequencePackage> ImportFromDeviceAsync(
        int sequenceId,
        CancellationToken cancellationToken = default)
    {
        var pkg = await ReadFromDeviceAsync(sequenceId, cancellationToken).ConfigureAwait(false);
        await SaveLocalPresetAsync(pkg, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Imported sequence {SeqId} from IEMD-SD to local store", sequenceId);
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

    private Task<IIemdSdClient> RequireClientAsync(CancellationToken cancellationToken) =>
        StationDeviceClientGuard.RequireIdleClientAsync(_devices, cancellationToken);
}
