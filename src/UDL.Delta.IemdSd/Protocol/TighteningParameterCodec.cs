using System.Text;
using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Protocol;

public static class TighteningParameterCodec
{
    public static TighteningParameterCore ExtractCoreFromRaw(int[] raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        if (raw.Length != ModbusRegisterMap.ParameterBlockWordCount)
            throw new ArgumentException($"Expected {ModbusRegisterMap.ParameterBlockWordCount} words.", nameof(raw));

        var core = new TighteningParameterCore
        {
            Name = ReadName(raw),
            MinAngleDeg = raw[TighteningParameterRegisterMap.MinAngle],
            LastStageServoOn = raw[TighteningParameterRegisterMap.LastStageServoOn] != 0,
            LinkedCompensationParamId = raw[TighteningParameterRegisterMap.LinkedCompensationParamId],
            MaxTighteningTimeTenthSec = raw[TighteningParameterRegisterMap.MaxTighteningTimeTenthSec],
            MaxLoosenTimeTenthSec = raw[TighteningParameterRegisterMap.MaxLoosenTimeTenthSec],
            MaxAngleDeg = raw[TighteningParameterRegisterMap.MaxAngle],
            MaxLoosenAngleDeg = raw[TighteningParameterRegisterMap.MaxLoosenAngle],
            TighteningStartDelayCentiSec = raw[TighteningParameterRegisterMap.TighteningStartDelayCentiSec],
            LoosenStartDelayCentiSec = raw[TighteningParameterRegisterMap.LoosenStartDelayCentiSec],
            CurveSampleStartTorqueMilliNm = raw[TighteningParameterRegisterMap.CurveSampleStartTorqueMilliNm],
            SeatAngleStartTorqueRate = raw[TighteningParameterRegisterMap.SeatAngleStartTorqueRate],
            SeatPointAngleCorrectionTenthDeg = raw[TighteningParameterRegisterMap.SeatPointAngleCorrectionTenthDeg],
            FinalCurrentJudgeEnabled = raw[TighteningParameterRegisterMap.FinalCurrentJudge] != 0,
            FeederResultDelayTenthSec = raw[TighteningParameterRegisterMap.FeederResultDelayTenthSec],
            ToolPrecisionCompTenthPercent = (short)raw[TighteningParameterRegisterMap.ToolPrecisionCompTenthPercent],
            TorqueRateAngleDelayTenthDeg = raw[TighteningParameterRegisterMap.TorqueRateAngleDelayTenthDeg],
            Loosen = ReadLoosen(raw),
        };

        var stages = new List<TighteningStageCore>(6);
        for (var i = 0; i < 6; i++)
            stages.Add(ReadStage(raw, TighteningParameterRegisterMap.StageStarts[i]));
        core.Stages = stages;
        return core;
    }

    public static void ApplyCoreToRaw(int[] raw, TighteningParameterCore core)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(core);
        if (raw.Length != ModbusRegisterMap.ParameterBlockWordCount)
            throw new ArgumentException($"Expected {ModbusRegisterMap.ParameterBlockWordCount} words.", nameof(raw));

