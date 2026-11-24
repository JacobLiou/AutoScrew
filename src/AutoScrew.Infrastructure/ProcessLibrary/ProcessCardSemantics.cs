using System.Globalization;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>工艺卡键值块 → Core 字段（扭矩按 lbf.in）。新旧格式解析器共用。</summary>
internal static class ProcessCardSemantics
{
    public static void ApplyStage(
        TighteningStageCore stage,
        int index,
        IReadOnlyDictionary<string, string> block,
        TighteningDirection direction)
    {
        // ver02「设定模式｜【off】」：关段，不推断为扭矩@0（数字 0 仍表示角度）。
        if (IsExplicitStageOff(GetString(block, "控制模式")))
        {
            ResetStage(stage);
            return;
        }

        stage.Direction = direction;
        stage.SpeedRpm = GetInt(block, "速度", 0);

        var tightenAngle = GetInt(block, "拧紧角度", GetInt(block, "角度", 0));
        var torqueLbf = GetDouble(block, "扭矩", 0);
        var torqueRate = GetDouble(block, "扭矩率", 0);
        var clampTorqueLbf = GetDouble(block, "夹紧扭矩", 0);
        var clampAngle = GetInt(block, "夹紧角度", 0);

        var maxTorqueLbf = GetDouble(block, "最大扭矩", 0);
        var minTorqueLbf = GetDouble(block, "最小扭矩", 0);
        var maxAngle = GetInt(block, "最大角度", 0);
        var minAngle = GetInt(block, "最小角度", 0);

        var torqueJudge = ResolveJudgeEnabled(block, "扭矩判断", maxTorqueLbf > 0 || minTorqueLbf > 0);
        var angleJudge = ResolveJudgeEnabled(block, "角度判断", maxAngle > 0 || minAngle > 0);

        var explicitMode = TryParseControlMode(GetString(block, "控制模式"));
        stage.ControlMode = explicitMode
            ?? ResolveControlMode(index, tightenAngle, torqueLbf, torqueRate, clampTorqueLbf, clampAngle);

        switch (stage.ControlMode)
        {
            case TighteningControlMode.Angle:
                stage.TargetAngleDeg = tightenAngle;
                stage.TargetTorqueMilliNm = 0;
                break;
            case TighteningControlMode.Torque:
                stage.TargetTorqueMilliNm = LbfInToMilliNm(torqueLbf);
                stage.TargetAngleDeg = 0;
                break;
            case TighteningControlMode.TorqueRate:
                stage.TargetTorqueRate = (int)Math.Round(torqueRate * 1000.0);
                stage.TargetAngleDeg = tightenAngle;
                stage.TargetTorqueMilliNm = 0;
                break;
            case TighteningControlMode.ClampTorque:
                stage.TargetTorqueMilliNm = LbfInToMilliNm(clampTorqueLbf > 0 ? clampTorqueLbf : torqueLbf);
                stage.TargetAngleDeg = 0;
                break;
            case TighteningControlMode.ClampAngle:
                stage.TargetAngleDeg = clampAngle > 0 ? clampAngle : tightenAngle;
                stage.MaxClampAngleDeg = clampAngle;
                stage.TargetTorqueMilliNm = 0;
                break;
        }

        if (clampTorqueLbf > 0 && stage.ControlMode != TighteningControlMode.ClampTorque)
            stage.MaxClampTorqueMilliNm = LbfInToMilliNm(clampTorqueLbf);
        if (clampAngle > 0 && stage.ControlMode != TighteningControlMode.ClampAngle)
            stage.MaxClampAngleDeg = clampAngle;

        if (torqueJudge)
        {
            stage.MaxTorqueMilliNm = LbfInToMilliNm(maxTorqueLbf);
            stage.MinTorqueMilliNm = LbfInToMilliNm(minTorqueLbf);
        }
        else
        {
            stage.MaxTorqueMilliNm = 0;
            stage.MinTorqueMilliNm = 0;
        }

        if (angleJudge)
        {
            stage.MaxAngleDeg = maxAngle;
            stage.MinAngleDeg = minAngle;
        }
        else
        {
            stage.MaxAngleDeg = 0;
            stage.MinAngleDeg = 0;
        }

        ApplyStageAdvanced(stage, block);
    }

