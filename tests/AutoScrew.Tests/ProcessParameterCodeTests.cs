using AutoScrew.Infrastructure.ProcessLibrary;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProcessParameterCodeTests
{
    [Theory]
    [InlineData("1830331949-00", "1830331949", 0)]
    [InlineData("1830331949-01", "1830331949", 1)]
    [InlineData("1830330402-10", "1830330402", 10)]
    public void Parse_PnDashSlot_Succeeds(string code, string screwPn, int slotId)
    {
        var (pn, slot) = ProcessParameterCode.Parse(code);
        Assert.Equal(screwPn, pn);
        Assert.Equal(slotId, slot);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1830331949")]
    [InlineData("-01")]
    [InlineData("abc-xx")]
    public void Parse_Invalid_Throws(string code) =>
        Assert.Throws<InvalidDataException>(() => ProcessParameterCode.Parse(code));
}
