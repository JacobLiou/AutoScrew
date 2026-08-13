using System.Globalization;
using System.Text.RegularExpressions;
using AutoScrew.Application.Abstractions;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>工艺卡 TXT → <see cref="TighteningParameterTemplate"/>（扭矩默认 lbf.in）。</summary>
public static class ProcessCardTxtParser
{
    /// <summary>允许值为空（如最终模板「参数ID：」仅注释）。</summary>
    private static readonly Regex HeaderKv = new(
        @"^(?<k>[^：:]+)[：:]\s*(?<v>.*?)\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex StageHeader = new(
        @"^(?<n>\d+)\.(?<name>启动|旋入|预紧|拧紧)\s*$",
        RegexOptions.Compiled);

    public static ProcessCardParseResult Parse(string text, string? sourceFilePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var lines = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n')
            .Select(static l => StripComment(l.Trim()))
            .Where(static l => l.Length > 0)
            .ToList();

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var stageBlocks = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<string, string>? currentStage = null;
        var inLoosen = false;
        var loosenMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var loosenSegment = 0;

        foreach (var line in lines)
        {
            var stageMatch = StageHeader.Match(line);
            if (stageMatch.Success)
            {
                inLoosen = false;
                var idx = int.Parse(stageMatch.Groups["n"].Value, CultureInfo.InvariantCulture) - 1;
                currentStage = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                stageBlocks[idx] = currentStage;
                continue;
            }

            if (line.StartsWith("拧松设定", StringComparison.Ordinal))
            {
                inLoosen = true;
                currentStage = null;
                loosenSegment = 0;
                continue;
            }

            if (inLoosen)
            {
                if (line.StartsWith("第一段", StringComparison.Ordinal))
                {
                    loosenSegment = 1;
                    continue;
                }

                if (line.StartsWith("第二段", StringComparison.Ordinal))
                {
                    loosenSegment = 2;
                    continue;
                }
            }

            if (line is "基本设定" or "拧紧条件" or "拧松条件" or "进阶设定" or "拧紧设定")
            {
                currentStage = null;
                if (!line.StartsWith("拧松", StringComparison.Ordinal))
                    inLoosen = false;
                continue;
            }

            var kv = HeaderKv.Match(line);
            if (!kv.Success)
                continue;

            var key = NormalizeKey(kv.Groups["k"].Value);
            var value = kv.Groups["v"].Value.Trim();
            // 「速度（转/分钟）：80  8」取第一个数
            if (key.Contains("速度", StringComparison.Ordinal) && value.Contains(' ', StringComparison.Ordinal))
                value = value.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

            if (currentStage is not null)
            {
                currentStage[key] = value;
                continue;
            }

            if (inLoosen)
            {
                var prefix = loosenSegment switch
                {
                    1 => "一段",
                    2 => "二段",
                    _ => "",
                };
                loosenMap[prefix + key] = value;
                if (loosenSegment == 0 || key.Contains("生产履历", StringComparison.Ordinal) ||
                    key.Contains("最小扭矩", StringComparison.Ordinal))
                    loosenMap[key] = value;
                continue;
            }

            map[key] = value;
        }

        var (screwPn, slotId) = ProcessParameterCode.ResolveIdentity(
            GetString(map, "参数"),
            sourceFilePath,
            GetString(map, "参数ID") ?? GetString(map, "参数Id"));

        TryGetInt(map, "阶段有效", out var stageCount);
        if (stageCount <= 0)
            stageCount = stageBlocks.Count > 0 ? stageBlocks.Keys.Max() + 1 : 4;
        stageCount = Math.Clamp(stageCount, 1, 6);

        var direction = ParseDirection(GetString(map, "旋转方向"));

        var template = new TighteningParameterTemplate
        {
            ParameterId = ProcessParameterCode.ToDeviceParameterId(slotId),
            ToolIndex = 0,
            Core = new TighteningParameterCore
            {
                Name = screwPn,
                Stages = TighteningParameterCore.CreateDefaultStages(),
                Loosen = new TighteningLoosenCore(),
            },
        };

        template.Core.MaxAngleDeg = GetInt(map, "最大总角度", GetInt(map, "最大角度", 0));
        template.Core.MinAngleDeg = GetInt(map, "最小总角度", GetInt(map, "最小角度", 0));
        template.Core.MaxTighteningTimeTenthSec = SecondsToTenth(GetDouble(map, "最大拧紧时间", 0));
        template.Core.TighteningStartDelayCentiSec = GetInt(map, "拧紧启动延时", 0);
        template.Core.MaxLoosenTimeTenthSec = SecondsToTenth(GetDouble(map, "最大拧松时间", 0));
        template.Core.LoosenStartDelayCentiSec = GetInt(map, "拧松启动延时", 0);
        template.Core.MaxLoosenAngleDeg = GetInt(map, "最大拧松角度", 0);

        template.Core.FinalCurrentJudgeEnabled = ParseOnOff(GetString(map, "最终电流判定"));
        template.Core.LastStageServoOn = ParseOnOff(GetString(map, "末段伺服保持"));
        template.Core.FeederResultDelayTenthSec = GetInt(map, "供料结果延时", 0);
        template.Core.LinkedCompensationParamId = GetInt(map, "关联补偿参数ID", 0);
        template.Core.SeatAngleStartTorqueRate = GetInt(map, "贴合起始扭矩率", 0);
        template.Core.ToolPrecisionCompTenthPercent = GetInt(map, "工具精度补偿", 0);
        template.Core.CurveSampleStartTorqueMilliNm = GetInt(map, "曲线取样起始扭矩", 0);
        template.Core.TorqueRateAngleDelayTenthDeg = GetInt(map, "扭矩率角度间隔", 0);
        template.Core.SeatPointAngleCorrectionTenthDeg = GetInt(map, "贴合点角度修正", 0);

        for (var i = 0; i < stageCount; i++)
        {
            if (!stageBlocks.TryGetValue(i, out var block))
                continue;
            ApplyStage(template.Core.Stages[i], i, block, direction);
        }

        ApplyLoosen(template.Core.Loosen, loosenMap, direction);
        template.ApplyCoreToRaw();

        return new ProcessCardParseResult(template, screwPn, slotId);
    }

