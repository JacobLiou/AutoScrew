using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// 优先 MIMS MySQL；仅连接失败且启用回退时使用 <c>Authentication:Accounts</c>。
/// </summary>
public sealed class FallbackMimsAuthenticationService(
    IMimsAuthenticationService mims,
    ConfigurationAccountsAuthenticationService accounts,
    IOptions<AuthenticationOptions> authOptions,
    ILogger<FallbackMimsAuthenticationService> logger) : IUserAuthenticationService
{
    public async Task<LoginResult> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var outcome = await mims.SignInAsync(userName, password, cancellationToken).ConfigureAwait(false);
        if (outcome.Success && outcome.Result is not null)
            return outcome.Result;

        if (!authOptions.Value.FallbackToMockAccountsOnMimsFailure || !outcome.IsConnectionFailure)
            return outcome.Result ?? LoginResult.Failed(outcome.ErrorMessage ?? "登录失败。");

        logger.LogWarning(
            "MIMS unreachable ({FailureKind}), trying Authentication:Accounts fallback.",
            outcome.FailureKind);

        var mock = await accounts.SignInAsync(userName, password, cancellationToken).ConfigureAwait(false);
        if (!mock.Success)
            return mock;

        logger.LogWarning(
            "User {User} signed in via mock account fallback (MIMS unavailable).",
            userName);

        return mock.WithMockAccountFallback();
    }
}
