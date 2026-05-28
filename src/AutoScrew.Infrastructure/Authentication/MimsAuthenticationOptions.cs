namespace AutoScrew.Infrastructure.Authentication;

public sealed class MimsAuthenticationOptions
{
    public const string SectionName = "Authentication:Mims";

    /// <summary>MySQL 连接串（User Secrets / 环境变量；若与 <see cref="ConnectionStringDpapiBase64"/> 同时存在则优先此项）。</summary>
    public string ConnectionString { get; set; } = "";

    /// <summary>
    /// 经应用内置对称算法加密的连接串（Base64）；新版密文可跨机器部署，旧版 DPAPI 密文也可继续读取。
    /// 在任意机器上用仓库内 <c>tools/EncryptMimsConnectionString</c> 从明文生成后粘贴到此处。
    /// </summary>
    public string ConnectionStringDpapiBase64 { get; set; } = "";

    /// <summary>仅用于兼容旧版 DPAPI 密文：<c>LocalMachine</c>（默认）或 <c>CurrentUser</c>。</summary>
    public string DpapiScope { get; set; } = "LocalMachine";

    public int CommandTimeoutSeconds { get; set; } = 15;

    /// <summary>
    /// 已废弃：登录授权改由 <see cref="MimsRoleMapper"/> 按 <c>mims_role.name</c> 是否含「操作员」映射。
    /// 保留属性以免破坏既有 appsettings / User Secrets JSON。
    /// </summary>
    [Obsolete("登录不再使用 RoleMap；见 MimsRoleMapper。")]
    public Dictionary<string, string> RoleMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>已废弃，见 <see cref="RoleMap"/>。</summary>
    [Obsolete("登录不再使用 UnmappedRoleBehavior；见 MimsRoleMapper。")]
    public string UnmappedRoleBehavior { get; set; } = "Operator";
}
