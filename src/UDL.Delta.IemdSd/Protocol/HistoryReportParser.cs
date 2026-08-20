using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Protocol;

/// <summary>Parsers for controller history payloads (#750 list fields, #752–#754).</summary>
public static class HistoryReportParser
{
    public const int ErrorReportWordCount = 7;
    public const int WarningReportWordCount = 7;
    public const int ButtonReportWordCount = 12;

    public static DateTime? TryParseTimestamp(int year, int month, int day, int hour, int minute, int second)
    {
        try
        {
            if (year is < 2000 or > 2100 || month is < 1 or > 12 || day is < 1 or > 31)
                return null;
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Parse Y/M/D/h/m/s from the first six words of a history payload (0xD2–0xD7).</summary>
    public static DateTime? ParseTimestampWords(ReadOnlySpan<int> words)
    {
        if (words.Length < 6)
            return null;
        return TryParseTimestamp(words[0], words[1], words[2], words[3], words[4], words[5]);
    }

    public static ErrorReportEntry ParseError(uint reportId, int[] words)
    {
        ArgumentNullException.ThrowIfNull(words);
        return new ErrorReportEntry
        {
            ReportId = reportId,
            Timestamp = ParseTimestampWords(words),
            Code = words.Length > 6 ? (ushort)words[6] : (ushort)0,
        };
    }

    public static WarningReportEntry ParseWarning(uint reportId, int[] words)
    {
        ArgumentNullException.ThrowIfNull(words);
        return new WarningReportEntry
        {
            ReportId = reportId,
            Timestamp = ParseTimestampWords(words),
            Code = words.Length > 6 ? (ushort)words[6] : (ushort)0,
        };
    }

    public static ButtonReportEntry ParseButton(uint reportId, int[] words)
    {
        ArgumentNullException.ThrowIfNull(words);
        static uint Dw(int lo, int hi) => (uint)(hi * 65536 + (ushort)lo);

        return new ButtonReportEntry
        {
            ReportId = reportId,
            Timestamp = ParseTimestampWords(words),
            ButtonId = words.Length > 6 ? (ushort)words[6] : (ushort)0,
            ValueBefore = words.Length > 8 ? Dw(words[7], words[8]) : 0u,
            ValueAfter = words.Length > 10 ? Dw(words[9], words[10]) : 0u,
            UserId = words.Length > 11 ? (ushort)words[11] : (ushort)0,
        };
    }

    /// <summary>Format AL/NG/WN display code from numeric register value (decimal bands per CH12).</summary>
    public static string FormatAlarmCode(ushort code)
    {
        if (code == 0)
            return string.Empty;
        if (code is >= 1001 and <= 2999)
            return $"AL{code}";
        if (code is >= 3001 and <= 4999)
            return $"NG{code}";
        if (code is >= 5001 and <= 6999)
            return $"WN{code}";
        return code.ToString();
    }

    public static string FormatUserAccount(ushort userId) => userId switch
    {
        0 => string.Empty,
        6 => "Admin",
        >= 1 and <= 5 => $"User{userId}",
        _ => $"User{userId}",
    };

    public static string FormatTorqueUnit(ushort unit) => unit switch
    {
        0 => "N·m",
        1 => "kgf·cm",
        2 => "lbf·ft",
        3 => "lbf.in",
        _ => "",
    };

    internal static int At(int[] words, int hexOffset) =>
        words[hexOffset - ModbusRegisterMap.CommandData];
}
