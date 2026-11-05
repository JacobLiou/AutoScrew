using System.Globalization;
using System.Text.RegularExpressions;
using AutoScrew.Application.Abstractions;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>工艺卡 TXT → <see cref="TighteningParameterTemplate"/>（扭矩默认 lbf.in）。自动识别旧键值格式与 ver02 表格式。</summary>
public static class ProcessCardTxtParser
{
    /// <summary>允许值为空（如最终模板「参数ID：」仅注释）。</summary>
    private static readonly Regex HeaderKv = new(
        @"^(?<k>[^：:]+)[：:]\s*(?<v>.*?)\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>兼容命名四阶段与自创 <c>N.阶段N</c>。</summary>
    private static readonly Regex StageHeader = new(
        @"^(?<n>\d+)\.(?<name>启动|旋入|预紧|拧紧|阶段\d+)\s*$",
        RegexOptions.Compiled);

    public static ProcessCardParseResult Parse(string text, string? sourceFilePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (ProcessCardVer02Parser.LooksLikeVer02(text))
            return ProcessCardVer02Parser.Parse(text, sourceFilePath);

        return ParseLegacyKv(text, sourceFilePath);
    }

    public static ProcessCardParseResult ParseFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        return Parse(text, filePath);
    }

    private static ProcessCardParseResult ParseLegacyKv(string text, string? sourceFilePath)
    {
        var lines = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n')
            .Select(static l => StripComment(l.Trim()))
            .Where(static l => l.Length > 0 && !l.StartsWith('#') && !l.StartsWith("//", StringComparison.Ordinal))
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
                if (idx is < 0 or > 5)
                    continue;
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
            ProcessCardSemantics.GetString(map, "参数"),
            sourceFilePath,
            ProcessCardSemantics.GetString(map, "参数ID") ?? ProcessCardSemantics.GetString(map, "参数Id"));

        ProcessCardSemantics.TryGetInt(map, "阶段有效", out var stageCount);
        if (stageCount <= 0)
            stageCount = stageBlocks.Count > 0 ? stageBlocks.Keys.Max() + 1 : 4;
        stageCount = Math.Clamp(stageCount, 1, 6);

        var direction = ProcessCardSemantics.ParseDirection(ProcessCardSemantics.GetString(map, "旋转方向"));
        var strategy = ProcessCardSemantics.ParseStrategy(ProcessCardSemantics.GetString(map, "策略"));

        var template = new TighteningParameterTemplate
        {
            ParameterId = ProcessParameterCode.ToDeviceParameterId(slotId),
            ToolIndex = 0,
            Core = new TighteningParameterCore
            {
                Name = screwPn,
                Strategy = strategy,
                Stages = TighteningParameterCore.CreateDefaultStages(),
                Loosen = new TighteningLoosenCore(),
            },
        };

        template.Core.MaxAngleDeg = ProcessCardSemantics.GetInt(map, "最大总角度", ProcessCardSemantics.GetInt(map, "最大角度", 0));
        template.Core.MinAngleDeg = ProcessCardSemantics.GetInt(map, "最小总角度", ProcessCardSemantics.GetInt(map, "最小角度", 0));
        template.Core.MaxTighteningTimeTenthSec =
            ProcessCardSemantics.SecondsToTenth(ProcessCardSemantics.GetDouble(map, "最大拧紧时间", 0));
        template.Core.TighteningStartDelayCentiSec = ProcessCardSemantics.GetInt(map, "拧紧启动延时", 0);
        template.Core.MaxLoosenTimeTenthSec =
            ProcessCardSemantics.SecondsToTenth(ProcessCardSemantics.GetDouble(map, "最大拧松时间", 0));
        template.Core.LoosenStartDelayCentiSec = ProcessCardSemantics.GetInt(map, "拧松启动延时", 0);
        template.Core.MaxLoosenAngleDeg = ProcessCardSemantics.GetInt(map, "最大拧松角度", 0);

        template.Core.FinalCurrentJudgeEnabled =
            ProcessCardSemantics.ParseOnOff(ProcessCardSemantics.GetString(map, "最终电流判定"));
        template.Core.LastStageServoOn =
            ProcessCardSemantics.ParseOnOff(ProcessCardSemantics.GetString(map, "末段伺服保持"));
        template.Core.FeederResultDelayTenthSec = ProcessCardSemantics.GetInt(map, "供料结果延时", 0);
        template.Core.LinkedCompensationParamId = ProcessCardSemantics.GetInt(map, "关联补偿参数ID", 0);
        template.Core.SeatAngleStartTorqueRate = ProcessCardSemantics.GetInt(map, "贴合起始扭矩率", 0);
        template.Core.ToolPrecisionCompTenthPercent = ProcessCardSemantics.GetInt(map, "工具精度补偿", 0);
        template.Core.CurveSampleStartTorqueMilliNm = ProcessCardSemantics.GetInt(map, "曲线取样起始扭矩", 0);
        template.Core.TorqueRateAngleDelayTenthDeg = ProcessCardSemantics.GetInt(map, "扭矩率角度间隔", 0);
        template.Core.SeatPointAngleCorrectionTenthDeg = ProcessCardSemantics.GetInt(map, "贴合点角度修正", 0);

        foreach (var (idx, block) in stageBlocks)
        {
            if (idx is < 0 or > 5)
                continue;
            ProcessCardSemantics.ApplyStage(template.Core.Stages[idx], idx, block, direction);
        }

        if (strategy == TighteningStrategy.SelfDefined && stageCount < 6)
        {
            for (var i = stageCount; i < 6; i++)
            {
                if (!stageBlocks.ContainsKey(i))
                    template.Core.Stages[i] = new TighteningStageCore();
            }
        }

        ProcessCardSemantics.ApplyLoosen(template.Core.Loosen, loosenMap, direction);
        template.ApplyCoreToRaw();

        return new ProcessCardParseResult(template, screwPn, slotId);
    }

    /// <summary>兼容旧测试入口。</summary>
    public static TighteningStrategy ParseStrategy(string? value) =>
        ProcessCardSemantics.ParseStrategy(value);

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
}
