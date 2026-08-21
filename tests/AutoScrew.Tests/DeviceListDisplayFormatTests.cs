using AutoScrew.Application;
using Xunit;

namespace AutoScrew.Tests;

public sealed class DeviceListDisplayFormatTests
{
    [Theory]
    [InlineData(1, "CANSHU", "1 CANSHU")]
    [InlineData(12, "  YUDINGWEI  ", "12 YUDINGWEI")]
    [InlineData(3, null, "3")]
    [InlineData(3, "", "3")]
    [InlineData(3, "   ", "3")]
    public void Format_IdAndName(int id, string? name, string expected) =>
        Assert.Equal(expected, DeviceListDisplayFormat.Format(id, name));
}
