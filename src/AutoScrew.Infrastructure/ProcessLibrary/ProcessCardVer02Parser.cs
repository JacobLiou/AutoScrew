using System.Globalization;
using System.Text.RegularExpressions;
using AutoScrew.Application.Abstractions;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>
/// 工艺卡 ver02 表格式（区｜段｜字段｜【值】）→ <see cref="TighteningParameterTemplate"/>。
/// 扭矩语义与旧卡一致（lbf.in）。
/// </summary>
public static class ProcessCardVer02Parser
{
    private static readonly Regex HeaderBracket = new(
        @"(?<k>PN|版本号|拧紧参数后缀|做成|变更内容|变更时间|ID)\s*[【\[](?<v>[^】\]]*)[】\]]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex DataRow = new(
        @"^(?<area>[^｜|]*)[｜|](?<seg>[^｜|]*)[｜|](?<field>[^｜|]*)[｜|][【\[](?<val>[^】\]]*)[】\]](?<tail>.*)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>1109 模板：策略｜【标准/加强/预定位/自创】（非四段 DataRow）。</summary>
    private static readonly Regex StrategyLine = new(
        @"^策略\s*[｜|]\s*[【\[](?<v>[^】\]]*)[】\]]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool LooksLikeVer02(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;
        if (text.Contains("PN【", StringComparison.Ordinal) || text.Contains("PN[", StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var raw in text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;
            if ((line.Contains('｜') || line.Contains('|')) &&
                (line.Contains('【') || line.Contains('[')) &&
                DataRow.IsMatch(line))
                return true;
        }

        return false;
    }

    public static ProcessCardParseResult Parse(string text, string? sourceFilePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var global = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var stageBlocks = new Dictionary<int, Dictionary<string, string>>();
        var loosenMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var raw in text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith("//", StringComparison.Ordinal))
                continue;

            // 说明行：拧紧参数后缀【】：20=…
            foreach (Match hm in HeaderBracket.Matches(line))
            {
                var hk = hm.Groups["k"].Value.Trim();
                var hv = NormalizeValue(hm.Groups["v"].Value);
                if (!headers.ContainsKey(hk) || !string.IsNullOrEmpty(hv))
                    headers[hk] = hv;
            }

            var sm = StrategyLine.Match(line);
            if (sm.Success)
            {
                SetIfMeaningful(global, "策略", NormalizeValue(sm.Groups["v"].Value));
                continue;
            }

            var dm = DataRow.Match(line);
            if (!dm.Success)
                continue;

            var area = NormalizeToken(dm.Groups["area"].Value);
            var seg = NormalizeToken(dm.Groups["seg"].Value);
            var field = NormalizeToken(dm.Groups["field"].Value);
            var value = NormalizeValue(dm.Groups["val"].Value);
            var tail = dm.Groups["tail"].Value;

            if (area.StartsWith("拧松", StringComparison.Ordinal))
            {
                ApplyLoosenRow(loosenMap, seg, field, value, tail);
                continue;
            }

            var stageIdx = MapStageIndex(area, seg);
            if (stageIdx is >= 0 and <= 5)
            {
                if (!stageBlocks.TryGetValue(stageIdx, out var block))
                {
                    block = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    stageBlocks[stageIdx] = block;
                }

                ApplyStageRow(block, field, value, tail);
                continue;
            }

            ApplyGlobalRow(global, area, seg, field, value, tail);
        }

        var screwPn = ProcessParameterCode.SanitizeAscii(headers.GetValueOrDefault("PN") ?? string.Empty);
        if (string.IsNullOrEmpty(screwPn))
            throw new InvalidDataException("ver02 工艺卡缺少 PN【…】。");

        var slotId = ResolveSlotId(headers.GetValueOrDefault("拧紧参数后缀"), sourceFilePath);

        var direction = ProcessCardSemantics.ParseDirection(ProcessCardSemantics.GetString(global, "旋转方向"));
        var strategyText = ProcessCardSemantics.GetString(global, "策略");
        var strategy = string.IsNullOrWhiteSpace(strategyText)
            ? TighteningStrategy.Standard
            : ProcessCardSemantics.ParseStrategy(strategyText);

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

        template.Core.MaxAngleDeg = ProcessCardSemantics.GetInt(global, "最大总角度", 0);
        template.Core.MinAngleDeg = ProcessCardSemantics.GetInt(global, "最小总角度", 0);
        template.Core.MaxTighteningTimeTenthSec =
            ProcessCardSemantics.SecondsToTenth(ProcessCardSemantics.GetDouble(global, "最大拧紧时间", 0));
        template.Core.TighteningStartDelayCentiSec =
            ProcessCardSemantics.GetInt(global, "拧紧启动延时", 0);
        template.Core.MaxLoosenTimeTenthSec =
            ProcessCardSemantics.SecondsToTenth(ProcessCardSemantics.GetDouble(global, "最大拧松时间", 0));
        template.Core.LoosenStartDelayCentiSec =
            ProcessCardSemantics.GetInt(global, "拧松启动延时", 0);
        template.Core.MaxLoosenAngleDeg = ProcessCardSemantics.GetInt(global, "最大拧松角度", 0);
        template.Core.FinalCurrentJudgeEnabled =
            ProcessCardSemantics.ParseOnOff(ProcessCardSemantics.GetString(global, "最终电流判定"));
        template.Core.FeederResultDelayTenthSec =
            ProcessCardSemantics.GetInt(global, "供料结果延时", 0);

        foreach (var (idx, block) in stageBlocks)
            ProcessCardSemantics.ApplyStage(template.Core.Stages[idx], idx, block, direction);

        if (string.IsNullOrWhiteSpace(strategyText))
            template.Core.Strategy = TighteningStrategyHelper.InferFromCore(template.Core);

        ProcessCardSemantics.ApplyLoosen(template.Core.Loosen, loosenMap, direction);
        template.ApplyCoreToRaw();

        return new ProcessCardParseResult(template, screwPn, slotId);
    }

