namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningResult
{
    public DeviceTighteningStatus Status { get; init; }

    public int TotalAngle { get; init; }

    public double FinalTorqueNm { get; init; }

    public double PrevailTorqueNm { get; init; }

    public uint ReportId { get; init; }

    public bool IsOk => Status is DeviceTighteningStatus.Ok or DeviceTighteningStatus.Pass;
}
