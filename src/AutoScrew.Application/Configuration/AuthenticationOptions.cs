namespace AutoScrew.Application.Configuration;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    /// <summary>Development / MimsMySql / CompanyDatabase</summary>
    public string Mode { get; set; } = "Development";

    /// <summary>MIMS MySQL 连接失败时回退 <c>Authentication:Accounts</c> 演示账号。</summary>
    public bool FallbackToMockAccountsOnMimsFailure { get; set; }
}