    public static void ApplyLoosen(
        TighteningLoosenCore loosen,
        IReadOnlyDictionary<string, string> map,
        TighteningDirection tightenDirection)
    {
        loosen.Direction = tightenDirection == TighteningDirection.Clockwise
            ? TighteningDirection.CounterClockwise
            : TighteningDirection.Clockwise;

        loosen.Stage1AngleDeg = GetInt(map, "一段角度", GetInt(map, "角度", 0));
        loosen.Stage1SpeedRpm = GetInt(map, "一段速度", 0);
        loosen.Stage2AngleDeg = GetInt(map, "二段角度", 0);
        loosen.Stage2SpeedRpm = GetInt(map, "二段速度", 0);

        if (loosen.Stage1SpeedRpm == 0)
            loosen.Stage1SpeedRpm = GetInt(map, "速度", 0);

        loosen.Stage1AccelMs = GetInt(map, "一段加速时间", GetInt(map, "一段加速", GetInt(map, "第一段加速", 0)));
        loosen.Stage2AccelMs = GetInt(map, "二段加速时间", GetInt(map, "二段加速", GetInt(map, "第二段加速", 0)));

        loosen.ProductionLogEnabled = ParseOnOff(GetString(map, "生产履历存档") ?? GetString(map, "生产履历"));
        loosen.DetectTorqueMilliNm = LbfInToMilliNm(GetDouble(map, "最小扭矩", 0));
    }

    public static TighteningDirection ParseDirection(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return TighteningDirection.Clockwise;
        if (value.Contains("逆", StringComparison.Ordinal) ||
            value.Contains("CCW", StringComparison.OrdinalIgnoreCase))
            return TighteningDirection.CounterClockwise;
        return TighteningDirection.Clockwise;
    }

    public static TighteningStrategy ParseStrategy(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return TighteningStrategy.Standard;
        var v = value.Trim();
        if (v.Contains("加强", StringComparison.Ordinal) ||
            v.Equals("Enhanced", StringComparison.OrdinalIgnoreCase))
            return TighteningStrategy.Enhanced;
        if (v.Contains("预定位", StringComparison.Ordinal) ||
            v.Contains("PrePosition", StringComparison.OrdinalIgnoreCase) ||
            v.Contains("Pre-position", StringComparison.OrdinalIgnoreCase))
            return TighteningStrategy.PrePosition;
        if (v.Contains("自创", StringComparison.Ordinal) ||
            v.Contains("Self", StringComparison.OrdinalIgnoreCase))
            return TighteningStrategy.SelfDefined;
        return TighteningStrategy.Standard;
    }

    public static bool ParseOnOff(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        var v = value.Trim();
        return v.Equals("ON", StringComparison.OrdinalIgnoreCase)
               || v.Equals("开", StringComparison.Ordinal)
               || v.Equals("是", StringComparison.Ordinal)
               || v.Equals("1", StringComparison.Ordinal);
    }

    public static string? GetString(IReadOnlyDictionary<string, string> map, string key) =>
        map.TryGetValue(key, out var v) ? v : null;

