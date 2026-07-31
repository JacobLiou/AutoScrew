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
    public string TemplateDirectory { get; set; } = "Templates";

    public string? OptionalNetworkArchiveRoot { get; set; }

    /// <summary>
    /// 局域网服务账号口令的 aes256 密文（见 tools/EncryptMimsConnectionString）。
    /// 运行时解密用于 WNet；界面与 mes-settings 不保存明文。
    /// </summary>
    public string? LanSharePasswordAes256 { get; set; }

    /// <summary>可选域；空则仅用固定服务账号名连接 UNC。</summary>
    public string? LanShareDomain { get; set; }

    public string StationId { get; set; } = "STATION-01";

    /// <summary>UI 默认语言：zh-CN 或 en-US。</summary>
    public string UiCulture { get; set; } = "zh-CN";

    /// <summary>是否启用用户操作审计（JSONL + SQLite）。</summary>
    public bool AuditLogEnabled { get; set; } = true;

    /// <summary>审计 JSONL 目录；空则使用 {DataDirectory}/audit。</summary>
    public string AuditDirectory { get; set; } = "";

    /// <summary>作业活动日志 UI/内存缓冲上限（超出移除最旧项）。</summary>
    public int OperationActivityLogMaxInMemory { get; set; } = 200;

    /// <summary>作业活动日志 JSONL 目录；空则使用 {DataDirectory}/activity。</summary>
    public string OperationActivityDirectory { get; set; } = "";

    /// <summary>Running 阶段对待打 Pending 钉自动进入取钉+拧紧周期（Manual 扳机仍由电批完成）。</summary>
    public bool AutoRunScrewCycle { get; set; } = true;

    /// <summary>上一钉 OK 后是否立即自动进入下一 Pending 钉（false 时放钉后需技术员按钮或再次调度）。</summary>
    public bool AutoChainNextScrew { get; set; }

    /// <summary>作业台显示「当前螺钉：取钉+拧紧」维护按钮（技术员及以上始终可见）。</summary>
    public bool ShowManualRunScrewButton { get; set; } = true;

    /// <summary>SN 校验成功后写入 IEMD-SD 控制器条码 (#401)。</summary>
    public bool WriteSnToController { get; set; } = true;

    /// <summary>写条码失败时阻断配方加载；false 时仅记录警告。</summary>
    public bool StrictSnToController { get; set; }

    /// <summary>Mock MES 使用本地 local-recipes.json（见 doc/LOCAL_RECIPES.md）。</summary>
    public bool UseLocalRecipes { get; set; } = true;

    /// <summary>产线拧紧控制：HostGuided=模板引导+#302；DeviceProgram=来源绑定+#303。</summary>
    public ProductionTighteningMode TighteningControlMode { get; set; } = ProductionTighteningMode.HostGuided;
}
