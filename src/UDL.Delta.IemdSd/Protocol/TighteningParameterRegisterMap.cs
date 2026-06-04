namespace UDL.Delta.IemdSd.Protocol;

/// <summary>349-word 参数块相对 0xD2 的字索引（手册 A.3.1）。</summary>
internal static class TighteningParameterRegisterMap
{
    public const int NameWordCount = 20;
    public const int NameStart = 0;

    public const int MinAngle = 0xE6 - 0xD2;
    public const int LastStageServoOn = 0xE7 - 0xD2;
    public const int LinkedCompensationParamId = 0xE8 - 0xD2;
    public const int MaxTighteningTimeTenthSec = 0xE9 - 0xD2;
    public const int MaxLoosenTimeTenthSec = 0xEA - 0xD2;
    public const int MaxAngle = 0xEB - 0xD2;
    public const int MaxLoosenAngle = 0xEC - 0xD2;
    public const int TighteningStartDelayCentiSec = 0xED - 0xD2;
    public const int LoosenStartDelayCentiSec = 0xEE - 0xD2;
    public const int CurveSampleStartTorqueMilliNm = 0xEF - 0xD2;
    public const int SeatAngleStartTorqueRate = 0xF0 - 0xD2;
    public const int SeatPointAngleCorrectionTenthDeg = 0xF1 - 0xD2;
    public const int FinalCurrentJudge = 0xF2 - 0xD2;
    public const int FeederResultDelayTenthSec = 0xF3 - 0xD2;
    public const int ToolPrecisionCompTenthPercent = 0xF4 - 0xD2;
    public const int TorqueRateAngleDelayTenthDeg = 0xF5 - 0xD2;

    public const int StageWordCount = 50;
    public static readonly int[] StageStarts =
    [
        0xFA - 0xD2,
        0x12C - 0xD2,
        0x15E - 0xD2,
        0x190 - 0xD2,
        0x1C2 - 0xD2,
        0x1F4 - 0xD2,
    ];

    public const int LoosenStage1Angle = 0x226 - 0xD2;
    public const int LoosenStage1Speed = 0x227 - 0xD2;
    public const int LoosenStage2Angle = 0x228 - 0xD2;
    public const int LoosenStage2Speed = 0x229 - 0xD2;
    public const int LoosenDirection = 0x22A - 0xD2;
    public const int LoosenDetectTorqueMilliNm = 0x22B - 0xD2;
    public const int ProductionLogSwitch = 0x22C - 0xD2;
    public const int LoosenStage1AccelMs = 0x22D - 0xD2;
    public const int LoosenStage2AccelMs = 0x22E - 0xD2;

    public const int StageControlMode = 0;
    public const int StageDirection = 1;
    public const int StageSpeedRpm = 2;
    public const int StageTargetTorqueMilliNm = 3;
    public const int StageTargetAngleDeg = 4;
    public const int StageTargetTorqueRate = 5;
    public const int StageTorqueRateAngleIntervalTenthDeg = 6;
    public const int StageAccelTimeMs = 7;
    public const int StageMaxAngleDeg = 8;
    public const int StageMinAngleDeg = 9;
    public const int StageMaxTorqueMilliNm = 10;
    public const int StageMinTorqueMilliNm = 11;
    public const int StageMaxRunTimeCentiSec = 12;
    public const int StageMinRunTimeCentiSec = 13;
    public const int StageCompTorqueSwitch = 14;
    public const int StageCompTorqueAnglePercent = 15;
    public const int StagePauseTimeMs = 16;
    public const int StageMaxClampTorqueMilliNm = 17;
    public const int StageMinClampTorqueMilliNm = 18;
    public const int StageMaxClampAngleDeg = 19;
    public const int StageMinClampAngleDeg = 20;
    public const int StageSegment1TorqueMilliNm = 21;
    public const int StageSegment1PauseMs = 22;
    public const int StageSegment2AccelMs = 23;
    public const int StageFinalSpeedRpm = 24;
    public const int StageDecelTimeMs = 25;
}
