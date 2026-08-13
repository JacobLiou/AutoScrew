using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>工艺参数码：<c>螺钉PN-槽位</c>（如 1830331949-00），与工艺卡「参数」列一致。</summary>
public static class ProcessParameterCode
{
    public const int MinDeviceParameterId = 1;
    public const int MaxDeviceParameterId = 500;

    private static readonly Regex AsciiAlnum = new(
        @"[^A-Za-z0-9]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>文件名尾缀槽位：<c>..._00.txt</c> 或 <c>00.txt</c>。</summary>
    private static readonly Regex SlotFromFileName = new(
        @"(?:^|_)(\d{1,3})\.txt$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

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

    /// <summary>本地槽位序号 → 控制器 ParameterId（00→1，01→2）。</summary>
    public static int ToDeviceParameterId(int slotIndex)
    {
        var id = checked(slotIndex + 1);
        if (id is < MinDeviceParameterId or > MaxDeviceParameterId)
        {
            throw new InvalidDataException(
                $"槽位 {slotIndex:D2} 对应设备参数 ID {id} 超出允许范围 {MinDeviceParameterId}–{MaxDeviceParameterId}。");
        }

        return id;
    }

    public static bool TryParseSlotFromFileName(string? filePathOrName, out int slotIndex)
    {
        slotIndex = 0;
        if (string.IsNullOrWhiteSpace(filePathOrName))
            return false;

        var name = Path.GetFileName(filePathOrName.Trim());
        var m = SlotFromFileName.Match(name);
        if (!m.Success)
            return false;

        return int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out slotIndex);
    }

    /// <summary>
    /// 从「参数：」行解析；若为空则用文件名槽位，螺钉 PN 取自文件名前缀（去尾缀 _NN）。
    /// </summary>
    public static (string ScrewPn, int SlotId) ResolveIdentity(
        string? paramRaw,
        string? filePathOrName,
        string? legacyScrewPnFromParamIdLine = null)
    {
        var raw = paramRaw?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(raw))
        {
            var dash = raw.LastIndexOf('-');
            if (dash > 0 && dash < raw.Length - 1)
            {
                try
                {
                    return Parse(raw);
                }
                catch (InvalidDataException)
                {
                    // fall through to legacy / filename
                }
            }

            try
            {
                return ParseLegacySlotAndScrew(raw, legacyScrewPnFromParamIdLine);
            }
            catch (InvalidDataException) when (TryParseSlotFromFileName(filePathOrName, out _))
            {
                // fall through to filename
            }
        }

        if (!TryParseSlotFromFileName(filePathOrName, out var slotFromFile))
            throw new InvalidDataException("工艺卡缺少「参数：螺钉PN-槽位」，且无法从文件名解析槽位（期望 *_NN.txt）。");

        var fileName = Path.GetFileNameWithoutExtension(filePathOrName ?? string.Empty);
        var screwFromFile = ExtractScrewPnFromFileName(fileName, slotFromFile);
        if (string.IsNullOrEmpty(screwFromFile))
            throw new InvalidDataException($"无法从文件名解析螺钉 PN：{filePathOrName}");

        return (screwFromFile, slotFromFile);
    }

    private static string ExtractScrewPnFromFileName(string fileNameWithoutExt, int slotIndex)
    {
        var suffix = "_" + slotIndex.ToString("D2", CultureInfo.InvariantCulture);
        var suffixAlt = "_" + slotIndex.ToString(CultureInfo.InvariantCulture);
        var name = fileNameWithoutExt.Trim();
        if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            name = name[..^suffix.Length];
        else if (name.EndsWith(suffixAlt, StringComparison.OrdinalIgnoreCase))
            name = name[..^suffixAlt.Length];

        return SanitizeAscii(name.Trim());
    }

    public static string SanitizeAscii(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return AsciiAlnum.Replace(value, string.Empty);
    }
}
