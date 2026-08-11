using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>工艺参数码：<c>螺钉PN-槽位</c>（如 1830331949-00），与工艺卡「参数」列一致。</summary>
public static class ProcessParameterCode
{
    private static readonly Regex AsciiAlnum = new(
        @"[^A-Za-z0-9]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static (string ScrewPn, int SlotId) Parse(string? paramRaw)
    {
        var raw = paramRaw?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidDataException("参数码为空（期望 螺钉PN-槽位，如 1830331949-00）。");

        var dash = raw.LastIndexOf('-');
        if (dash <= 0 || dash >= raw.Length - 1)
            throw new InvalidDataException($"参数码无法识别为 螺钉PN-槽位：{raw}");

        var screwPart = SanitizeAscii(raw[..dash]);
        var slotPart = raw[(dash + 1)..].Trim();
        if (string.IsNullOrEmpty(screwPart))
            throw new InvalidDataException($"参数码中螺钉 PN 无效：{raw}");
        if (!int.TryParse(slotPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var slotId))
            throw new InvalidDataException($"参数码中槽位号无效：{raw}");

        return (screwPart, slotId);
    }

    /// <summary>旧工艺卡：仅槽位号 + 单独螺钉 PN。</summary>
    public static (string ScrewPn, int SlotId) ParseLegacySlotAndScrew(string? slotRaw, string? screwPnRaw)
    {
        var paramRaw = slotRaw?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(paramRaw))
            throw new InvalidDataException("工艺卡缺少「参数：螺钉PN-槽位」或「参数：NN」。");

        if (!int.TryParse(
                paramRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var legacySlot))
            throw new InvalidDataException($"工艺卡「参数」无法识别为槽位或 螺钉PN-槽位：{paramRaw}");

        var legacyScrew = SanitizeAscii(screwPnRaw ?? string.Empty);
        if (string.IsNullOrEmpty(legacyScrew))
            throw new InvalidDataException("旧格式工艺卡缺少「参数ID」（螺钉 PN）。");

        return (legacyScrew, legacySlot);
    }

    public static string SanitizeAscii(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return AsciiAlnum.Replace(value, string.Empty);
    }
}
