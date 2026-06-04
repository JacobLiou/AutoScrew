using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class CurveReader
{
    private readonly IModbusTransport _transport;
    private readonly CommandMailbox _mailbox;

    public CurveReader(IModbusTransport transport, CommandMailbox mailbox)
    {
        _transport = transport;
        _mailbox = mailbox;
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

        var angle = new int[Math.Min(totalPoints, 8000)];
        var torque = new int[Math.Min(totalPoints * 2, 16000)];

        var angleOffset = 0;
        while (angleOffset < totalPoints)
        {
            var mode = AngleMode(angleOffset);
            var chunk = (int)Math.Min(2000, totalPoints - angleOffset);
            var data = await ReadBlockAsync(reportId, mode, chunk, cancellationToken).ConfigureAwait(false);
            Array.Copy(data, 0, angle, angleOffset, chunk);
            angleOffset += chunk;
        }

        var torquePoints = 0;
        while (torquePoints < totalPoints)
        {
            var mode = TorqueMode(torquePoints);
            var chunk = (int)Math.Min(1000, totalPoints - torquePoints);
            var data = await ReadBlockAsync(reportId, mode, chunk * 2, cancellationToken).ConfigureAwait(false);
            var dest = TorqueDestIndex(torquePoints, totalPoints);
            Array.Copy(data, 0, torque, dest, chunk * 2);
            torquePoints += chunk;
        }

        var paramWords = await ReadBlockAsync(reportId, (int)CurveReadMode.Parameter, 550, cancellationToken)
            .ConfigureAwait(false);

        var points = new List<CurvePoint>((int)totalPoints);
        for (var i = 0; i < totalPoints; i++)
        {
            var t = (short)angle[i];
            var tr = (short)torque[i * 2 + 1] * 65536 + (ushort)torque[i * 2];
            points.Add(new CurvePoint(i * 3.0, t, tr / 1000.0));
        }

        return new CurveSnapshot
        {
            ReportId = reportId,
            TotalPoints = totalPoints,
            Points = points,
            Scale = ParseScale(scaleWords),
            ParameterId = (ushort)paramWords[0],
        };
    }

    private async Task<int[]> ReadBlockAsync(uint reportId, int mode, int wordCount, CancellationToken cancellationToken)
    {
        var req = CommandMailbox.CreateRequest(ModbusFunctionCodes.ReadCurve, word4: mode);
        CommandMailbox.SetReportId(req, reportId);
        await _mailbox.SendCommandAsync(ModbusFunctionCodes.ReadCurve, req, cancellationToken).ConfigureAwait(false);
        return await _transport.ReadHoldingAsync(ModbusRegisterMap.CommandData, wordCount, cancellationToken)
            .ConfigureAwait(false);
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

    private static int TorqueDestIndex(int torquePoints, uint totalPoints)
    {
        if (totalPoints < 1000) return 0;
        if (totalPoints < 2000) return 2000;
        if (totalPoints < 3000) return 4000;
        if (totalPoints < 4000) return 6000;
        if (totalPoints < 5000) return 8000;
        if (totalPoints < 6000) return 10000;
        if (totalPoints < 7000) return 12000;
        return 14000;
    }
}
