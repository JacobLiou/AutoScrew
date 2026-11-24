using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ParameterBlockWriter
{
    /// <summary>对齐 SDSoft：超时寄存器为 0 时写 32767 表示不限制。</summary>
    public const int UnlimitedTimeTenthSec = 32767;

    private readonly IIemdSdCommandExecutor _executor;
    private readonly int _toolIndex;
    private readonly IemdSdControllerProfile _profile;
    private readonly ILogger? _logger;

    public ParameterBlockWriter(
        IIemdSdCommandExecutor executor,
        int toolIndex,
        IemdSdControllerProfile profile,
        ILogger? logger = null)
    {
        _executor = executor;
        _toolIndex = toolIndex;
        _profile = profile;
        _logger = logger;
    }

    public async Task WriteAsync(TighteningParameterTemplate template, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(template);
        ParameterBlockReader.ValidateParameterId(template.ParameterId);

        if (template.RawBlock.Length != ModbusRegisterMap.ParameterBlockWordCount)
            throw new ArgumentException($"Raw block must contain {ModbusRegisterMap.ParameterBlockWordCount} words.");

        // 编辑器 Raw 与 Core 保持未掩码同步（本地 JSON / 切换策略不丢槽位数据）。
        template.ApplyCoreToRaw();

        // 写出副本：按策略掩码清非活动槽，不改动调用方 Core；按协议档编码设备布局。
        var writeCore = CloneCoreForWrite(template.Core);
        TighteningStrategyHelper.ApplyStrategyMask(writeCore.Stages, writeCore.Strategy);
        NormalizeTimeoutsForDevice(writeCore);
        ValidateTimeoutsForDevice(writeCore);

        var writeBlock = (int[])template.RawBlock.Clone();
        TighteningParameterCodec.ApplyCoreToRaw(writeBlock, writeCore, _profile);

        var word4 = TighteningStrategyHelper.ToMailboxWord4(writeCore.Strategy, _profile);
        var blockTightening = writeBlock[TighteningParameterRegisterMap.MaxTighteningTimeTenthSec];
        var blockLoosen = writeBlock[TighteningParameterRegisterMap.MaxLoosenTimeTenthSec];
        _logger?.LogInformation(
            "COMM ParameterWrite ControllerProfile={ControllerProfile} ParameterId={ParameterId} Strategy={Strategy} MailboxWord4={MailboxWord4} MaxTighteningTimeTenthSec={MaxTighteningTimeTenthSec} MaxLoosenTimeTenthSec={MaxLoosenTimeTenthSec} BlockE9={BlockE9} BlockEA={BlockEA}",
            _profile,
            template.ParameterId,
            writeCore.Strategy,
            word4,
            writeCore.MaxTighteningTimeTenthSec,
            writeCore.MaxLoosenTimeTenthSec,
            blockTightening,
            blockLoosen);

        try
        {
            await _executor.ExecuteAsync(
                ModbusCommandInvocation.WithWritePayload(
                    ModbusFunctionCodes.WriteParameter,
                    writeBlock,
                    word2: _toolIndex,
                    word3: template.ParameterId,
                    word4: word4),
                cancellationToken).ConfigureAwait(false);
        }
        catch (IemdSdCommunicationException ex) when (ex.DeviceErrorCode is int)
        {
            _logger?.LogWarning(
                "COMM ParameterWrite Rejected DeviceErrorCode={DeviceErrorCode} MaxTighteningTimeTenthSec={MaxTighteningTimeTenthSec} MaxLoosenTimeTenthSec={MaxLoosenTimeTenthSec} BlockE9={BlockE9} BlockEA={BlockEA}",
                ex.DeviceErrorCode,
                writeCore.MaxTighteningTimeTenthSec,
                writeCore.MaxLoosenTimeTenthSec,
                blockTightening,
                blockLoosen);
            throw;
        }
    }

    /// <summary>0 → 32767（不限制），与 SDSoft TransferCSV 一致。</summary>
    internal static void NormalizeTimeoutsForDevice(TighteningParameterCore core)
    {
        if (core.MaxTighteningTimeTenthSec == 0)
            core.MaxTighteningTimeTenthSec = UnlimitedTimeTenthSec;
        if (core.MaxLoosenTimeTenthSec == 0)
            core.MaxLoosenTimeTenthSec = UnlimitedTimeTenthSec;
    }

    internal static void ValidateTimeoutsForDevice(TighteningParameterCore core)
    {
        if (core.MaxTighteningTimeTenthSec > UnlimitedTimeTenthSec)
        {
            throw new InvalidOperationException(
                $"最大拧紧超时（×0.1s）={core.MaxTighteningTimeTenthSec} 超过上限 {UnlimitedTimeTenthSec}，请减小后再下发。");
        }

        if (core.MaxLoosenTimeTenthSec > UnlimitedTimeTenthSec)
        {
            throw new InvalidOperationException(
                $"最大拧松超时（×0.1s）={core.MaxLoosenTimeTenthSec} 超过上限 {UnlimitedTimeTenthSec}，请减小后再下发。");
        }
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
        TwoStageModeEnabled = s.TwoStageModeEnabled,
        ErgoStopEnabled = s.ErgoStopEnabled,
    };
}
