using System.Runtime.CompilerServices;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Domain.Curves;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// 台达 IEMD-SD 电批：Modbus #302/#303/#750/#751 + GetResultStatus 拧紧周期。
/// 供钉仍由 <see cref="SimulatedLockStationHardware"/> 仿真。
/// </summary>
public sealed class IemdSdLockStationHardware : ILockStationHardware
{
    private readonly IStationDeviceService _devices;
    private readonly IControllerSourceConfigService _sourceConfig;
    private readonly SimulatedLockStationHardware _feederSim;
    private readonly IemdSdOptions _options;
    private readonly AutoScrewAppOptions _appOptions;
    private readonly ILogger<IemdSdLockStationHardware> _logger;

    public IemdSdLockStationHardware(
        IStationDeviceService devices,
        IControllerSourceConfigService sourceConfig,
        SimulatedLockStationHardware feederSim,
        IOptions<IemdSdOptions> options,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<IemdSdLockStationHardware> logger)
    {
        _devices = devices;
        _sourceConfig = sourceConfig;
        _feederSim = feederSim;
        _options = options.Value;
        _appOptions = appOptions.Value;
        _logger = logger;
    }

    public LockHardwareOutcome? LastOutcome { get; private set; }

    public Task PickScrewAsync(CancellationToken cancellationToken = default) =>
        _feederSim.PickScrewAsync(cancellationToken);

    public async Task ClearErrorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
            var client = _devices.GetClient();
            if (client is null)
            {
                _logger.LogWarning("ClearErrors skipped: IEMD-SD client is not connected.");
                return;
            }

            await client.ClearErrorsAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("IEMD-SD ClearErrors completed.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ClearErrors failed; unlock may still proceed.");
        }
    }

    public async Task PrepareForJobAsync(CancellationToken cancellationToken = default)
    {
        await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
        var client = _devices.GetClient();
        if (client is null)
            return;

        var controlMode = await ResolveControlModeAsync(cancellationToken).ConfigureAwait(false);
        if (controlMode == ProductionTighteningMode.DeviceProgram)
        {
            try
            {
                var mode = await _sourceConfig.LoadLocalModeAsync(cancellationToken).ConfigureAwait(false);
                var content = await _sourceConfig.LoadLocalContentAsync(cancellationToken).ConfigureAwait(false);
                await _sourceConfig.WriteToDeviceAsync(mode, content, cancellationToken).ConfigureAwait(false);
                if (content.BindingType == TighteningSourceBindingType.Sequence && content.TargetId > 0)
                {
                    await client.SwitchSequenceUnderManualAsync(content.TargetId, cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation(
                        "DeviceProgram: activated sequence {SeqId} screwCount={Count}",
                        content.TargetId,
                        content.ScrewCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "DeviceProgram setup failed; falling back to HostGuided manual source.");
                await IemdSdProductionSetup.EnsureManualSourceAsync(client, _logger, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        else
        {
            await IemdSdProductionSetup.EnsureManualSourceAsync(client, _logger, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
        TighteningContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
        var client = _devices.GetClient()
                     ?? throw new InvalidOperationException("IEMD-SD device is not connected.");

        var controlMode = await ResolveControlModeAsync(cancellationToken).ConfigureAwait(false);
        if (controlMode == ProductionTighteningMode.HostGuided)
        {
            var paramId = context.ControllerParameterId;
            if (paramId <= 0 && _options.ParameterIdByPosition.TryGetValue(context.PositionIndex.ToString(), out var mapped))
                paramId = mapped;
            if (paramId <= 0)
                paramId = context.PositionIndex;

            await client.SwitchParameterAsync(paramId, 1, cancellationToken).ConfigureAwait(false);
        }

        // Cycle + #750/#751 under one exclusive device session (IsBusy held throughout).
        var artifacts = await client.ExecuteProductionTighteningAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var cycle = artifacts.Cycle;
        var report = artifacts.Report;
        var curve = artifacts.Curve;
        var reportId = artifacts.ReportId;

        if (artifacts.ArtifactReadError is not null)
        {
            _logger.LogWarning(
                "Read report/curve for ReportId={ReportId} failed; using cycle registers only. {Error}",
                reportId,
                artifacts.ArtifactReadError);
        }

        LastOutcome = new LockHardwareOutcome(
            cycle.IsOk,
            report?.AppliedTorqueNm ?? (float)cycle.FinalTorqueNm,
            report?.TotalAngle ?? cycle.TotalAngle,
            report?.ErrorCode,
            reportId);

        if (curve is { Points.Count: > 0 })
        {
            foreach (var p in curve.Points)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return new TorqueAngleSample(p.TimeMs, p.TorqueNm, p.AngleDeg, 0, null);
            }
        }
        else
        {
            yield return new TorqueAngleSample(0, cycle.FinalTorqueNm, cycle.TotalAngle, 0, null);
        }
    }

    private async Task<ProductionTighteningMode> ResolveControlModeAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _sourceConfig.LoadProductionControlModeAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Load production control mode failed; using appsettings default.");
            return _appOptions.TighteningControlMode;
        }
    }
}
