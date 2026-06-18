using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// 从 <c>Authentication:Accounts</c> 读取本地演示/回退账号。
/// </summary>
public sealed class ConfigurationAccountsAuthenticationService(
    IConfiguration configuration,
    ILogger<ConfigurationAccountsAuthenticationService> logger) : IUserAuthenticationService
{
    public Task<LoginResult> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName) || password is null)
            return Task.FromResult(LoginResult.Failed("请输入用户名和密码。"));

        var accounts = ConfigurationAuthAccounts.Load(configuration);
        var match = accounts.FirstOrDefault(a =>
            string.Equals(a.UserName, userName.Trim(), StringComparison.OrdinalIgnoreCase)
            && a.Password == password);

        if (match is null)
        {
            logger.LogWarning("Configuration account login failed for user {User}", userName);
            return Task.FromResult(LoginResult.Failed("用户名或密码错误。"));
        }

        if (!Enum.TryParse<UserRole>(match.Role, true, out var role))
            role = UserRole.Operator;

        var uid = string.IsNullOrWhiteSpace(match.UserId) ? userName.Trim() : match.UserId!;
        var display = string.IsNullOrWhiteSpace(match.DisplayName) ? userName.Trim() : match.DisplayName!;

        logger.LogInformation("Configuration account user {User} signed in as {Role}.", userName, role);
        return Task.FromResult(LoginResult.Ok(uid, display, role));
    }
}

internal static class ConfigurationAuthAccounts
{
    internal static IReadOnlyList<AuthAccountRow> Load(IConfiguration configuration)
    {
        var list = new List<AuthAccountRow>();
        foreach (var child in configuration.GetSection($"{AuthenticationOptions.SectionName}:Accounts").GetChildren())
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

    internal sealed record AuthAccountRow(string UserName, string Password, string Role, string? UserId, string? DisplayName);
}
