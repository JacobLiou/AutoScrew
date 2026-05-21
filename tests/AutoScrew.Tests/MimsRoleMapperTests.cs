using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Authentication;
using Xunit;

namespace AutoScrew.Tests;

public sealed class MimsRoleMapperTests
{
    [Theory]
    [InlineData("操作员", UserRole.Operator)]
    [InlineData("七分厂操作员", UserRole.Operator)]
    [InlineData("单步权限操作员", UserRole.Operator)]
    [InlineData("技术员", UserRole.Technician)]
    [InlineData("Super Admin", UserRole.Technician)]
    [InlineData("生产管理员", UserRole.Technician)]
    [InlineData("工程师", UserRole.Technician)]
    [InlineData("产线工程师", UserRole.Technician)]
    public void ToAutoScrewRole_maps_by_role_name(string? mimsRoleName, UserRole expected) =>
        Assert.Equal(expected, MimsRoleMapper.ToAutoScrewRole(mimsRoleName));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToAutoScrewRole_empty_name_defaults_to_technician(string? mimsRoleName) =>
        Assert.Equal(UserRole.Technician, MimsRoleMapper.ToAutoScrewRole(mimsRoleName));
}
