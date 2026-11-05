using System.Globalization;
using System.Text.RegularExpressions;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>
/// 拧紧顺序 TXT Ver01 表格式（业务模板）→ <see cref="SequenceExcelStepRow"/>。
/// 旧「步骤」键值块已废弃。
/// </summary>
public static class SequenceTxtParser
{
    private static readonly Regex HeaderBracket = new(
        @"(?<k>产品PN|版本号|做成|变更内容)\s*[【\[](?<v>[^】\]]*)[】\]]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex ParamAndQty = new(
        @"[【\[](?<code>[^】\]]+)[】\]]\s*(?<qty>\d+)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static SequenceExcelParseResult ParseFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        return Parse(text);
    }

    public static SequenceExcelParseResult Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (LooksLikeLegacyStepBlocks(text) && !LooksLikeVer01(text))
        {
            return new SequenceExcelParseResult(
                [],
                [
                    "顺序 TXT 已改用业务 Ver01 表格式（顺序｜位置｜螺钉PN｜【拧紧参数】数量｜…）。" +
                    "请勿再使用「步骤」键值块模板。",
                ]);
        }

        var rawLines = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var errors = new List<string>();
        var steps = new List<SequenceExcelStepRow>();
        string? sourceProductPn = null;
        var seenHeader = false;

        for (var i = 0; i < rawLines.Length; i++)
        {
            var lineNo = i + 1;
            var line = rawLines[i].Trim().Replace('\u3000', ' ');
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith("//", StringComparison.Ordinal))
                continue;

            foreach (Match hm in HeaderBracket.Matches(line))
            {
                var key = hm.Groups["k"].Value.Trim();
                var value = ProcessParameterCode.SanitizeAscii(hm.Groups["v"].Value);
                if (key.Equals("产品PN", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
                    sourceProductPn = value;
            }

            if (IsTableHeader(line))
            {
                seenHeader = true;
                continue;
            }

            if (line.StartsWith("拧紧参数后缀", StringComparison.Ordinal) ||
                line.StartsWith("变更时间", StringComparison.Ordinal))
                continue;

            // 头信息行（无竖线）跳过
            if (!line.Contains('｜') && !line.Contains('|'))
                continue;

            if (!seenHeader && !LooksLikeDataRow(line))
                continue;

            try
            {
                steps.Add(ParseDataRow(line, lineNo));
            }
            catch (Exception ex)
            {
                errors.Add($"第 {lineNo} 行：{ex.Message}");
            }
        }

        if (errors.Count == 0 && steps.Count == 0)
            errors.Add("未找到有效步骤（期望 Ver01 表头后的数据行）。");

        if (errors.Count > 0)
            return new SequenceExcelParseResult([], errors, sourceProductPn);

        var ordered = steps.OrderBy(s => s.Order).ThenBy(s => s.ExcelRowNumber).ToList();
        return new SequenceExcelParseResult(ordered, [], sourceProductPn);
    }

    public static bool LooksLikeVer01(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;
        if (text.Contains("产品PN【", StringComparison.Ordinal) ||
            text.Contains("产品PN[", StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var raw in text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n'))
        {
            var line = raw.Trim();
            if (IsTableHeader(line))
                return true;
        }

        return false;
    }

    private static bool LooksLikeLegacyStepBlocks(string text)
    {
        foreach (var raw in text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n'))
        {
            var line = StripAngleComment(raw.Trim());
            if (line.Equals("步骤", StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static bool IsTableHeader(string line)
    {
        if (!(line.Contains('｜') || line.Contains('|')))
            return false;
        return line.Contains("顺序", StringComparison.Ordinal)
               && line.Contains("位置", StringComparison.Ordinal)
               && line.Contains("螺钉", StringComparison.Ordinal);
    }

    private static bool LooksLikeDataRow(string line)
    {
        if (!(line.Contains('｜') || line.Contains('|')))
            return false;
        var first = SplitPipe(line)[0].Trim();
        return int.TryParse(first, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
    }

    private static SequenceExcelStepRow ParseDataRow(string line, int lineNo)
    {
        var parts = SplitPipe(line);
        if (parts.Length < 5)
            throw new InvalidDataException("列数不足（至少需要：顺序｜位置｜螺钉PN｜【参数】数量｜批头）。");

        var orderText = parts[0].Trim();
        var location = parts[1].Trim();
        var screwPn = parts[2].Trim();

        var paramSeg = parts[3].Trim();
        var pm = ParamAndQty.Match(paramSeg);
        if (!pm.Success)
            throw new InvalidDataException($"无法解析拧紧参数与数量：{paramSeg}");

        var parameterCode = pm.Groups["code"].Value.Trim();
        var quantityText = pm.Groups["qty"].Value.Trim();
        var bitText = parts[4].Trim();

        var nozzle = parts.Length > 5 ? parts[5].Trim() : string.Empty;
        var remarkBody = parts.Length > 6 ? parts[6].Trim() : string.Empty;
        var result = parts.Length > 7 ? parts[7].Trim() : string.Empty;

        var remark = SequenceRemarkMeta.Merge(remarkBody, nozzle, result);

        return SequenceStepRowFactory.Create(
            lineNo,
            orderText,
            location,
            screwPn,
            parameterCode,
            quantityText,
            bitText,
            remark);
    }

    private static string[] SplitPipe(string line)
    {
        // 统一全角竖线
        var normalized = line.Replace('|', '｜');
        return normalized.Split('｜', StringSplitOptions.None);
    }

    private static string StripAngleComment(string line)
    {
        var idx = line.IndexOf('<');
        return idx >= 0 ? line[..idx].TrimEnd() : line;
    }
}
