using AutoScrew.Application;
using Xunit;

namespace AutoScrew.Tests;

public sealed class DeviceNgDisplayFormatTests
{
    [Theory]
    [InlineData(1507, "AL1507 · 1507")]
    [InlineData(4103, "AL1007 · 4103")]
    [InlineData(5081, "WN5081 · 5081")]
    public void FormatCodeLine_UsesDeltaBands(ushort code, string expected) =>
        Assert.Equal(expected, DeviceNgDisplayFormat.FormatCodeLine(code));

    [Fact]
    public void TryGetChineseDescription_KnownAlarm_ReturnsText()
    {
        var text = DeviceNgDisplayFormat.TryGetChineseDescription(4103);
        Assert.Equal("速度控制误差过大", text);
    }

    [Fact]
    public void BuildDeviceAdvice_IncludesFormattedCode()
    {
        var advice = DeviceNgDisplayFormat.BuildDeviceAdvice(4103);
        Assert.Contains("AL1007", advice);
        Assert.Contains("4103", advice);
    }
}
