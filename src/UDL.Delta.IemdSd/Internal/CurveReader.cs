using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class CurveReader
{
    private readonly IIemdSdCommandExecutor _executor;

    public CurveReader(IIemdSdCommandExecutor executor)
    {
        _executor = executor;
    }

    public async Task<CurveSnapshot> ReadAsync(uint reportId, CancellationToken cancellationToken)
    {
        var scaleWords = await ReadBlockAsync(reportId, (int)CurveReadMode.Scale, 150, cancellationToken)
            .ConfigureAwait(false);
        var totalPoints = (uint)scaleWords[28];
        if (totalPoints == 0)
        {
            return new CurveSnapshot
            {
                ReportId = reportId,
                TotalPoints = 0,
                Scale = ParseScale(scaleWords),
            };
        }

        var cappedPoints = (int)Math.Min(totalPoints, 8000u);
        var angle = new int[cappedPoints];
        var torque = new int[cappedPoints * 2];

        var angleOffset = 0;
        while (angleOffset < cappedPoints)
        {
            var mode = AngleMode(angleOffset);
            var chunk = Math.Min(2000, cappedPoints - angleOffset);
            var data = await ReadBlockAsync(reportId, mode, chunk, cancellationToken).ConfigureAwait(false);
            var copyLen = Math.Min(chunk, data.Length);
            Array.Copy(data, 0, angle, angleOffset, copyLen);
            angleOffset += chunk;
        }

        var torquePoints = 0;
        while (torquePoints < cappedPoints)
        {
            var mode = TorqueMode(torquePoints);
            var chunk = Math.Min(1000, cappedPoints - torquePoints);
            var data = await ReadBlockAsync(reportId, mode, chunk * 2, cancellationToken).ConfigureAwait(false);
            // Contiguous assembly: each point is 2 words at index point*2 (not fixed band slots).
            var dest = torquePoints * 2;
            var copyLen = Math.Min(chunk * 2, data.Length);
            Array.Copy(data, 0, torque, dest, copyLen);
            torquePoints += chunk;
        }

        var paramWords = await ReadBlockAsync(reportId, (int)CurveReadMode.Parameter, 550, cancellationToken)
            .ConfigureAwait(false);

        var points = ComposePoints(angle, torque, cappedPoints);

        return new CurveSnapshot
        {
            ReportId = reportId,
            TotalPoints = totalPoints,
            Points = points,
            Scale = ParseScale(scaleWords),
            ParameterId = (ushort)paramWords[0],
        };
    }

    /// <summary>Compose display points from #751 angle (1 word) + torque (2 words) buffers.</summary>
    internal static List<CurvePoint> ComposePoints(int[] angle, int[] torque, int pointCount)
    {
        var points = new List<CurvePoint>(pointCount);
        for (var i = 0; i < pointCount; i++)
        {
            var angleDeg = (short)angle[i];
            var torqueMilli = (short)torque[i * 2 + 1] * 65536 + (ushort)torque[i * 2];
            points.Add(new CurvePoint(i * 3.0, angleDeg, torqueMilli / 1000.0));
        }

        return points;
    }

    private async Task<int[]> ReadBlockAsync(uint reportId, int mode, int wordCount, CancellationToken cancellationToken)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReportId(ModbusFunctionCodes.ReadCurve, reportId, (uint)wordCount, mode),
            cancellationToken).ConfigureAwait(false);
        return result.ReadPayload
               ?? throw new InvalidOperationException("Curve read returned no payload.");
    }

    private static CurveScaleInfo ParseScale(int[] s) => new()
    {
        TotalPoints = (uint)s[28],
        MaxTorqueNm = (short)s[26] / 1000f,
        MaxAngle = (short)s[25],
        MaxTimeSec = (short)s[24] / 1000f,
    };

    private static int AngleMode(int offset) => offset switch
    {
        < 2000 => 1,
        < 4000 => 21,
        < 6000 => 31,
        _ => 41,
    };

    private static int TorqueMode(int offset) => offset switch
    {
        < 1000 => 4,
        < 2000 => 5,
        < 3000 => 24,
        < 4000 => 25,
        < 5000 => 34,
        < 6000 => 35,
        < 7000 => 44,
        _ => 45,
    };
}
