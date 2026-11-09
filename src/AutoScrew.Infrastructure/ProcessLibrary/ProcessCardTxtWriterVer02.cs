using System.Globalization;
using System.Text;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>
/// <see cref="TighteningParameterTemplate"/> → 工艺卡 ver02 表格式 TXT（扭矩显示为 lbf.in）。
/// 可被 <see cref="ProcessCardVer02Parser"/> / <see cref="ProcessCardTxtParser"/> 解析。
/// </summary>
public static class ProcessCardTxtWriterVer02
{
    private static readonly string[] SegNames = ["启动", "旋入", "预紧", "拧紧", "阶段5", "阶段6"];

    public static string Format(
        TighteningParameterTemplate template,
        string screwPn,
        int slotId,
        string? author = null,
        DateTimeOffset? exportedAt = null,
        string? changeContent = null)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(template.Core);

        var pn = ProcessParameterCode.SanitizeAscii(screwPn);
        if (string.IsNullOrEmpty(pn))
            throw new InvalidDataException("螺钉 PN 无效（需 ASCII 字母数字）。");

        _ = ProcessParameterCode.ToDeviceParameterId(slotId);

        var core = template.Core;
        var stageIndices = ResolveStageIndicesToWrite(core);
        var sb = new StringBuilder();
        var who = string.IsNullOrWhiteSpace(author) ? "未知" : author.Trim();
        var content = string.IsNullOrWhiteSpace(changeContent)
            ? TemplateExportHeader.DefaultChangeContent
            : changeContent.Trim();

        sb.AppendLine($"PN【{pn}】");
        sb.AppendLine("版本号【00】");
        sb.AppendLine($"做成【{who}】");
        sb.AppendLine($"变更内容【{content}】");
        sb.AppendLine($"变更时间：{TemplateExportHeader.FormatChangeTime(exportedAt)}");
        sb.AppendLine();
        sb.AppendLine(
            $"拧紧参数后缀【{slotId.ToString("D2", CultureInfo.InvariantCulture)}】：20=紧固程序；10=预紧程序；00=正常锁附程序");
        sb.AppendLine($"策略｜【{FormatStrategy(core.Strategy).PadRight(5)}】                       标准/加强/预定位/自创/");
        sb.AppendLine("  ID　             【     】           Param ID  序号由系统自动设定(不用填写）");

        Row(sb, "", "", "进阶设定", core.FeederResultDelayTenthSec.ToString(CultureInfo.InvariantCulture), "供料结果延时");
        Row(sb, "", "", "进阶设定", FormatMode01(core.FinalCurrentJudgeEnabled), "最终电流判定 | F2 | 1=open, 0=close");
        Row(sb, "", "", "最大总角度", core.MaxAngleDeg.ToString(CultureInfo.InvariantCulture), "Max Angle Tightening");
        Row(sb, "", "", "启动延时", core.TighteningStartDelayCentiSec.ToString(CultureInfo.InvariantCulture), "Delay Start Tightening");
        Row(sb, "", "", "最小总角度", core.MinAngleDeg.ToString(CultureInfo.InvariantCulture), "Min Angle Tightening");
        Row(sb, "", "拧松条件", "启动延时", core.LoosenStartDelayCentiSec.ToString(CultureInfo.InvariantCulture), "Delay Start Loosening");
        Row(sb, "", "拧松条件", "超时", FormatTenthAsSeconds(core.MaxTighteningTimeTenthSec), "Max Time Tightening");

        foreach (var i in stageIndices)
        {
            if (i is < 0 or > 5 || i >= core.Stages.Count)
                continue;
            WriteStage(sb, core.Stages[i], i);
        }

        Row(sb, "拧松设定", "第一阶段", "速度", core.Loosen.Stage1SpeedRpm.ToString(CultureInfo.InvariantCulture), "Target Speed");
        Row(sb, "拧松设定", "第一阶段", "角度", core.Loosen.Stage1AngleDeg.ToString(CultureInfo.InvariantCulture), "Target Angle");
        Row(sb, "拧松设定", "第二阶段", "角度", core.Loosen.Stage2AngleDeg.ToString(CultureInfo.InvariantCulture), "Target Angle");
        Row(sb, "拧松设定", "第二阶段", "速度", core.Loosen.Stage2SpeedRpm.ToString(CultureInfo.InvariantCulture), "Target Speed");
        Row(sb, "拧松设定", "生产履历", "生产履历", FormatMode01(core.Loosen.ProductionLogEnabled), "ON/OFF | 22C | 0关闭, 1开启");
        Row(sb, "拧松设定", "最小扭矩", "最小扭矩", FormatLbf(core.Loosen.DetectTorqueMilliNm), "| 22B | 直接写值");

