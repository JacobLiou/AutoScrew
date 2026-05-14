namespace AutoScrew.Application.Abstractions;

public interface ICurrentUser
{
    string UserId { get; }

    string DisplayName { get; }

    UserRole Role { get; }

    bool CanAdjustParameters { get; }

    bool CanUnlockNg { get; }
}

public enum UserRole
{
    Operator = 0,
    Technician = 1,
    Administrator = 2
}
