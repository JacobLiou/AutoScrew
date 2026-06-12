using System.Text.Json.Serialization;

namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningStageCore
{
    public TighteningControlMode ControlMode { get; set; }
    public TighteningDirection Direction { get; set; }
    public int SpeedRpm { get; set; }
    public int TargetTorqueMilliNm { get; set; }
    public int TargetAngleDeg { get; set; }
    public int TargetTorqueRate { get; set; }
    public int TorqueRateAngleIntervalTenthDeg { get; set; }
    public int AccelTimeMs { get; set; }
    public int MaxAngleDeg { get; set; }
    public int MinAngleDeg { get; set; }
    public int MaxTorqueMilliNm { get; set; }
    public int MinTorqueMilliNm { get; set; }
    public int MaxRunTimeCentiSec { get; set; }
    public int MinRunTimeCentiSec { get; set; }
    public bool CompTorqueEnabled { get; set; }
    public int CompTorqueAnglePercent { get; set; }
    public int PauseTimeMs { get; set; }
    public int MaxClampTorqueMilliNm { get; set; }
    public int MinClampTorqueMilliNm { get; set; }
    public int MaxClampAngleDeg { get; set; }
    public int MinClampAngleDeg { get; set; }
    public int Segment1TorqueMilliNm { get; set; }
    public int Segment1PauseMs { get; set; }
    public int Segment2AccelMs { get; set; }
    public int FinalSpeedRpm { get; set; }
    public int DecelTimeMs { get; set; }

    [JsonIgnore]
    public int ControlModeValue
    {
        get => (int)ControlMode;
        set => ControlMode = (TighteningControlMode)value;
    }

    public float TargetTorqueNm => TargetTorqueMilliNm / 1000f;
    public float MaxTorqueNm => MaxTorqueMilliNm / 1000f;
    public float MinTorqueNm => MinTorqueMilliNm / 1000f;
}