        WriteName(raw, core.Name);
        raw[TighteningParameterRegisterMap.MinAngle] = core.MinAngleDeg;
        raw[TighteningParameterRegisterMap.LastStageServoOn] = core.LastStageServoOn ? 1 : 0;
        raw[TighteningParameterRegisterMap.LinkedCompensationParamId] = core.LinkedCompensationParamId;
        raw[TighteningParameterRegisterMap.MaxTighteningTimeTenthSec] = core.MaxTighteningTimeTenthSec;
        raw[TighteningParameterRegisterMap.MaxLoosenTimeTenthSec] = core.MaxLoosenTimeTenthSec;
        raw[TighteningParameterRegisterMap.MaxAngle] = core.MaxAngleDeg;
        raw[TighteningParameterRegisterMap.MaxLoosenAngle] = core.MaxLoosenAngleDeg;
        raw[TighteningParameterRegisterMap.TighteningStartDelayCentiSec] = core.TighteningStartDelayCentiSec;
        raw[TighteningParameterRegisterMap.LoosenStartDelayCentiSec] = core.LoosenStartDelayCentiSec;
        raw[TighteningParameterRegisterMap.CurveSampleStartTorqueMilliNm] = core.CurveSampleStartTorqueMilliNm;
        raw[TighteningParameterRegisterMap.SeatAngleStartTorqueRate] = core.SeatAngleStartTorqueRate;
        raw[TighteningParameterRegisterMap.SeatPointAngleCorrectionTenthDeg] = core.SeatPointAngleCorrectionTenthDeg;
        raw[TighteningParameterRegisterMap.FinalCurrentJudge] = core.FinalCurrentJudgeEnabled ? 1 : 0;
        raw[TighteningParameterRegisterMap.FeederResultDelayTenthSec] = core.FeederResultDelayTenthSec;
        raw[TighteningParameterRegisterMap.ToolPrecisionCompTenthPercent] = (ushort)core.ToolPrecisionCompTenthPercent;
        raw[TighteningParameterRegisterMap.TorqueRateAngleDelayTenthDeg] = core.TorqueRateAngleDelayTenthDeg;

        for (var i = 0; i < 6; i++)
        {
            var stage = core.Stages.Count > i ? core.Stages[i] : new TighteningStageCore();
            WriteStage(raw, TighteningParameterRegisterMap.StageStarts[i], stage);
        }

