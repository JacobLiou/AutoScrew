namespace UDL.Delta.IemdSd.Protocol;

/// <summary>Subset of Appendix A.2 operating status registers.</summary>
public sealed class OperatingStatusSnapshot
{
    public DeviceTighteningStatus TighteningResult { get; init; }

    public float FinalTorqueNm { get; init; }

    public float CompTorqueNm { get; init; }

    public int TotalAngle { get; init; }

    public uint ReportId { get; init; }

    public int Ready { get; init; }

    public int TighteningFinish { get; init; }
}
