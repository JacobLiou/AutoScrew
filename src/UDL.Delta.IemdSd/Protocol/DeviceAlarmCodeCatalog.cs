namespace UDL.Delta.IemdSd.Protocol;

/// <summary>
/// CH12 异警说明（AL/NG/WN）。说明不在 Modbus 报文中，由本地码表解析。
/// #752/#753 的 D8 按手册以 0x1nnn/0x3nnn/0x5nnn 等十六进制码存放。
/// </summary>
public static class DeviceAlarmCodeCatalog
{
    /// <summary>返回中文说明；未知码返回 null。</summary>
    public static string? TryGetChineseDescription(ushort code)
    {
        if (code == 0)
            return null;

        foreach (var key in HistoryReportParser.EnumerateCatalogKeys(code))
        {
            if (DeviceAlarmCodeCatalogData.ChineseNames.TryGetValue(key, out var zh) &&
                !string.IsNullOrWhiteSpace(zh) &&
                !LooksUntranslated(zh))
                return zh;
        }

        foreach (var key in HistoryReportParser.EnumerateCatalogKeys(code))
        {
            if (DeviceAlarmCodeCatalogData.EnglishNames.TryGetValue(key, out var en) &&
                !string.IsNullOrWhiteSpace(en))
                return en;
        }

        return null;
    }

    /// <summary>英文原名（手册）；未知返回 null。</summary>
    public static string? TryGetEnglishDescription(ushort code)
    {
        if (code == 0)
            return null;

        foreach (var key in HistoryReportParser.EnumerateCatalogKeys(code))
        {
            if (DeviceAlarmCodeCatalogData.EnglishNames.TryGetValue(key, out var en) &&
                !string.IsNullOrWhiteSpace(en))
                return en;
        }

        return null;
    }

    /// <summary>中文表里若仍残留大量英文，视为未翻译，继续尝试其它键。</summary>
    private static bool LooksUntranslated(string text)
    {
        var letters = 0;
        var asciiLetters = 0;
        foreach (var ch in text)
        {
            if (char.IsLetter(ch))
            {
                letters++;
                if (ch <= 127)
                    asciiLetters++;
            }
        }

        return letters > 0 && asciiLetters * 2 >= letters;
    }
}
