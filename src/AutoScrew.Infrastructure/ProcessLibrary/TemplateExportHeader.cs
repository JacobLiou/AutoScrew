using System.Globalization;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>工艺卡 / 顺序 TXT 导出头部的变更责任人与时间。</summary>
public static class TemplateExportHeader
{
    public const string DefaultChangeContent = "导出";

    /// <summary>优先显示名，否则登录账号；皆空则「未知」。</summary>
    public static string ResolveAuthor(string? displayName, string? userId)
    {
        var name = displayName?.Trim() ?? string.Empty;
        if (name.Length > 0)
            return name;
        var id = userId?.Trim() ?? string.Empty;
        return id.Length > 0 ? id : "未知";
    }

    /// <summary>对齐业务模板「年/月/日/时/分」（按给定时间的本地钟面；缺省用当前本地时间）。</summary>
    public static string FormatChangeTime(DateTimeOffset? exportedAt = null)
    {
        var t = exportedAt ?? DateTimeOffset.Now;
        return t.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture);
    }
}
