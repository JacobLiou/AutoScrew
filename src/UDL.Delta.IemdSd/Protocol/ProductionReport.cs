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

    /// <summary>Report timestamp from #750 0x136–0x13B when present; null if invalid.</summary>
    public DateTime? Timestamp { get; init; }

    /// <summary>Torque unit code at 0x14E (0=N·m / 1=kgf·cm / 2=lbf·ft / 3=lbf·in).</summary>
    public ushort TorqueUnit { get; init; }

    /// <summary>User account id at 0x164 (1–5 User, 6 Admin).</summary>
    public ushort UserId { get; init; }
}
