using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// MIMS <c>mims_role.name</c> → AutoScrew 二元角色（操作员 / 技术员）。
/// </summary>
public static class MimsRoleMapper
{
    public static UserRole ToAutoScrewRole(string? mimsRoleName)
    {
        if (!string.IsNullOrWhiteSpace(mimsRoleName)
            && mimsRoleName.Contains("操作员", StringComparison.Ordinal))
            return UserRole.Operator;

        return UserRole.Technician;
    }
}
