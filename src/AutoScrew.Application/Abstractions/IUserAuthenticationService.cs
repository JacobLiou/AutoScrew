namespace AutoScrew.Application.Abstractions;

/// <summary>
/// 用户登录认证。公司数据库接入后，在 Infrastructure 或 Hmi 中替换实现并注册 DI。
/// </summary>
public interface IUserAuthenticationService
{
    Task<LoginResult> SignInAsync(string userName, string password, CancellationToken cancellationToken = default);
}

public sealed class LoginResult
{
    public bool Success { get; private init; }

    public string? ErrorMessage { get; private init; }

    public string UserId { get; private init; } = "";

    public string DisplayName { get; private init; } = "";

    public UserRole Role { get; private init; }

    public int? MimsPersonId { get; private init; }

    public int? MimsRoleId { get; private init; }

    public int? MimsRoleType { get; private init; }

    public static LoginResult Failed(string message) =>
        new() { Success = false, ErrorMessage = message };

    public static LoginResult Ok(
        string userId,
        string displayName,
        UserRole role,
        int? mimsPersonId = null,
        int? mimsRoleId = null,
        int? mimsRoleType = null) =>
        new()
        {
            Success = true,
            UserId = userId,
            DisplayName = displayName,
            Role = role,
            MimsPersonId = mimsPersonId,
            MimsRoleId = mimsRoleId,
            MimsRoleType = mimsRoleType
        };
}
