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
}
