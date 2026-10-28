using System.Text.RegularExpressions;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>
/// 拧紧顺序 TXT：每个「步骤」一块键值，规则与工艺卡相同（<c>#</c> / <c>//</c> 注释，行尾 <c>&lt;说明&gt;</c> 忽略）。
/// 产出与 <see cref="SequenceExcelParser"/> 相同的 <see cref="SequenceExcelStepRow"/>。
/// </summary>
public static class SequenceTxtParser
{
    private static readonly string[] RequiredKeys = ["拧紧顺序", "拧紧参数", "数量", "批头"];

    private static readonly Regex HeaderKv = new(
        @"^(?<k>[^：:]+)[：:]\s*(?<v>.*?)\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static SequenceExcelParseResult ParseFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        return Parse(text);
    }

    public static SequenceExcelParseResult Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var rawLines = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var errors = new List<string>();
        var steps = new List<SequenceExcelStepRow>();
        Dictionary<string, string>? current = null;
        var stepLine = 0;

        for (var i = 0; i < rawLines.Length; i++)
        {
            var lineNo = i + 1;
            var line = StripComment(rawLines[i].Trim());
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith("//", StringComparison.Ordinal))
                continue;

            if (line.Equals("步骤", StringComparison.Ordinal))
            {
                Flush();
                current = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                stepLine = lineNo;
                continue;
            }

            if (current is null)
                continue;

            var kv = HeaderKv.Match(line);
            if (!kv.Success)
                continue;

            var key = NormalizeKey(kv.Groups["k"].Value);
            if (key.Length == 0)
                continue;
            current[key] = kv.Groups["v"].Value.Trim();
        }

        Flush();

        if (errors.Count == 0 && steps.Count == 0)
            errors.Add("未找到有效步骤。");

        if (errors.Count > 0)
            return new SequenceExcelParseResult([], errors);

        var ordered = steps.OrderBy(s => s.Order).ThenBy(s => s.ExcelRowNumber).ToList();
        return new SequenceExcelParseResult(ordered, []);

        void Flush()
        {
            if (current is null)
                return;

            try
            {
                steps.Add(BuildStep(current, stepLine));
            }
            catch (Exception ex)
            {
                errors.Add($"第 {stepLine} 行：{ex.Message}");
            }

            current = null;
        }
    }

    private static SequenceExcelStepRow BuildStep(IReadOnlyDictionary<string, string> map, int stepLine)
    {
        foreach (var required in RequiredKeys)
        {
            if (!map.TryGetValue(required, out var value) || string.IsNullOrWhiteSpace(value))
                throw new InvalidDataException($"缺少「{required}」。");
        }

        map.TryGetValue("位置", out var location);
        map.TryGetValue("螺钉PN", out var screwPn);
        map.TryGetValue("备注", out var remark);

        return SequenceStepRowFactory.Create(
            stepLine,
            map["拧紧顺序"],
            location,
            screwPn,
            map["拧紧参数"],
            map["数量"],
            map["批头"],
            remark);
    }

    private static string StripComment(string line)
    {
        var idx = line.IndexOf('<');
        return idx >= 0 ? line[..idx].TrimEnd() : line;
    }

    private static string NormalizeKey(string key)
    {
        key = key.Trim().Replace('\u3000', ' ');
        var paren = key.IndexOf('（');
        if (paren < 0)
            paren = key.IndexOf('(');
        if (paren > 0)
            key = key[..paren].Trim();
        return key;
    }
}
