using System.Globalization;
using System.Text;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary><see cref="TighteningParameterTemplate"/> → 工艺卡 TXT（扭矩显示为 lbf.in，可再被 <see cref="ProcessCardTxtParser"/> 解析）。</summary>
public static class ProcessCardTxtWriter
{
    private static readonly string[] StageTitles =
    [
        "1.启动",
        "2.旋入",
        "3.预紧",
        "4.拧紧",
        "5.阶段5",
        "6.阶段6",
    ];

    public static string Format(TighteningParameterTemplate template, string screwPn, int slotId)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(template.Core);

        var pn = ProcessParameterCode.SanitizeAscii(screwPn);
        if (string.IsNullOrEmpty(pn))
            throw new InvalidDataException("螺钉 PN 无效（需 ASCII 字母数字）。");

        _ = ProcessParameterCode.ToDeviceParameterId(slotId);

        var core = template.Core;
        var stageCount = ResolveStageCount(core);
        var direction = core.Stages.Count > 0
            ? core.Stages[0].Direction
            : TighteningDirection.Clockwise;

        var sb = new StringBuilder();
        Line(sb, "参数ID", pn, "螺钉PN");
        Line(sb, "参数", ProcessParameterCode.FormatParameterCode(pn, slotId), "设定的该螺钉参数");
        Line(sb, "阶段有效", $"{stageCount} 阶段有效");

        sb.AppendLine();
        sb.AppendLine("基本设定");
        sb.AppendLine();
        sb.AppendLine("拧紧条件");
        Line(sb, "旋转方向", FormatDirection(direction), "拧紧螺钉旋转方向");
        Line(sb, "最大总角度（°）", core.MaxAngleDeg.ToString(CultureInfo.InvariantCulture), "1~4步的总的角度值");
        Line(sb, "最小总角度（°）", core.MinAngleDeg.ToString(CultureInfo.InvariantCulture), "1~4步的总的角度值");
        Line(sb, "最大拧紧时间（秒）", FormatTenthAsSeconds(core.MaxTighteningTimeTenthSec));
        Line(sb, "拧紧启动延时（×0.01）", core.TighteningStartDelayCentiSec.ToString(CultureInfo.InvariantCulture));
        Line(sb, "末段伺服保持", FormatOnOff(core.LastStageServoOn), "是否启动未段伺服保持");
        Line(sb, "关联补偿参数ID", core.LinkedCompensationParamId.ToString(CultureInfo.InvariantCulture));

        sb.AppendLine();
        sb.AppendLine("拧松条件");
        Line(sb, "最大拧松时间（秒）", FormatTenthAsSeconds(core.MaxLoosenTimeTenthSec));
        Line(sb, "拧松启动延时（×0.01）", core.LoosenStartDelayCentiSec.ToString(CultureInfo.InvariantCulture));
        Line(sb, "最大拧松角度（°）", core.MaxLoosenAngleDeg.ToString(CultureInfo.InvariantCulture));

        sb.AppendLine();
        sb.AppendLine("进阶设定");
        Line(sb, "最终电流判定", FormatOnOff(core.FinalCurrentJudgeEnabled), "是否启动电流判定");
        Line(sb, "供料结果延时（×0.1s）", core.FeederResultDelayTenthSec.ToString(CultureInfo.InvariantCulture));
        Line(sb, "贴合起始扭矩率", core.SeatAngleStartTorqueRate.ToString(CultureInfo.InvariantCulture));
        Line(sb, "工具精度补偿（×0.1%）", core.ToolPrecisionCompTenthPercent.ToString(CultureInfo.InvariantCulture));
        Line(sb, "曲线取样起始扭矩（mNm）", core.CurveSampleStartTorqueMilliNm.ToString(CultureInfo.InvariantCulture));
        Line(sb, "扭矩率角度间隔（×0.1°）", core.TorqueRateAngleDelayTenthDeg.ToString(CultureInfo.InvariantCulture));
        Line(sb, "贴合点角度修正(×0.1°)", core.SeatPointAngleCorrectionTenthDeg.ToString(CultureInfo.InvariantCulture));

        sb.AppendLine();
        sb.AppendLine("拧紧设定");
        sb.AppendLine();

        for (var i = 0; i < stageCount; i++)
        {
            var stage = core.Stages[i];
            sb.AppendLine(StageTitles[i]);
            WriteStage(sb, stage, i);
            sb.AppendLine();
        }