    private static int ResolveSlotId(string? suffixRaw, string? sourceFilePath)
    {
        if (!string.IsNullOrWhiteSpace(suffixRaw))
        {
            var token = suffixRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var slot))
                return slot;
            throw new InvalidDataException($"ver02「拧紧参数后缀」无法识别为槽位号：{suffixRaw}");
        }

        if (ProcessParameterCode.TryParseSlotFromFileName(sourceFilePath, out var fromFile))
            return fromFile;

        throw new InvalidDataException(
            "ver02 工艺卡缺少「拧紧参数后缀【00|10|20】」，且无法从文件名解析槽位（期望 *_NN.txt）。");
    }

    private static int MapStageIndex(string area, string seg)
    {
        if (!area.StartsWith("拧紧设定", StringComparison.Ordinal) &&
            !string.Equals(area, "拧紧", StringComparison.Ordinal))
            return -1;

        if (seg.Contains("阶段5", StringComparison.Ordinal) || seg.Contains("阶段 5", StringComparison.Ordinal))
            return 4;
        if (seg.Contains("阶段6", StringComparison.Ordinal) || seg.Contains("阶段 6", StringComparison.Ordinal))
            return 5;
        if (seg.Contains("启动", StringComparison.Ordinal))
            return 0;
        if (seg.Contains("旋入", StringComparison.Ordinal))
            return 1;
        if (seg.Contains("预紧", StringComparison.Ordinal))
            return 2;
        if (seg.Contains("拧紧", StringComparison.Ordinal))
            return 3;
        return -1;
    }

    private static void ApplyGlobalRow(
        Dictionary<string, string> global,
        string area,
        string seg,
        string field,
        string value,
        string tail)
    {
        // 模板瑕疵：拧松条件｜超时｜英文 Max Time Tightening → 最大拧紧时间
        if (field.Contains("超时", StringComparison.Ordinal) &&
            (ContainsIgnore(tail, "Max Time Tightening") || seg.Contains("超时", StringComparison.Ordinal)))
        {
            SetIfMeaningful(global, "最大拧紧时间", value);
            return;
        }

        if (field.Contains("供料结果延时", StringComparison.Ordinal) || ContainsIgnore(tail, "供料结果延时"))
        {
            SetIfMeaningful(global, "供料结果延时", value);
            return;
        }

        if (field.Contains("最终电流判定", StringComparison.Ordinal) || ContainsIgnore(tail, "最终电流判定"))
        {
            SetIfMeaningful(global, "最终电流判定", value);
            return;
        }

        if (field.Contains("最大总角度", StringComparison.Ordinal) || ContainsIgnore(tail, "Max Angle Tightening"))
        {
            SetIfMeaningful(global, "最大总角度", value);
            return;
        }

        if (field.Contains("最小总角度", StringComparison.Ordinal) || ContainsIgnore(tail, "Min Angle Tightening"))
        {
            SetIfMeaningful(global, "最小总角度", value);
            return;
        }

        if (field.Contains("启动延时", StringComparison.Ordinal))
        {
            if (area.Contains("拧松", StringComparison.Ordinal) || seg.Contains("拧松", StringComparison.Ordinal) ||
                ContainsIgnore(tail, "Loosen"))
                SetIfMeaningful(global, "拧松启动延时", value);
            else
                SetIfMeaningful(global, "拧紧启动延时", value);
            return;
        }

        if (field.Contains("最大拧紧时间", StringComparison.Ordinal))
            SetIfMeaningful(global, "最大拧紧时间", value);
        else if (field.Contains("最大拧松时间", StringComparison.Ordinal))
            SetIfMeaningful(global, "最大拧松时间", value);
        else if (field.Contains("最大拧松角度", StringComparison.Ordinal))
            SetIfMeaningful(global, "最大拧松角度", value);
        else if (field.Contains("旋转方向", StringComparison.Ordinal))
            SetIfMeaningful(global, "旋转方向", value);
        else if (field.Contains("策略", StringComparison.Ordinal))
            SetIfMeaningful(global, "策略", value);
    }

    private static void ApplyStageRow(
        Dictionary<string, string> block,
        string field,
        string value,
        string tail)
    {
        if (field.Contains("设定模式", StringComparison.Ordinal) || field.Contains("控制模式", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "控制模式", value);
            return;
        }

        if (field.Contains("进阶设定", StringComparison.Ordinal))
        {
            ApplyAdvancedAlias(block, value, tail);
            return;
        }

        if (field.Contains("夹紧扭矩", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "夹紧扭矩", value);
            return;
        }

        if (field.Contains("夹紧角度", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "夹紧角度", value);
            return;
        }

        if (field.Contains("最大扭矩", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "最大扭矩", value);
            return;
        }

        if (field.Contains("最小扭矩", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "最小扭矩", value);
            return;
        }

        if (field.Contains("最大角度", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "最大角度", value);
            return;
        }

        if (field.Contains("最小角度", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "最小角度", value);
            return;
        }

        if (field.Contains("扭矩率", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "扭矩率", value);
            return;
        }

        if (field.Contains("扭矩", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "扭矩", value);
            return;
        }

        if (field.Contains("速度", StringComparison.Ordinal))
        {
            SetIfMeaningful(block, "速度", value);
            return;
        }

        if (field.Contains("角度", StringComparison.Ordinal))
        {
            // 拧紧段第二行角度 0 不覆盖已解析目标角
            if (block.ContainsKey("拧紧角度") && string.IsNullOrEmpty(value))
                return;
            if (block.ContainsKey("拧紧角度") && value is "0")
                return;
            SetIfMeaningful(block, "拧紧角度", value);
        }
    }

    private static void ApplyAdvancedAlias(Dictionary<string, string> block, string value, string tail)
    {
        if (ContainsIgnore(tail, "Acc. Time of 2nd") || ContainsIgnore(tail, "第二阶段加速度"))
        {
            SetIfMeaningful(block, "二段加速", value);
            return;
        }

        if (ContainsIgnore(tail, "Acc. time") || ContainsIgnore(tail, "加速度时间"))
        {
            SetIfMeaningful(block, "加速时间", value);
            return;
        }

        if (ContainsIgnore(tail, "Dcc. time") || ContainsIgnore(tail, "减速度时间"))
        {
            SetIfMeaningful(block, "减速时间", value);
            return;
        }

        if (ContainsIgnore(tail, "Speed of 2nd") || ContainsIgnore(tail, "第二阶段速度"))
        {
            SetIfMeaningful(block, "最终速度", value);
            return;
        }

        if (ContainsIgnore(tail, "Pause Time") || ContainsIgnore(tail, "第一阶段暂定") || ContainsIgnore(tail, "暂定时间"))
        {
            SetIfMeaningful(block, "一段暂停", value);
            return;
        }

        if (ContainsIgnore(tail, "Torque of 1st") || ContainsIgnore(tail, "第一阶段扭矩"))
        {
            // 取首次非空
            if (!block.ContainsKey("一段扭矩") || string.IsNullOrWhiteSpace(block["一段扭矩"]))
                SetIfMeaningful(block, "一段扭矩", value);
            return;
        }

        if (ContainsIgnore(tail, "两阶段模式") || ContainsIgnore(tail, "0x1AA"))
        {
            SetIfMeaningful(block, "两阶段模式", value);
            return;
        }

        if (ContainsIgnore(tail, "缓停止") || ContainsIgnore(tail, "bit2"))
        {
            SetIfMeaningful(block, "缓停止", value);
        }
    }

    private static void ApplyLoosenRow(
        Dictionary<string, string> loosenMap,
        string seg,
        string field,
        string value,
        string tail)
    {
        var prefix = seg.Contains("第一", StringComparison.Ordinal) ? "一段"
            : seg.Contains("第二", StringComparison.Ordinal) ? "二段"
            : "";

        if (field.Contains("生产履历", StringComparison.Ordinal) || seg.Contains("生产履历", StringComparison.Ordinal))
        {
            SetIfMeaningful(loosenMap, "生产履历存档", value);
            return;
        }

        if (field.Contains("最小扭矩", StringComparison.Ordinal) || seg.Contains("最小扭矩", StringComparison.Ordinal))
        {
            SetIfMeaningful(loosenMap, "最小扭矩", value);
            return;
        }

        // 以中文字段为准（模板二段英中文对调）
        if (field.Contains("角度", StringComparison.Ordinal))
        {
            SetIfMeaningful(loosenMap, prefix + "角度", value);
            return;
        }

        if (field.Contains("速度", StringComparison.Ordinal))
        {
            SetIfMeaningful(loosenMap, prefix + "速度", value);
            return;
        }

        if (ContainsIgnore(tail, "Target Speed") && string.IsNullOrEmpty(prefix) == false)
        {
            // 仅当中文未命中时兜底
            SetIfMeaningful(loosenMap, prefix + "速度", value);
        }
    }

    private static void SetIfMeaningful(Dictionary<string, string> map, string key, string value)
    {
        if (string.IsNullOrEmpty(value) && map.ContainsKey(key))
            return;
        map[key] = value;
    }

    private static string NormalizeToken(string raw) =>
        raw.Replace('\u3000', ' ').Trim();

    private static string NormalizeValue(string raw) =>
        raw.Replace('\u3000', ' ').Trim();

    private static bool ContainsIgnore(string haystack, string needle) =>
        haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
}