        return sb.ToString();
    }

    public static void WriteFile(
        string filePath,
        TighteningParameterTemplate template,
        string screwPn,
        int slotId,
        string? author = null,
        DateTimeOffset? exportedAt = null,
        string? changeContent = null)
    {
        var text = Format(template, screwPn, slotId, author, exportedAt, changeContent);
        File.WriteAllText(filePath, text, Encoding.UTF8);
    }

    private static void WriteStage(StringBuilder sb, TighteningStageCore stage, int index)
    {
        var seg = SegNames[Math.Clamp(index, 0, SegNames.Length - 1)];
        const string area = "拧紧设定";

        Row(sb, area, seg, "设定模式", FormatControlModeCode(stage.ControlMode), ModeTail(index));
        Row(sb, area, seg, "速度", stage.SpeedRpm.ToString(CultureInfo.InvariantCulture), "Target Speed");

        // 目标角 / 扭矩 / 夹紧按模式写出，并保留辅助字段便于回读推断
        if (stage.TargetAngleDeg > 0 || stage.ControlMode is TighteningControlMode.Angle or TighteningControlMode.ClampAngle)
            Row(sb, area, seg, "角度", stage.TargetAngleDeg.ToString(CultureInfo.InvariantCulture), "Target Angle");

        if (stage.TargetTorqueMilliNm > 0 || stage.ControlMode is TighteningControlMode.Torque or TighteningControlMode.ClampTorque)
            Row(sb, area, seg, "扭矩", FormatLbf(stage.TargetTorqueMilliNm), "Target Torque | Clamp Torque L");

        if (stage.TargetTorqueRate > 0 || stage.ControlMode == TighteningControlMode.TorqueRate)
            Row(sb, area, seg, "扭矩率", FormatTorqueRate(stage.TargetTorqueRate), "Start torque for tightening angle calculation H");

        if (stage.MaxClampTorqueMilliNm > 0 || stage.ControlMode == TighteningControlMode.ClampTorque)
            Row(sb, area, seg, "夹紧扭矩", FormatLbf(
                stage.ControlMode == TighteningControlMode.ClampTorque
                    ? stage.TargetTorqueMilliNm
                    : stage.MaxClampTorqueMilliNm), string.Empty);

        if (stage.MaxClampAngleDeg > 0 || stage.ControlMode == TighteningControlMode.ClampAngle)
            Row(sb, area, seg, "夹紧角度",
                (stage.MaxClampAngleDeg > 0 ? stage.MaxClampAngleDeg : stage.TargetAngleDeg)
                .ToString(CultureInfo.InvariantCulture),
                string.Empty);

        Row(sb, area, seg, "进阶设定", stage.AccelTimeMs.ToString(CultureInfo.InvariantCulture), "加速度时间   Acc. time");
        Row(sb, area, seg, "进阶设定", stage.DecelTimeMs.ToString(CultureInfo.InvariantCulture), "减速度时间   Dcc. time");

        if (index == 3 || stage.Segment1TorqueMilliNm > 0 || stage.TwoStageModeEnabled)
        {
            Row(sb, area, seg, "进阶设定", FormatLbf(stage.Segment1TorqueMilliNm), "第一阶段扭矩   Torque of 1st Stage L");
            Row(sb, area, seg, "进阶设定", stage.FinalSpeedRpm.ToString(CultureInfo.InvariantCulture), "第二阶段速度   Speed of 2nd Stage");
            Row(sb, area, seg, "进阶设定", stage.Segment2AccelMs.ToString(CultureInfo.InvariantCulture), "第二阶段加速度时间   Acc. Time of 2nd Stage");
            Row(sb, area, seg, "进阶设定", stage.Segment1PauseMs.ToString(CultureInfo.InvariantCulture), "第一阶段暂定时间   Pause Time after 1st Stage");
            Row(sb, area, seg, "进阶设定", FormatMode01(stage.TwoStageModeEnabled), "两阶段模式   ON/OFF | 0x1AAbit11 | 0关闭, 1开启");
            Row(sb, area, seg, "进阶设定", FormatMode01(stage.ErgoStopEnabled), "缓停止   ON/OFF | 0x1AAbit2 | 0关闭, 1开启");
        }

        Row(sb, area, seg, "最大角度", stage.MaxAngleDeg.ToString(CultureInfo.InvariantCulture), "Max. Angle");
        Row(sb, area, seg, "最小角度", stage.MinAngleDeg.ToString(CultureInfo.InvariantCulture), "Min. Angle");
        Row(sb, area, seg, "最大扭矩", FormatLbf(stage.MaxTorqueMilliNm), "Max. Torque L");
        Row(sb, area, seg, "最小扭矩", FormatLbf(stage.MinTorqueMilliNm), "Min. Torque L");
    }

    private static string ModeTail(int index) =>
        index switch
        {
            1 => "on/off | 12C | 0=角度, 1=扭矩, 2=扭矩率",
            2 => "on/off | 15E | 1=扭矩, 2=扭矩率",
            3 => "on/off   有标记 | 190 | 0=角度, 1=扭矩, 3=夹紧扭矩, 4=夹紧角度",
            _ => "on/off",
        };

    private static void Row(
        StringBuilder sb,
        string area,
        string seg,
        string field,
        string value,
        string tail)
    {
        var cell = string.IsNullOrWhiteSpace(value) ? "     " : value.PadRight(5);
        var line = $"{Pad(area)}｜{Pad(seg)}｜{Pad(field)}｜【{cell}】";
        if (!string.IsNullOrEmpty(tail))
            line += "   " + tail;
        sb.AppendLine(line);
    }

    private static string Pad(string text) =>
        string.IsNullOrEmpty(text) ? "　　　　" : text;

    private static IReadOnlyList<int> ResolveStageIndicesToWrite(TighteningParameterCore core) =>
        core.Strategy switch
        {
            TighteningStrategy.Enhanced => [3],
            TighteningStrategy.PrePosition => [0, 1],
            TighteningStrategy.SelfDefined => ExpandSelfDefinedIndices(core),
            _ => ExpandStandardIndices(core),
        };

    private static List<int> ExpandStandardIndices(TighteningParameterCore core)
    {
        var last = 3;
        for (var i = 0; i < 4 && i < core.Stages.Count; i++)
        {
            if (StageHasContent(core.Stages[i]))
                last = i;
        }

        return Enumerable.Range(0, last + 1).ToList();
    }

    private static List<int> ExpandSelfDefinedIndices(TighteningParameterCore core)
    {
        var last = -1;
        for (var i = 0; i < 6 && i < core.Stages.Count; i++)
        {
            if (StageHasContent(core.Stages[i]))
                last = i;
        }

        if (last < 0)
            return [0];
        return Enumerable.Range(0, last + 1).ToList();
    }

    private static bool StageHasContent(TighteningStageCore stage) =>
        stage.SpeedRpm > 0
        || stage.TargetAngleDeg > 0
        || stage.TargetTorqueMilliNm > 0
        || stage.TargetTorqueRate > 0
        || stage.MaxTorqueMilliNm > 0
        || stage.MaxAngleDeg > 0
        || stage.MaxClampAngleDeg > 0
        || stage.MaxClampTorqueMilliNm > 0
        || stage.AccelTimeMs > 0
        || stage.PauseTimeMs > 0
        || stage.FinalSpeedRpm > 0
        || stage.Segment1TorqueMilliNm > 0;

    private static string FormatControlModeCode(TighteningControlMode mode) =>
        mode switch
        {
            TighteningControlMode.Angle => "0",
            TighteningControlMode.Torque => "1",
            TighteningControlMode.TorqueRate => "2",
            TighteningControlMode.ClampTorque => "3",
            TighteningControlMode.ClampAngle => "4",
            _ => "1",
        };

    private static string FormatStrategy(TighteningStrategy strategy) =>
        strategy switch
        {
            TighteningStrategy.Enhanced => "加强",
            TighteningStrategy.PrePosition => "预定位",
            TighteningStrategy.SelfDefined => "自创",
            _ => "标准",
        };

    private static string FormatMode01(bool on) => on ? "1" : "0";

    private static string FormatTenthAsSeconds(int tenthSec) =>
        (tenthSec / 10.0).ToString("0.##", CultureInfo.InvariantCulture);

    private static string FormatLbf(int milliNm) =>
        TorqueUnitConverter.MilliNmToDisplay(milliNm, DefaultTorqueUnit.LbfIn)
            .ToString("0.##", CultureInfo.InvariantCulture);

    private static string FormatTorqueRate(int targetTorqueRate) =>
        (targetTorqueRate / 1000.0).ToString("0.###", CultureInfo.InvariantCulture);
}
