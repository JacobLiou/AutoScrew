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
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(499, 500)]
    public void ToDeviceParameterId_SlotPlusOne(int slot, int expected) =>
        Assert.Equal(expected, ProcessParameterCode.ToDeviceParameterId(slot));

    [Fact]
    public void ToDeviceParameterId_OutOfRange_Throws() =>
        Assert.Throws<InvalidDataException>(() => ProcessParameterCode.ToDeviceParameterId(500));

    [Theory]
    [InlineData("1830330479_00.txt", 0)]
    [InlineData("1830330479 _01.txt", 1)]
    [InlineData("00.txt", 0)]
    [InlineData(@"C:\tmp\foo_10.txt", 10)]
    public void TryParseSlotFromFileName_Succeeds(string name, int slot)
    {
        Assert.True(ProcessParameterCode.TryParseSlotFromFileName(name, out var parsed));
        Assert.Equal(slot, parsed);
    }
}
