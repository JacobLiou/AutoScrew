namespace UDL.Delta.IemdSd.Protocol;

public sealed class ProductionReport
{
    public uint ReportId { get; init; }

    public ushort Tool { get; init; }

    public uint ScrewNo { get; init; }

    public ushort SeqId { get; init; }

    public ushort ParmId { get; init; }

    public float TargetTorqueNm { get; init; }

    public int TargetAngle { get; init; }

    public float FinalTorqueNm { get; init; }

    public int TighteningAngle { get; init; }

    public int TotalAngle { get; init; }

    public DeviceTighteningStatus Status { get; init; }

    public float CycleTimeSec { get; init; }

    public ushort ErrorCode { get; init; }

    public float AppliedTorqueNm { get; init; }

    public float PrevailTorqueNm { get; init; }
}
