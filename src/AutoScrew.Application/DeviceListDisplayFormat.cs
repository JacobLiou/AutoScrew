using System.Globalization;

namespace AutoScrew.Application;

/// <summary>设备侧参数/顺序列表与来源绑定显示：ID 不补零，有名则「ID 空格名称」。</summary>
public static class DeviceListDisplayFormat
{
    public static string Format(int id, string? name)
    {
        var trimmed = name?.Trim();
        var idText = id.ToString(CultureInfo.InvariantCulture);
        // 无名称、或名称仅为 ID 字符串（占位）时只显示 ID，避免「10 10」
        if (string.IsNullOrEmpty(trimmed) ||
            string.Equals(trimmed, idText, StringComparison.Ordinal))
            return idText;

        return idText + " " + trimmed;
    }
}
