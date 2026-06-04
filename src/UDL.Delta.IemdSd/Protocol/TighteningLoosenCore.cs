namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningLoosenCore
{
    public int Stage1AngleDeg { get; set; }
    public int Stage1SpeedRpm { get; set; }
    public int Stage2AngleDeg { get; set; }
    public int Stage2SpeedRpm { get; set; }
    public TighteningDirection Direction { get; set; }
    public int DetectTorqueMilliNm { get; set; }
    public bool ProductionLogEnabled { get; set; }
    public int Stage1AccelMs { get; set; }
    public int Stage2AccelMs { get; set; }

    public float DetectTorqueNm => DetectTorqueMilliNm / 1000f;
}
