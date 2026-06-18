using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Hmi.Services;

/// <summary>
/// Development：从配置匹配账号；CompanyDatabase：占位，待你提供公司库连接与校验逻辑后实现。
/// </summary>
public sealed class AppAuthenticationService(
    IConfiguration configuration,
    ConfigurationAccountsAuthenticationService accounts,
    ILogger<AppAuthenticationService> logger) : IUserAuthenticationService
{
    public Task<LoginResult> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var mode = configuration["Authentication:Mode"] ?? "Development";

        if (string.Equals(mode, "CompanyDatabase", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Authentication:Mode=CompanyDatabase — 尚未接入公司数据库。");
            return Task.FromResult(LoginResult.Failed(
                "公司数据库认证尚未实现。请实现 IUserAuthenticationService 并注册，或暂时将 Authentication:Mode 设为 Development。"));
        }

        return accounts.SignInAsync(userName, password, cancellationToken);
    }
}
