namespace AutoScrew.Application.Abstractions;

public interface ICurrentUser
{
    string UserId { get; }

    string DisplayName { get; }

    UserRole Role { get; }

    bool CanAdjustParameters { get; }

    bool CanUnlockNg { get; }

    /// <summary>MIMS 用户主键（仅 MIMS 登录成功后有值）。</summary>
    int? MimsPersonId { get; }

    /// <summary>MIMS 角色主键。</summary>
    int? MimsRoleId { get; }

    /// <summary>MIMS <c>mims_role.type</c>（<c>RoleKind</c> 数值）。</summary>
    int? MimsRoleType { get; }
}

public enum UserRole
{
    Operator = 0,
    Technician = 1,
    Administrator = 2
}
