using AutoScrew.Application.Services;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ScrewNgAdvisorTests
{
    [Theory]
    [InlineData("FLOAT_002", "浮锁")]
    [InlineData("DEVICE_NG", "退出作业")]
    [InlineData("UNKNOWN", "技术员")]
    public void GetAdvice_ReturnsNonEmpty(string code, string expectedFragment)
    {
        var advice = ScrewNgAdvisor.GetAdvice(code);
        Assert.Contains(expectedFragment, advice);
    }
}
