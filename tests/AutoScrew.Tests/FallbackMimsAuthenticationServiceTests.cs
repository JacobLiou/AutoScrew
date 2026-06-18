using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class FallbackMimsAuthenticationServiceTests
{
    [Fact]
    public async Task SignInAsync_connection_failed_with_fallback_uses_mock_account()
    {
        var service = CreateService(
            fallbackEnabled: true,
            mimsOutcome: MimsSignInOutcome.Failed(
                MimsSignInFailureKind.ConnectionFailed,
                "无法连接用户数据库，请稍后重试或联系管理员。"));

        var result = await service.SignInAsync("operator", "demo");

        Assert.True(result.Success);
        Assert.True(result.UsedMockAccountFallback);
        Assert.Equal(UserRole.Operator, result.Role);
    }

    [Fact]
    public async Task SignInAsync_invalid_credentials_does_not_fallback()
    {
        var service = CreateService(
            fallbackEnabled: true,
            mimsOutcome: MimsSignInOutcome.Failed(
                MimsSignInFailureKind.InvalidCredentials,
                "用户名或密码错误。"));

        var result = await service.SignInAsync("operator", "demo");

        Assert.False(result.Success);
        Assert.Equal("用户名或密码错误。", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInAsync_connection_failed_without_fallback_returns_mims_error()
    {
        const string message = "无法连接用户数据库，请稍后重试或联系管理员。";
        var service = CreateService(
            fallbackEnabled: false,
            mimsOutcome: MimsSignInOutcome.Failed(MimsSignInFailureKind.ConnectionFailed, message));

        var result = await service.SignInAsync("operator", "demo");

        Assert.False(result.Success);
        Assert.Equal(message, result.ErrorMessage);
    }

    [Fact]
    public async Task SignInAsync_connection_failed_with_fallback_and_wrong_mock_password_fails()
    {
        var service = CreateService(
            fallbackEnabled: true,
            mimsOutcome: MimsSignInOutcome.Failed(
                MimsSignInFailureKind.ConnectionFailed,
                "无法连接用户数据库，请稍后重试或联系管理员。"));

        var result = await service.SignInAsync("operator", "wrong");

        Assert.False(result.Success);
        Assert.Equal("用户名或密码错误。", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInAsync_mims_success_returns_without_fallback()
    {
        var service = CreateService(
            fallbackEnabled: true,
            mimsOutcome: MimsSignInOutcome.Succeeded(
                LoginResult.Ok("mims-user", "MIMS User", UserRole.Technician, 1, 2, 3)));

        var result = await service.SignInAsync("mims-user", "secret");

        Assert.True(result.Success);
        Assert.False(result.UsedMockAccountFallback);
        Assert.Equal("mims-user", result.UserId);
        Assert.Equal(1, result.MimsPersonId);
    }

    private static FallbackMimsAuthenticationService CreateService(
        bool fallbackEnabled,
        MimsSignInOutcome mimsOutcome)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Accounts:0:UserName"] = "operator",
                ["Authentication:Accounts:0:Password"] = "demo",
                ["Authentication:Accounts:0:Role"] = "Operator",
                ["Authentication:Accounts:0:UserId"] = "operator",
                ["Authentication:Accounts:0:DisplayName"] = "操作员",
            })
            .Build();

        var accounts = new ConfigurationAccountsAuthenticationService(
            configuration,
            NullLogger<ConfigurationAccountsAuthenticationService>.Instance);

        return new FallbackMimsAuthenticationService(
            new StubMimsAuthenticationService(mimsOutcome),
            accounts,
            Options.Create(new AuthenticationOptions { FallbackToMockAccountsOnMimsFailure = fallbackEnabled }),
            NullLogger<FallbackMimsAuthenticationService>.Instance);
    }

    private sealed class StubMimsAuthenticationService(MimsSignInOutcome outcome) : IMimsAuthenticationService
    {
        public Task<MimsSignInOutcome> SignInAsync(
            string userName,
            string password,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(outcome);
    }
}