        sb.AppendLine("拧松设定");
        sb.AppendLine("第一段");
        Line(sb, "角度", core.Loosen.Stage1AngleDeg.ToString(CultureInfo.InvariantCulture));
        Line(sb, "速度（转/分钟）", core.Loosen.Stage1SpeedRpm.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine("第二段");
        Line(sb, "速度（转/分钟）", core.Loosen.Stage2SpeedRpm.ToString(CultureInfo.InvariantCulture));
        Line(sb, "角度（°）", core.Loosen.Stage2AngleDeg.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine();
        Line(sb, "生产履历存档", FormatOnOff(core.Loosen.ProductionLogEnabled), "off情况下，设定有效，拧松但不做存档");
        Line(sb, "最小扭矩（lbf.in）", FormatLbf(core.Loosen.DetectTorqueMilliNm));

        return sb.ToString();
    }

    public static void WriteFile(string filePath, TighteningParameterTemplate template, string screwPn, int slotId)
    {
        var text = Format(template, screwPn, slotId);
        File.WriteAllText(filePath, text, Encoding.UTF8);
    }

    private static void WriteStage(StringBuilder sb, TighteningStageCore stage, int index)
    {
        switch (stage.ControlMode)
        {
            case TighteningControlMode.Angle:
                Line(sb, "拧紧角度（°）", stage.TargetAngleDeg.ToString(CultureInfo.InvariantCulture));
                break;
            case TighteningControlMode.Torque:
                Line(sb, "扭矩（lbf.in）", FormatLbf(stage.TargetTorqueMilliNm));
                if (stage.TargetAngleDeg > 0)
                    Line(sb, "拧紧角度（°）", stage.TargetAngleDeg.ToString(CultureInfo.InvariantCulture));
                break;
            case TighteningControlMode.TorqueRate:
                Line(sb, "扭矩率（lbf.in/°）", FormatTorqueRate(stage.TargetTorqueRate));
                if (stage.TargetAngleDeg > 0)
                    Line(sb, "拧紧角度（°）", stage.TargetAngleDeg.ToString(CultureInfo.InvariantCulture));
                break;
            case TighteningControlMode.ClampTorque:
                Line(sb, "夹紧扭矩（lbf.in）", FormatLbf(stage.TargetTorqueMilliNm));
                if (stage.MaxClampTorqueMilliNm > 0 && stage.MaxClampTorqueMilliNm != stage.TargetTorqueMilliNm)
                    Line(sb, "扭矩（lbf.in）", FormatLbf(stage.TargetTorqueMilliNm));
                break;
            case TighteningControlMode.ClampAngle:
                Line(sb, "夹紧角度", stage.MaxClampAngleDeg > 0
                    ? stage.MaxClampAngleDeg.ToString(CultureInfo.InvariantCulture)
                    : stage.TargetAngleDeg.ToString(CultureInfo.InvariantCulture));
                break;
            default:
                if (stage.TargetAngleDeg > 0)
                    Line(sb, "拧紧角度（°）", stage.TargetAngleDeg.ToString(CultureInfo.InvariantCulture));
                if (stage.TargetTorqueMilliNm > 0)
                    Line(sb, "扭矩（lbf.in）", FormatLbf(stage.TargetTorqueMilliNm));
                break;
        }

        // 非主控字段但样例卡常有的夹紧值
        if (stage.ControlMode != TighteningControlMode.ClampTorque && stage.MaxClampTorqueMilliNm > 0)
            Line(sb, "夹紧扭矩（lbf.in）", FormatLbf(stage.MaxClampTorqueMilliNm));
        if (stage.ControlMode != TighteningControlMode.ClampAngle && stage.MaxClampAngleDeg > 0)
            Line(sb, "夹紧角度", stage.MaxClampAngleDeg.ToString(CultureInfo.InvariantCulture));

        Line(sb, "速度（转/分钟）", stage.SpeedRpm.ToString(CultureInfo.InvariantCulture));

        var torqueJudge = stage.MaxTorqueMilliNm > 0 || stage.MinTorqueMilliNm > 0;
        Line(sb, "扭矩判断", FormatOnOff(torqueJudge));
        Line(sb, "最大扭矩（lbf.in）", FormatLbf(stage.MaxTorqueMilliNm));
        Line(sb, "最小扭矩（lbf.in）", FormatLbf(stage.MinTorqueMilliNm));

        // 启动段样例通常只有扭矩判断；旋入及以后写角度判断
        if (index > 0)
        {
            var angleJudge = stage.MaxAngleDeg > 0 || stage.MinAngleDeg > 0;
            Line(sb, "角度判断", FormatOnOff(angleJudge));
            Line(sb, "最大角度（°）", stage.MaxAngleDeg.ToString(CultureInfo.InvariantCulture));
            Line(sb, "最小角度（°）", stage.MinAngleDeg.ToString(CultureInfo.InvariantCulture));
        }
    }

    private static int ResolveStageCount(TighteningParameterCore core)
    {
        var last = -1;
        for (var i = 0; i < core.Stages.Count && i < 6; i++)
        {
            if (StageHasContent(core.Stages[i]))
                last = i;
        }

        return last < 0 ? 4 : Math.Max(last + 1, 1);
    }

    private static bool StageHasContent(TighteningStageCore stage) =>
        stage.SpeedRpm > 0
        || stage.TargetAngleDeg > 0
        || stage.TargetTorqueMilliNm > 0
        || stage.TargetTorqueRate > 0
        || stage.MaxTorqueMilliNm > 0
        || stage.MaxAngleDeg > 0
        || stage.MaxClampAngleDeg > 0
        || stage.MaxClampTorqueMilliNm > 0;

    private static void Line(StringBuilder sb, string key, string value, string? comment = null)
    {
        if (string.IsNullOrEmpty(comment))
            sb.AppendLine($"{key}：{value}");
        else
            sb.AppendLine($"{key}：{value}                 <{comment}>");
    }

    private static string FormatDirection(TighteningDirection direction) =>
        direction == TighteningDirection.CounterClockwise ? "逆时针" : "顺时针";

    private static string FormatOnOff(bool on) => on ? "ON" : "OFF";

    private static string FormatTenthAsSeconds(int tenthSec) =>
        (tenthSec / 10.0).ToString("0.##", CultureInfo.InvariantCulture);

    private static string FormatLbf(int milliNm) =>
        TorqueUnitConverter.MilliNmToDisplay(milliNm, DefaultTorqueUnit.LbfIn)
            .ToString("0.##", CultureInfo.InvariantCulture);

    private static string FormatTorqueRate(int targetTorqueRate) =>
        (targetTorqueRate / 1000.0).ToString("0.###", CultureInfo.InvariantCulture);
}
