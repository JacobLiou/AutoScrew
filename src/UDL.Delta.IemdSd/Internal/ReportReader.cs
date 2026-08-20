using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ReportReader
{
    private readonly IIemdSdCommandExecutor _executor;

    public ReportReader(IIemdSdCommandExecutor executor)
    {
        _executor = executor;
    }

    public async Task<ProductionReport> ReadAsync(uint reportId, CancellationToken cancellationToken)
    {
        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReportId(ModbusFunctionCodes.ReadReport, reportId, 253),
            cancellationToken).ConfigureAwait(false);

        var words = result.ReadPayload
                      ?? throw new InvalidOperationException("Report read returned no payload.");

        return Parse(reportId, words);
    }

    internal static ProductionReport Parse(uint reportId, int[] w)
    {
        static ushort U(int v) => (ushort)v;
        static short S(int v) => (short)v;
        static uint Dw(int lo, int hi) => (uint)(hi * 65536 + (ushort)lo);
        static float Torque(int lo) => lo / 1000f;

        var status = (DeviceTighteningStatus)U(At(w, 0x147));
        var year = At(w, 0x136);
        var month = At(w, 0x137);
        var day = At(w, 0x138);
        var hour = At(w, 0x139);
        var minute = At(w, 0x13A);
        var second = At(w, 0x13B);

        return new ProductionReport
        {
            ReportId = reportId,
            Tool = U(At(w, 0x13C)),
            ScrewNo = Dw(At(w, 0x13D), At(w, 0x13E)),
            SeqId = U(At(w, 0x13F)),
            ParmId = U(At(w, 0x140)),
            TargetTorqueNm = Torque(At(w, 0x141)),
            TargetAngle = At(w, 0x142),
            FinalTorqueNm = Torque(At(w, 0x143)),
            TighteningAngle = At(w, 0x145),
            TotalAngle = S(At(w, 0x146)),
            Status = status,
            CycleTimeSec = U(At(w, 0x148)) / 1000f,
            ErrorCode = U(At(w, 0x149)),
            AppliedTorqueNm = (float)Dw(At(w, 0x17D), At(w, 0x17E)) / 1000f,
            PrevailTorqueNm = Torque(At(w, 0x152)),
            Timestamp = HistoryReportParser.TryParseTimestamp(year, month, day, hour, minute, second),
            TorqueUnit = U(At(w, 0x14E)),
            UserId = U(At(w, 0x164)),
        };
    }

    private static int At(int[] words, int hexOffset) => words[hexOffset - ModbusRegisterMap.CommandData];
}
