using System.Text.RegularExpressions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>
/// 顺序 TXT Ver01 中「吸头」「结果」无协议字段，与备注合并往返。
/// 标记格式：<c>[吸头:A][结果:预紧]</c>（有值才写出）。
/// </summary>
public static class SequenceRemarkMeta
{
    private static readonly Regex NozzleTag = new(
        @"\[吸头:(?<v>[^\]]*)\]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex ResultTag = new(
        @"\[结果:(?<v>[^\]]*)\]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string? Merge(string? remark, string? nozzle, string? result)
    {
        var body = string.IsNullOrWhiteSpace(remark) ? string.Empty : remark.Trim();
        var n = NormalizeCell(nozzle);
        var r = NormalizeCell(result);
        if (n.Length == 0 && r.Length == 0)
            return string.IsNullOrEmpty(body) ? null : body;

        var tags = string.Empty;
        if (n.Length > 0)
            tags += $"[吸头:{n}]";
        if (r.Length > 0)
            tags += $"[结果:{r}]";

        if (string.IsNullOrEmpty(body))
            return tags;
        return body + tags;
    }

    public static void TrySplit(string? remark, out string body, out string nozzle, out string result)
    {
        body = string.Empty;
        nozzle = string.Empty;
        result = string.Empty;
        if (string.IsNullOrWhiteSpace(remark))
            return;

        var text = remark.Trim();
        var n = NozzleTag.Match(text);
        if (n.Success)
        {
            nozzle = n.Groups["v"].Value.Trim();
            text = NozzleTag.Replace(text, string.Empty);
        }

        var r = ResultTag.Match(text);
        if (r.Success)
        {
            result = r.Groups["v"].Value.Trim();
            text = ResultTag.Replace(text, string.Empty);
        }

        body = text.Trim();
    }

    public static string NormalizeCell(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace('\u3000', ' ').Trim();
}
