namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningParameterCore
{
    public string Name { get; set; } = string.Empty;
    public int MinAngleDeg { get; set; }
    public bool LastStageServoOn { get; set; }
    public int LinkedCompensationParamId { get; set; }
    public int MaxTighteningTimeTenthSec { get; set; }
    public int MaxLoosenTimeTenthSec { get; set; }
    public int MaxAngleDeg { get; set; }
    public int MaxLoosenAngleDeg { get; set; }
    public int TighteningStartDelayCentiSec { get; set; }
    public int LoosenStartDelayCentiSec { get; set; }
    public int CurveSampleStartTorqueMilliNm { get; set; }
    public int SeatAngleStartTorqueRate { get; set; }
    public int SeatPointAngleCorrectionTenthDeg { get; set; }
    public bool FinalCurrentJudgeEnabled { get; set; }
    public int FeederResultDelayTenthSec { get; set; }
    public int ToolPrecisionCompTenthPercent { get; set; }
    public int TorqueRateAngleDelayTenthDeg { get; set; }
    public IList<TighteningStageCore> Stages { get; set; } = CreateDefaultStages();
    public TighteningLoosenCore Loosen { get; set; } = new();

    public static IList<TighteningStageCore> CreateDefaultStages()
    {
        var stages = new TighteningStageCore[6];
        for (var i = 0; i < stages.Length; i++)
            stages[i] = new TighteningStageCore();
        return stages;
    }
}
