namespace AutoScrew.Application.Configuration;

public sealed class AutoScrewAppOptions
{
    public const string SectionName = "AutoScrew";

    public bool UseMockMes { get; set; }

    public bool UseSimulatedHardware { get; set; }

    public string MesBaseUrl { get; set; } = "https://localhost/";

    /// <summary>SQLite DB and local work files root.</summary>
    public string DataDirectory { get; set; } = "";

    /// <summary>Optional folder scanned for PN templates when MES returns only a file name.</summary>
    public string TemplateDirectory { get; set; } = "";

    public string? OptionalNetworkArchiveRoot { get; set; }

    public string StationId { get; set; } = "STATION-01";

    /// <summary>UI 默认语言：zh-CN 或 en-US。</summary>
    public string UiCulture { get; set; } = "zh-CN";

    /// <summary>是否启用用户操作审计（JSONL + SQLite）。</summary>
    public bool AuditLogEnabled { get; set; } = true;

    /// <summary>审计 JSONL 目录；空则使用 {DataDirectory}/audit。</summary>
    public string AuditDirectory { get; set; } = "";

    /// <summary>Running 阶段对待打 Pending 钉自动进入取钉+拧紧周期（Manual 扳机仍由电批完成）。</summary>
    public bool AutoRunScrewCycle { get; set; } = true;

    /// <summary>上一钉 OK 后是否立即自动进入下一 Pending 钉（false 时放钉后需技术员按钮或再次调度）。</summary>
    public bool AutoChainNextScrew { get; set; }

    /// <summary>作业台显示「当前螺钉：取钉+拧紧」维护按钮（技术员及以上始终可见）。</summary>
    public bool ShowManualRunScrewButton { get; set; } = true;
}
