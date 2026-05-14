namespace AutoScrew.Infrastructure.Authentication;

public sealed class MimsAuthenticationOptions
{
    public const string SectionName = "Authentication:Mims";

    /// <summary>MySQL 连接串（仅 User Secrets / 环境变量，勿写入受版本控制的 appsettings）。</summary>
    public string ConnectionString { get; set; } = "";

    public int CommandTimeoutSeconds { get; set; } = 15;

    /// <summary>mims_role.type（RoleKind 数值）→ AutoScrew UserRole 名称。</summary>
    public Dictionary<string, string> RoleMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>未在 <see cref="RoleMap"/> 中配置的 role.type：Operator | Deny。</summary>
    public string UnmappedRoleBehavior { get; set; } = "Operator";
}
