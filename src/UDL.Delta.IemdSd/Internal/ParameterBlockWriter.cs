using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ParameterBlockWriter
{
    private readonly IIemdSdCommandExecutor _executor;
    private readonly int _toolIndex;

    public ParameterBlockWriter(IIemdSdCommandExecutor executor, int toolIndex)
    {
        _executor = executor;
        _toolIndex = toolIndex;
    }

    public async Task WriteAsync(TighteningParameterTemplate template, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(template);
        ParameterBlockReader.ValidateParameterId(template.ParameterId);

        if (template.RawBlock.Length != ModbusRegisterMap.ParameterBlockWordCount)
            throw new ArgumentException($"Raw block must contain {ModbusRegisterMap.ParameterBlockWordCount} words.");

        // 编辑器 Raw 与 Core 保持未掩码同步（本地 JSON / 切换策略不丢槽位数据）。
        template.ApplyCoreToRaw();

        // 写出副本：按策略掩码清非活动槽，不改动调用方 Core。
        var writeCore = CloneCoreForWrite(template.Core);
        TighteningStrategyHelper.ApplyStrategyMask(writeCore.Stages, writeCore.Strategy);
        var writeBlock = (int[])template.RawBlock.Clone();
        TighteningParameterCodec.ApplyCoreToRaw(writeBlock, writeCore);

        var word4 = TighteningStrategyHelper.ToMailboxWord4(writeCore.Strategy);

        await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                ModbusFunctionCodes.WriteParameter,
                writeBlock,
                word2: _toolIndex,
                word3: template.ParameterId,
                word4: word4),
            cancellationToken).ConfigureAwait(false);
    }

    private static TighteningParameterCore CloneCoreForWrite(TighteningParameterCore source)
    {
        var clone = new TighteningParameterCore
        {
            Name = source.Name,
            Strategy = source.Strategy,
            MinAngleDeg = source.MinAngleDeg,
            LastStageServoOn = source.LastStageServoOn,
            LinkedCompensationParamId = source.LinkedCompensationParamId,
            MaxTighteningTimeTenthSec = source.MaxTighteningTimeTenthSec,
            MaxLoosenTimeTenthSec = source.MaxLoosenTimeTenthSec,
            MaxAngleDeg = source.MaxAngleDeg,
            MaxLoosenAngleDeg = source.MaxLoosenAngleDeg,
            TighteningStartDelayCentiSec = source.TighteningStartDelayCentiSec,
            LoosenStartDelayCentiSec = source.LoosenStartDelayCentiSec,
            CurveSampleStartTorqueMilliNm = source.CurveSampleStartTorqueMilliNm,
            SeatAngleStartTorqueRate = source.SeatAngleStartTorqueRate,
            SeatPointAngleCorrectionTenthDeg = source.SeatPointAngleCorrectionTenthDeg,
            FinalCurrentJudgeEnabled = source.FinalCurrentJudgeEnabled,
            FeederResultDelayTenthSec = source.FeederResultDelayTenthSec,
            ToolPrecisionCompTenthPercent = source.ToolPrecisionCompTenthPercent,
            TorqueRateAngleDelayTenthDeg = source.TorqueRateAngleDelayTenthDeg,
            Loosen = source.Loosen,
            Stages = TighteningParameterCore.CreateDefaultStages(),
        };

        for (var i = 0; i < 6 && i < source.Stages.Count; i++)
            clone.Stages[i] = CloneStage(source.Stages[i]);

        return clone;
    }

    private static TighteningStageCore CloneStage(TighteningStageCore s) => new()
    {
        ControlMode = s.ControlMode,
        Direction = s.Direction,
        SpeedRpm = s.SpeedRpm,
        TargetTorqueMilliNm = s.TargetTorqueMilliNm,
        TargetAngleDeg = s.TargetAngleDeg,
        TargetTorqueRate = s.TargetTorqueRate,
        TorqueRateAngleIntervalTenthDeg = s.TorqueRateAngleIntervalTenthDeg,
        AccelTimeMs = s.AccelTimeMs,
        MaxAngleDeg = s.MaxAngleDeg,
        MinAngleDeg = s.MinAngleDeg,
        MaxTorqueMilliNm = s.MaxTorqueMilliNm,
        MinTorqueMilliNm = s.MinTorqueMilliNm,
        MaxRunTimeCentiSec = s.MaxRunTimeCentiSec,
        MinRunTimeCentiSec = s.MinRunTimeCentiSec,
        CompTorqueEnabled = s.CompTorqueEnabled,
        CompTorqueAnglePercent = s.CompTorqueAnglePercent,
        PauseTimeMs = s.PauseTimeMs,
        MaxClampTorqueMilliNm = s.MaxClampTorqueMilliNm,
        MinClampTorqueMilliNm = s.MinClampTorqueMilliNm,
        MaxClampAngleDeg = s.MaxClampAngleDeg,
        MinClampAngleDeg = s.MinClampAngleDeg,
        Segment1TorqueMilliNm = s.Segment1TorqueMilliNm,
        Segment1PauseMs = s.Segment1PauseMs,
        Segment2AccelMs = s.Segment2AccelMs,
        FinalSpeedRpm = s.FinalSpeedRpm,
        DecelTimeMs = s.DecelTimeMs,
    };
}