        WriteLoosen(raw, core.Loosen);
    }

    public static string ReadName(int[] raw)
    {
        // Device HMI packs ASCII with low-byte-first within each Modbus word (same as SequenceCodec).
        var bytes = new List<byte>(TighteningParameterRegisterMap.NameWordCount * 2);
        for (var i = 0; i < TighteningParameterRegisterMap.NameWordCount; i++)
        {
            var word = (ushort)raw[TighteningParameterRegisterMap.NameStart + i];
            bytes.Add((byte)(word & 0xFF));
            bytes.Add((byte)(word >> 8));
        }

        var length = bytes.IndexOf((byte)0);
        if (length < 0)
            length = bytes.Count;

        return Encoding.ASCII.GetString(bytes.ToArray(), 0, length).Trim();
    }

    public static void WriteName(int[] raw, string name)
    {
        var text = name ?? string.Empty;
        if (text.Length > TighteningParameterRegisterMap.NameWordCount * 2 - 1)
            text = text[..(TighteningParameterRegisterMap.NameWordCount * 2 - 1)];

        var bytes = Encoding.ASCII.GetBytes(text);
        for (var i = 0; i < TighteningParameterRegisterMap.NameWordCount; i++)
        {
            var lo = i * 2 < bytes.Length ? bytes[i * 2] : (byte)0;
            var hi = i * 2 + 1 < bytes.Length ? bytes[i * 2 + 1] : (byte)0;
            raw[TighteningParameterRegisterMap.NameStart + i] = (hi << 8) | lo;
        }
    }

    private static TighteningStageCore ReadStage(int[] raw, int baseIndex)
    {
        return new TighteningStageCore
        {
            ControlMode = (TighteningControlMode)(ushort)raw[baseIndex + TighteningParameterRegisterMap.StageControlMode],
            Direction = (TighteningDirection)(ushort)raw[baseIndex + TighteningParameterRegisterMap.StageDirection],
            SpeedRpm = raw[baseIndex + TighteningParameterRegisterMap.StageSpeedRpm],
            TargetTorqueMilliNm = raw[baseIndex + TighteningParameterRegisterMap.StageTargetTorqueMilliNm],
            TargetAngleDeg = raw[baseIndex + TighteningParameterRegisterMap.StageTargetAngleDeg],
            TargetTorqueRate = raw[baseIndex + TighteningParameterRegisterMap.StageTargetTorqueRate],
            TorqueRateAngleIntervalTenthDeg = raw[baseIndex + TighteningParameterRegisterMap.StageTorqueRateAngleIntervalTenthDeg],
            AccelTimeMs = raw[baseIndex + TighteningParameterRegisterMap.StageAccelTimeMs],
            MaxAngleDeg = raw[baseIndex + TighteningParameterRegisterMap.StageMaxAngleDeg],
            MinAngleDeg = raw[baseIndex + TighteningParameterRegisterMap.StageMinAngleDeg],
            MaxTorqueMilliNm = raw[baseIndex + TighteningParameterRegisterMap.StageMaxTorqueMilliNm],
            MinTorqueMilliNm = raw[baseIndex + TighteningParameterRegisterMap.StageMinTorqueMilliNm],
            MaxRunTimeCentiSec = raw[baseIndex + TighteningParameterRegisterMap.StageMaxRunTimeCentiSec],
            MinRunTimeCentiSec = raw[baseIndex + TighteningParameterRegisterMap.StageMinRunTimeCentiSec],
            CompTorqueEnabled = raw[baseIndex + TighteningParameterRegisterMap.StageCompTorqueSwitch] != 0,
            CompTorqueAnglePercent = raw[baseIndex + TighteningParameterRegisterMap.StageCompTorqueAnglePercent],
            PauseTimeMs = raw[baseIndex + TighteningParameterRegisterMap.StagePauseTimeMs],
            MaxClampTorqueMilliNm = raw[baseIndex + TighteningParameterRegisterMap.StageMaxClampTorqueMilliNm],
            MinClampTorqueMilliNm = raw[baseIndex + TighteningParameterRegisterMap.StageMinClampTorqueMilliNm],
            MaxClampAngleDeg = raw[baseIndex + TighteningParameterRegisterMap.StageMaxClampAngleDeg],
            MinClampAngleDeg = raw[baseIndex + TighteningParameterRegisterMap.StageMinClampAngleDeg],
            Segment1TorqueMilliNm = raw[baseIndex + TighteningParameterRegisterMap.StageSegment1TorqueMilliNm],
            Segment1PauseMs = raw[baseIndex + TighteningParameterRegisterMap.StageSegment1PauseMs],
            Segment2AccelMs = raw[baseIndex + TighteningParameterRegisterMap.StageSegment2AccelMs],
            FinalSpeedRpm = raw[baseIndex + TighteningParameterRegisterMap.StageFinalSpeedRpm],
            DecelTimeMs = raw[baseIndex + TighteningParameterRegisterMap.StageDecelTimeMs],
        };
    }

    private static void WriteStage(int[] raw, int baseIndex, TighteningStageCore stage)
    {
        raw[baseIndex + TighteningParameterRegisterMap.StageControlMode] = (ushort)stage.ControlMode;
        raw[baseIndex + TighteningParameterRegisterMap.StageDirection] = (ushort)stage.Direction;
        raw[baseIndex + TighteningParameterRegisterMap.StageSpeedRpm] = stage.SpeedRpm;
        raw[baseIndex + TighteningParameterRegisterMap.StageTargetTorqueMilliNm] = stage.TargetTorqueMilliNm;
        raw[baseIndex + TighteningParameterRegisterMap.StageTargetAngleDeg] = stage.TargetAngleDeg;
        raw[baseIndex + TighteningParameterRegisterMap.StageTargetTorqueRate] = stage.TargetTorqueRate;
        raw[baseIndex + TighteningParameterRegisterMap.StageTorqueRateAngleIntervalTenthDeg] = stage.TorqueRateAngleIntervalTenthDeg;
        raw[baseIndex + TighteningParameterRegisterMap.StageAccelTimeMs] = stage.AccelTimeMs;
        raw[baseIndex + TighteningParameterRegisterMap.StageMaxAngleDeg] = stage.MaxAngleDeg;
        raw[baseIndex + TighteningParameterRegisterMap.StageMinAngleDeg] = stage.MinAngleDeg;
        raw[baseIndex + TighteningParameterRegisterMap.StageMaxTorqueMilliNm] = stage.MaxTorqueMilliNm;
        raw[baseIndex + TighteningParameterRegisterMap.StageMinTorqueMilliNm] = stage.MinTorqueMilliNm;
        raw[baseIndex + TighteningParameterRegisterMap.StageMaxRunTimeCentiSec] = stage.MaxRunTimeCentiSec;
        raw[baseIndex + TighteningParameterRegisterMap.StageMinRunTimeCentiSec] = stage.MinRunTimeCentiSec;
        raw[baseIndex + TighteningParameterRegisterMap.StageCompTorqueSwitch] = stage.CompTorqueEnabled ? 1 : 0;
        raw[baseIndex + TighteningParameterRegisterMap.StageCompTorqueAnglePercent] = stage.CompTorqueAnglePercent;
        raw[baseIndex + TighteningParameterRegisterMap.StagePauseTimeMs] = stage.PauseTimeMs;
        raw[baseIndex + TighteningParameterRegisterMap.StageMaxClampTorqueMilliNm] = stage.MaxClampTorqueMilliNm;
        raw[baseIndex + TighteningParameterRegisterMap.StageMinClampTorqueMilliNm] = stage.MinClampTorqueMilliNm;
        raw[baseIndex + TighteningParameterRegisterMap.StageMaxClampAngleDeg] = stage.MaxClampAngleDeg;
        raw[baseIndex + TighteningParameterRegisterMap.StageMinClampAngleDeg] = stage.MinClampAngleDeg;
        raw[baseIndex + TighteningParameterRegisterMap.StageSegment1TorqueMilliNm] = stage.Segment1TorqueMilliNm;
        raw[baseIndex + TighteningParameterRegisterMap.StageSegment1PauseMs] = stage.Segment1PauseMs;
        raw[baseIndex + TighteningParameterRegisterMap.StageSegment2AccelMs] = stage.Segment2AccelMs;
        raw[baseIndex + TighteningParameterRegisterMap.StageFinalSpeedRpm] = stage.FinalSpeedRpm;
        raw[baseIndex + TighteningParameterRegisterMap.StageDecelTimeMs] = stage.DecelTimeMs;
    }

    private static TighteningLoosenCore ReadLoosen(int[] raw) => new()
    {
        Stage1AngleDeg = raw[TighteningParameterRegisterMap.LoosenStage1Angle],
        Stage1SpeedRpm = raw[TighteningParameterRegisterMap.LoosenStage1Speed],
        Stage2AngleDeg = raw[TighteningParameterRegisterMap.LoosenStage2Angle],
        Stage2SpeedRpm = raw[TighteningParameterRegisterMap.LoosenStage2Speed],
        Direction = (TighteningDirection)(ushort)raw[TighteningParameterRegisterMap.LoosenDirection],
        DetectTorqueMilliNm = raw[TighteningParameterRegisterMap.LoosenDetectTorqueMilliNm],
        ProductionLogEnabled = raw[TighteningParameterRegisterMap.ProductionLogSwitch] != 0,
        Stage1AccelMs = raw[TighteningParameterRegisterMap.LoosenStage1AccelMs],
        Stage2AccelMs = raw[TighteningParameterRegisterMap.LoosenStage2AccelMs],
    };

    private static void WriteLoosen(int[] raw, TighteningLoosenCore loosen)
    {
        raw[TighteningParameterRegisterMap.LoosenStage1Angle] = loosen.Stage1AngleDeg;
        raw[TighteningParameterRegisterMap.LoosenStage1Speed] = loosen.Stage1SpeedRpm;
        raw[TighteningParameterRegisterMap.LoosenStage2Angle] = loosen.Stage2AngleDeg;
        raw[TighteningParameterRegisterMap.LoosenStage2Speed] = loosen.Stage2SpeedRpm;
        raw[TighteningParameterRegisterMap.LoosenDirection] = (ushort)loosen.Direction;
        raw[TighteningParameterRegisterMap.LoosenDetectTorqueMilliNm] = loosen.DetectTorqueMilliNm;
        raw[TighteningParameterRegisterMap.ProductionLogSwitch] = loosen.ProductionLogEnabled ? 1 : 0;
        raw[TighteningParameterRegisterMap.LoosenStage1AccelMs] = loosen.Stage1AccelMs;
        raw[TighteningParameterRegisterMap.LoosenStage2AccelMs] = loosen.Stage2AccelMs;
    }
}
