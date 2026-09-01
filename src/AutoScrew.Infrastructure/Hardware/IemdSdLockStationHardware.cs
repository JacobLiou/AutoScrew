using System.Runtime.CompilerServices;
using AutoScrew.Application.Abstractions;
using AutoScrew.Domain.Curves;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// 台达 IEMD-SD 电批：开工 #300+#303 激活工艺库顺序，周期 GetResultStatus + #750/#751。
/// 供钉仍由 <see cref="SimulatedLockStationHardware"/> 仿真。
/// </summary>
public sealed class IemdSdLockStationHardware : ILockStationHardware
{
    private readonly IStationDeviceService _devices;
    private readonly SimulatedLockStationHardware _feederSim;
    private readonly ILogger<IemdSdLockStationHardware> _logger;

    public IemdSdLockStationHardware(
        IStationDeviceService devices,
        SimulatedLockStationHardware feederSim,
        ILogger<IemdSdLockStationHardware> logger)
    {
        _devices = devices;
        _feederSim = feederSim;
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

    public async Task PrepareForJobAsync(CancellationToken cancellationToken = default, int? sequenceId = null)
    {
        await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
        var client = _devices.GetClient();
        if (client is null)
            return;

        await IemdSdProductionSetup.EnsureManualSourceAsync(client, _logger, cancellationToken)
            .ConfigureAwait(false);

        if (sequenceId is not > 0)
        {
            _logger.LogWarning(
                "No process-library sequence id for this job; #303 skipped (no per-screw #302 fallback).");
            return;
        }

        await client.SwitchSequenceUnderManualAsync(sequenceId.Value, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Activated process-library sequence {SeqId} (#303).", sequenceId.Value);
    }

    public async IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
        TighteningContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await _devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
        var client = _devices.GetClient()
                     ?? throw new InvalidOperationException("IEMD-SD device is not connected.");

        // Cycle + #750/#751 under one exclusive device session (IsBusy held throughout).
        // 不发 #302：参数由已激活的拧紧顺序步进。
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

        // OK/NG 以 cycle/report 为准；曲线仅用于显示与归档，读不到时保持空点，不造假单点。
        LastOutcome = new LockHardwareOutcome(
            cycle.IsOk,
            report?.AppliedTorqueNm ?? (float)cycle.FinalTorqueNm,
            report?.TotalAngle ?? cycle.TotalAngle,
            report?.ErrorCode,
            reportId);

        if (curve is not { Points.Count: > 0 })
        {
            _logger.LogDebug(
                "No #751 curve points for ReportId={ReportId}; UI plot stays empty (device result unchanged).",
                reportId);
            yield break;
        }

        foreach (var p in curve.Points)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new TorqueAngleSample(p.TimeMs, p.TorqueNm, p.AngleDeg, 0, null);
        }
    }
}