    public static ProcessCardParseResult ParseFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        return Parse(text, filePath);
    }

    /// <summary>
    /// 身份：<c>参数：螺钉PN-槽位</c>；卡内「参数ID」仅作旧格式螺钉 PN 回退，不作为设备 ID。
    /// 缺「参数」时用文件名尾缀槽位。
    /// </summary>
    [Obsolete("Use ProcessParameterCode.ResolveIdentity")]
    private static (string ScrewPn, int SlotId) ParseParameterIdentity(IReadOnlyDictionary<string, string> map) =>
        ProcessParameterCode.ResolveIdentity(
            GetString(map, "参数"),
            filePathOrName: null,
            GetString(map, "参数ID") ?? GetString(map, "参数Id"));

    private static void ApplyStage(
        TighteningStageCore stage,
        int index,
        IReadOnlyDictionary<string, string> block,
        TighteningDirection direction)
    {
        stage.Direction = direction;
        stage.SpeedRpm = GetInt(block, "速度", 0);

        var tightenAngle = GetInt(block, "拧紧角度", 0);
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

        stage.ControlMode = ResolveControlMode(index, tightenAngle, torqueLbf, torqueRate, clampTorqueLbf, clampAngle);

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
    }

    /// <summary>
    /// 显式 OFF → false；显式 ON → true；键缺失且有正值上下限 → true。
    /// </summary>
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

    private static TighteningControlMode ResolveControlMode(
        int index,
        int tightenAngle,
        double torqueLbf,
        double torqueRate,
        double clampTorqueLbf,
        int clampAngle)
    {
        // 启动：固定角度
        if (index == 0)
            return TighteningControlMode.Angle;

        // 旋入：3 选 1
        if (index == 1)
        {
            if (torqueRate > 0)
                return TighteningControlMode.TorqueRate;
            if (tightenAngle > 0)
                return TighteningControlMode.Angle;
            return TighteningControlMode.Torque;
        }

        // 预紧：最终模板为扭矩/扭矩率；兼容旧卡夹紧扭矩
        if (index == 2)
        {
            if (torqueRate > 0)
                return TighteningControlMode.TorqueRate;
            if (clampTorqueLbf > 0)
                return TighteningControlMode.ClampTorque;
            return TighteningControlMode.Torque;
        }

        // 拧紧：4 选 1
        if (clampAngle > 0 && torqueLbf <= 0 && clampTorqueLbf <= 0 && tightenAngle <= 0)
            return TighteningControlMode.ClampAngle;
        if (clampTorqueLbf > 0 && torqueLbf <= 0)
            return TighteningControlMode.ClampTorque;
        if (torqueLbf > 0)
            return TighteningControlMode.Torque;
        if (tightenAngle > 0)
            return TighteningControlMode.Angle;
        return TighteningControlMode.Torque;
    }

    private static void ApplyLoosen(
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

        loosen.ProductionLogEnabled = ParseOnOff(GetString(map, "生产履历存档"));
        loosen.DetectTorqueMilliNm = LbfInToMilliNm(GetDouble(map, "最小扭矩", 0));
    }

    private static string StripComment(string line)
    {
        var idx = line.IndexOf('<');
        return idx >= 0 ? line[..idx].TrimEnd() : line;
    }

    private static string NormalizeKey(string key)
    {
        key = key.Trim();
        var paren = key.IndexOf('（');
        if (paren < 0)
            paren = key.IndexOf('(');
        if (paren > 0)
            key = key[..paren];
        return key.Trim();
    }

    private static string? GetString(IReadOnlyDictionary<string, string> map, string key) =>
        map.TryGetValue(key, out var v) ? v : null;

    private static bool TryGetInt(IReadOnlyDictionary<string, string> map, string key, out int value)
    {
        value = 0;
        if (!map.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return false;
        var token = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    private static int GetInt(IReadOnlyDictionary<string, string> map, string key, int fallback)
    {
        if (!map.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return fallback;
        var token = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    private static double GetDouble(IReadOnlyDictionary<string, string> map, string key, double fallback)
    {
        if (!map.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return fallback;
        var token = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    private static int SecondsToTenth(double seconds) => (int)Math.Round(seconds * 10.0);

    private static int LbfInToMilliNm(double lbfIn) =>
        TorqueUnitConverter.DisplayToMilliNm(lbfIn, DefaultTorqueUnit.LbfIn);

    private static bool ParseOnOff(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        var v = value.Trim();
        return v.Equals("ON", StringComparison.OrdinalIgnoreCase)
               || v.Equals("开", StringComparison.Ordinal)
               || v.Equals("是", StringComparison.Ordinal)
               || v.Equals("1", StringComparison.Ordinal);
    }

    private static TighteningDirection ParseDirection(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return TighteningDirection.Clockwise;
        if (value.Contains("逆", StringComparison.Ordinal) ||
            value.Contains("CCW", StringComparison.OrdinalIgnoreCase))
            return TighteningDirection.CounterClockwise;
        return TighteningDirection.Clockwise;
    }

    private static string SanitizeAscii(string value) => ProcessParameterCode.SanitizeAscii(value);
}
