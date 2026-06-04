using System.Runtime.CompilerServices;
using AutoScrew.Application.Abstractions;
using AutoScrew.Domain.Curves;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// 台达 IEMD-SD 电批：Modbus #302/#750/#751 + GetResultStatus 拧紧周期。
/// 供钉仍由 <see cref="SimulatedLockStationHardware"/> 仿真。
/// </summary>
public sealed class IemdSdLockStationHardware : ILockStationHardware
{
    private readonly IStationDeviceService _devices;
    private readonly SimulatedLockStationHardware _feederSim;
    private readonly IemdSdOptions _options;
    private readonly ILogger<IemdSdLockStationHardware> _logger;

    public IemdSdLockStationHardware(
        IStationDeviceService devices,
        IOptions<IemdSdOptions> options,
        ILogger<IemdSdLockStationHardware> logger)
    {
        _devices = devices;
        _options = options.Value;
        _logger = logger;
        _feederSim = new SimulatedLockStationHardware();
    }

    public LockHardwareOutcome? LastOutcome { get; private set; }

    public Task PickScrewAsync(CancellationToken cancellationToken = default) =>
        _feederSim.PickScrewAsync(cancellationToken);

    public async IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
        TighteningContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await _devices.EnsureActiveClientAsync(cancellationToken).ConfigureAwait(false);
        var client = _devices.GetActiveClient()
                     ?? throw new InvalidOperationException("IEMD-SD active device is not connected.");

        var paramId = context.ControllerParameterId;
        if (paramId <= 0 && _options.ParameterIdByPosition.TryGetValue(context.PositionIndex.ToString(), out var mapped))
            paramId = mapped;
        if (paramId <= 0)
            paramId = context.PositionIndex;

        await client.SwitchParameterAsync(paramId, 1, cancellationToken).ConfigureAwait(false);

        var beforeId = await client.GetCurrentReportIdAsync(cancellationToken).ConfigureAwait(false);
        var cycle = await client.ExecuteTighteningCycleAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var reportId = cycle.ReportId > 0 ? cycle.ReportId : beforeId;
        ProductionReport? report = null;
        CurveSnapshot? curve = null;
        try
        {
            report = await client.ReadReportAsync(reportId, cancellationToken).ConfigureAwait(false);
            curve = await client.ReadCurveAsync(reportId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Read report/curve for ReportId={ReportId} failed; using cycle registers only.", reportId);
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
}
