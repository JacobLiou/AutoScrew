namespace AutoScrew.Application.Abstractions;

public enum MimsSignInFailureKind
{
    None = 0,
    InvalidInput = 1,
    NotConfigured = 2,
    ConnectionFailed = 3,
    InvalidCredentials = 4,
}

public sealed record MimsSignInOutcome(
    bool Success,
    LoginResult? Result,
    MimsSignInFailureKind FailureKind,
    string? ErrorMessage = null)
{
    public static MimsSignInOutcome Succeeded(LoginResult result) =>
        new(true, result, MimsSignInFailureKind.None);

    public static MimsSignInOutcome Failed(MimsSignInFailureKind kind, string message) =>
        new(false, LoginResult.Failed(message), kind, message);

    public bool IsConnectionFailure =>
        FailureKind is MimsSignInFailureKind.ConnectionFailed or MimsSignInFailureKind.NotConfigured;
}

public interface IMimsAuthenticationService
{
    Task<MimsSignInOutcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default);
}
