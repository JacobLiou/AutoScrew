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
                    bitId,
                    d.SourceProductPn,
                    d.SourceSequenceId);
            })
            .ToList();
    }

    public Task<TighteningSequencePackage> LoadLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default) =>
        _store.LoadAsync(sequenceId, cancellationToken);

    public Task SaveLocalPresetAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default) =>
        _store.SaveAsync(package, cancellationToken);

    public Task SaveLocalPresetWithOriginAsync(
        TighteningSequencePackage package,
        string sourceProductPn,
        int sourceSequenceId,
        CancellationToken cancellationToken = default) =>
        _store.SaveAsync(package, cancellationToken, sourceProductPn, sourceSequenceId);

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

    public async Task<IReadOnlyList<ControllerDeviceSequenceEntry>> ListDeviceSequenceEntriesAsync(
        CancellationToken cancellationToken = default)
    {
        var ids = await ListDeviceSequenceIdsAsync(cancellationToken).ConfigureAwait(false);
        if (ids.Count == 0)
            return [];

        // Must be sequential: RequireIdleClientAsync throws while DeviceSession.IsBusy.
        // Concurrent name reads race (second call sees busy) and degrade to ID-only display.
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var results = new List<ControllerDeviceSequenceEntry>(ids.Count);
        foreach (var sequenceId in ids)
        {
            try
            {
                var template = await client.ReadSequenceAsync(sequenceId, cancellationToken).ConfigureAwait(false);
                template.ExtractCoreFromRaw();
                results.Add(new ControllerDeviceSequenceEntry(
                    sequenceId,
                    template.Core?.Name?.Trim() ?? string.Empty));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ListDeviceSequenceEntries: failed to read name for sequence {SeqId}", sequenceId);
                results.Add(new ControllerDeviceSequenceEntry(sequenceId, string.Empty));
            }
        }

        return results;
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

    public async Task DeleteFromDeviceAsync(int sequenceId, CancellationToken cancellationToken = default)
    {
        if (sequenceId is < 1 or > TighteningSequenceRegisterMap.MaxSteps)
            throw new ArgumentOutOfRangeException(
                nameof(sequenceId),
                sequenceId,
                $"Sequence ID must be 1–{TighteningSequenceRegisterMap.MaxSteps}.");

        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.DeleteSequenceAsync(sequenceId, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Deleted sequence {SeqId} from IEMD-SD (#210)", sequenceId);
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
