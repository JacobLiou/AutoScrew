using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure;

public sealed class SessionCurrentUser : ICurrentUser
{
    public string UserId { get; private set; } = "operator";

    public string DisplayName { get; private set; } = "Operator";

    public UserRole Role { get; private set; } = UserRole.Operator;

    public bool CanAdjustParameters => Role >= UserRole.Technician;

    public bool CanUnlockNg => Role >= UserRole.Technician;

    public void SetRole(UserRole role, string userId, string displayName)
    {
        Role = role;
        UserId = userId;
        DisplayName = displayName;
    }
}