    public static bool TryGetInt(IReadOnlyDictionary<string, string> map, string key, out int value)
    {
        value = 0;
        if (!map.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return false;
        var token = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    public static int GetInt(IReadOnlyDictionary<string, string> map, string key, int fallback)
    {
        if (!map.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return fallback;
        var token = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    public static double GetDouble(IReadOnlyDictionary<string, string> map, string key, double fallback)
    {
        if (!map.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return fallback;
        var token = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    public static int SecondsToTenth(double seconds) => (int)Math.Round(seconds * 10.0);

    private static void ApplyStageAdvanced(TighteningStageCore stage, IReadOnlyDictionary<string, string> block)
    {
        if (block.ContainsKey("扭矩率角度间隔"))
            stage.TorqueRateAngleIntervalTenthDeg = GetInt(block, "扭矩率角度间隔", 0);

        stage.AccelTimeMs = GetInt(block, "加速时间", 0);
        stage.DecelTimeMs = GetInt(block, "减速时间", 0);
        stage.MaxRunTimeCentiSec = GetInt(block, "最大运行时间", 0);
        stage.MinRunTimeCentiSec = GetInt(block, "最小运行时间", 0);

        if (block.ContainsKey("补偿扭矩"))
            stage.CompTorqueEnabled = ParseOnOff(GetString(block, "补偿扭矩"));
        stage.CompTorqueAnglePercent = GetInt(block, "补偿扭矩角度百分比", 0);
        stage.PauseTimeMs = GetInt(block, "暂停时间", GetInt(block, "持续时间", 0));

        var minClampTorqueLbf = GetDouble(block, "最小夹紧扭矩", 0);
        if (minClampTorqueLbf > 0 || block.ContainsKey("最小夹紧扭矩"))
            stage.MinClampTorqueMilliNm = LbfInToMilliNm(minClampTorqueLbf);
        stage.MinClampAngleDeg = GetInt(block, "最小夹紧角度", 0);

        var seg1Lbf = GetDouble(block, "一段扭矩", 0);
        if (seg1Lbf > 0 || block.ContainsKey("一段扭矩"))
            stage.Segment1TorqueMilliNm = LbfInToMilliNm(seg1Lbf);
        stage.Segment1PauseMs = GetInt(block, "一段暂停", 0);
        stage.Segment2AccelMs = GetInt(block, "二段加速", 0);
        stage.FinalSpeedRpm = GetInt(block, "最终速度", 0);

        if (block.ContainsKey("两阶段模式"))
            stage.TwoStageModeEnabled = ParseOnOff(GetString(block, "两阶段模式"));
        else
            stage.TwoStageModeEnabled =
                stage.Segment1TorqueMilliNm > 0
                || stage.Segment1PauseMs > 0
                || stage.Segment2AccelMs > 0
                || stage.FinalSpeedRpm > 0;

        if (block.ContainsKey("缓停止"))
            stage.ErgoStopEnabled = ParseOnOff(GetString(block, "缓停止"));
    }

    private static bool ResolveJudgeEnabled(
        IReadOnlyDictionary<string, string> block,
        string key,
        bool hasPositiveLimits)
    {
        if (!block.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return hasPositiveLimits;

        if (IsExplicitOff(raw))
            return false;

        return ParseOnOff(raw) || hasPositiveLimits;
    }

    private static bool IsExplicitOff(string value)
    {
        var v = value.Trim();
        return v.Equals("OFF", StringComparison.OrdinalIgnoreCase)
               || v.Equals("关", StringComparison.Ordinal)
               || v.Equals("否", StringComparison.Ordinal)
               || v.Equals("0", StringComparison.Ordinal);
    }

    /// <summary>ver02 设定模式显式关段（off/关）；与数字 0（角度）区分。</summary>
    public static bool IsExplicitStageOff(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        var v = value.Trim();
        return v.Equals("off", StringComparison.OrdinalIgnoreCase)
               || v.Equals("关", StringComparison.Ordinal)
               || v.Equals("关闭", StringComparison.Ordinal)
               || v.Equals("否", StringComparison.Ordinal);
    }

    /// <summary>支持中文名与 ver02 数字枚举（0 角度 / 1 扭矩 / 2 扭矩率 / 3 夹紧扭矩 / 4 夹紧角度）。</summary>
    public static TighteningControlMode? TryParseControlMode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || IsExplicitStageOff(value))
            return null;
        var v = value.Trim();
        if (int.TryParse(v.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
        {
            return code switch
            {
                0 => TighteningControlMode.Angle,
                1 => TighteningControlMode.Torque,
                2 => TighteningControlMode.TorqueRate,
                3 => TighteningControlMode.ClampTorque,
                4 => TighteningControlMode.ClampAngle,
                _ => null,
            };
        }

        if (v.Contains("扭矩率", StringComparison.Ordinal))
            return TighteningControlMode.TorqueRate;
        if (v.Contains("夹紧扭矩", StringComparison.Ordinal))
            return TighteningControlMode.ClampTorque;
        if (v.Contains("夹紧角度", StringComparison.Ordinal))
            return TighteningControlMode.ClampAngle;
        if (v.Contains("扭矩", StringComparison.Ordinal))
            return TighteningControlMode.Torque;
        if (v.Contains("角度", StringComparison.Ordinal))
            return TighteningControlMode.Angle;
        return null;
    }

    private static TighteningControlMode ResolveControlMode(
        int index,
        int tightenAngle,
        double torqueLbf,
        double torqueRate,
        double clampTorqueLbf,
        int clampAngle)
    {
        if (index == 0)
            return TighteningControlMode.Angle;

        if (index == 1)
        {
            if (torqueRate > 0)
                return TighteningControlMode.TorqueRate;
            if (tightenAngle > 0)
                return TighteningControlMode.Angle;
            return TighteningControlMode.Torque;
        }

        if (index == 2)
        {
            if (torqueRate > 0)
                return TighteningControlMode.TorqueRate;
            if (clampTorqueLbf > 0)
                return TighteningControlMode.ClampTorque;
            return TighteningControlMode.Torque;
        }

        if (clampAngle > 0 && torqueLbf <= 0 && clampTorqueLbf <= 0 && tightenAngle <= 0)
            return TighteningControlMode.ClampAngle;
        if (clampTorqueLbf > 0 && torqueLbf <= 0)
            return TighteningControlMode.ClampTorque;
        if (torqueRate > 0)
            return TighteningControlMode.TorqueRate;
        if (torqueLbf > 0)
            return TighteningControlMode.Torque;
        if (tightenAngle > 0)
            return TighteningControlMode.Angle;
        return TighteningControlMode.Torque;
    }

    private static void ResetStage(TighteningStageCore stage)
    {
        stage.ControlMode = TighteningControlMode.Angle;
        stage.Direction = TighteningDirection.Clockwise;
        stage.SpeedRpm = 0;
        stage.TargetTorqueMilliNm = 0;
        stage.TargetAngleDeg = 0;
        stage.TargetTorqueRate = 0;
        stage.TorqueRateAngleIntervalTenthDeg = 0;
        stage.AccelTimeMs = 0;
        stage.MaxAngleDeg = 0;
        stage.MinAngleDeg = 0;
        stage.MaxTorqueMilliNm = 0;
        stage.MinTorqueMilliNm = 0;
        stage.MaxRunTimeCentiSec = 0;
        stage.MinRunTimeCentiSec = 0;
        stage.CompTorqueEnabled = false;
        stage.CompTorqueAnglePercent = 0;
        stage.PauseTimeMs = 0;
        stage.MaxClampTorqueMilliNm = 0;
        stage.MinClampTorqueMilliNm = 0;
        stage.MaxClampAngleDeg = 0;
        stage.MinClampAngleDeg = 0;
        stage.Segment1TorqueMilliNm = 0;
        stage.Segment1PauseMs = 0;
        stage.Segment2AccelMs = 0;
        stage.FinalSpeedRpm = 0;
        stage.DecelTimeMs = 0;
        stage.TwoStageModeEnabled = false;
        stage.ErgoStopEnabled = false;
    }

    private static int LbfInToMilliNm(double lbfIn) =>
        TorqueUnitConverter.DisplayToMilliNm(lbfIn, DefaultTorqueUnit.LbfIn);
}
