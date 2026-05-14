using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Hmi.Services;

/// <summary>
/// Development：从配置匹配账号；CompanyDatabase：占位，待你提供公司库连接与校验逻辑后实现。
/// </summary>
public sealed class AppAuthenticationService(IConfiguration configuration, ILogger<AppAuthenticationService> logger)
    : IUserAuthenticationService
{
    public Task<LoginResult> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName) || password is null)
            return Task.FromResult(LoginResult.Failed("请输入用户名和密码。"));

        var mode = configuration["Authentication:Mode"] ?? "Development";

        if (string.Equals(mode, "CompanyDatabase", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Authentication:Mode=CompanyDatabase — 尚未接入公司数据库。");
            return Task.FromResult(LoginResult.Failed(
                "公司数据库认证尚未实现。请实现 IUserAuthenticationService 并注册，或暂时将 Authentication:Mode 设为 Development。"));
        }

        var accounts = LoadAccountsFromConfiguration();
        var match = accounts.FirstOrDefault(a =>
            string.Equals(a.UserName, userName.Trim(), StringComparison.OrdinalIgnoreCase)
            && a.Password == password);

        if (match is null)
        {
            logger.LogWarning("Login failed for user {User}", userName);
            return Task.FromResult(LoginResult.Failed("用户名或密码错误。"));
        }

        if (!Enum.TryParse<UserRole>(match.Role, true, out var role))
            role = UserRole.Operator;

        var uid = string.IsNullOrWhiteSpace(match.UserId) ? userName.Trim() : match.UserId!;
        var display = string.IsNullOrWhiteSpace(match.DisplayName) ? userName.Trim() : match.DisplayName!;

        logger.LogInformation("User {User} signed in as {Role}.", userName, role);
        return Task.FromResult(LoginResult.Ok(uid, display, role));
    }

    private List<AuthAccountRow> LoadAccountsFromConfiguration()
    {
        var list = new List<AuthAccountRow>();
        foreach (var child in configuration.GetSection("Authentication:Accounts").GetChildren())
        {
            var userName = child["UserName"];
            var password = child["Password"];
            if (string.IsNullOrWhiteSpace(userName) || password is null)
                continue;

            list.Add(new AuthAccountRow(
                userName.Trim(),
                password,
                child["Role"] ?? "Operator",
                child["UserId"],
                child["DisplayName"]));
        }

        return list;
    }

    private sealed record AuthAccountRow(string UserName, string Password, string Role, string? UserId, string? DisplayName);
}
