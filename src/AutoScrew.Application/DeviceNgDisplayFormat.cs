using System.Globalization;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application;

/// <summary>作业 NG 弹窗：Delta AL/NG/WN 格式 + 设备原始码 + 中文说明。</summary>
public static class DeviceNgDisplayFormat
{
    /// <summary>如「AL1507 · 1507」或「NG4103 · 4103」。</summary>
    public static string FormatCodeLine(ushort deviceCode)
    {
        var formatted = HistoryReportParser.FormatAlarmCode(deviceCode);
        if (string.IsNullOrEmpty(formatted))
            return deviceCode.ToString(CultureInfo.InvariantCulture);

        var raw = deviceCode.ToString(CultureInfo.InvariantCulture);
        return string.Equals(formatted, raw, StringComparison.Ordinal)
            ? formatted
            : $"{formatted} · {raw}";
    }

    public static string? TryGetChineseDescription(ushort deviceCode) =>
        DeviceAlarmCodeCatalog.TryGetChineseDescription(deviceCode);

    public static string BuildDeviceMessage(ushort deviceCode, string? fallback = null)
    {
        var zh = TryGetChineseDescription(deviceCode);
        if (!string.IsNullOrWhiteSpace(zh))
            return zh;

        return string.IsNullOrWhiteSpace(fallback)
            ? $"设备判定 NG（码 {deviceCode}）"
            : fallback;
    }

    public static string BuildDeviceAdvice(ushort deviceCode)
    {
        var formatted = HistoryReportParser.FormatAlarmCode(deviceCode);
        var codeHint = string.IsNullOrEmpty(formatted)
            ? deviceCode.ToString(CultureInfo.InvariantCulture)
            : $"{formatted} · {deviceCode}";
        var zh = TryGetChineseDescription(deviceCode);
        var detail = string.IsNullOrWhiteSpace(zh) ? "" : $"（{zh}）";
        return $"控制器告警 {codeHint}{detail}。请「退出作业」挂起，在设备上清错并确认后，再扫同一 SN 恢复。";
    }
}
