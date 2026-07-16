using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class TorqueUnitConverterTests
{
    [Fact]
    public void ToNm_SystemUnit_IsIdentityAt1001()
    {
        var nm = TorqueUnitConverter.ToNm(1500, 1001);
        Assert.Equal(1.5, nm, 3);
    }

    [Fact]
    public void MilliNmToKgfCm_RoundTrips()
    {
        const int milliNm = 450;
        var kgfCm = TorqueUnitConverter.MilliNmToKgfCm(milliNm);
        Assert.Equal(milliNm, TorqueUnitConverter.KgfCmToMilliNm(kgfCm));
        Assert.Equal(4.59, kgfCm);
    }

    [Theory]
    [InlineData(DefaultTorqueUnit.NewtonMeter, 620, 0.62)]
    [InlineData(DefaultTorqueUnit.KgfCm, 70, 0.71)]
    [InlineData(DefaultTorqueUnit.LbfIn, 70, 0.62)]
    [InlineData(DefaultTorqueUnit.LbfFt, 1000, 0.74)]
    public void MilliNmToDisplay_MatchesDemoCoefficients(DefaultTorqueUnit unit, int milliNm, double expected)
    {
        Assert.Equal(expected, TorqueUnitConverter.MilliNmToDisplay(milliNm, unit));
    }

    [Theory]
    [InlineData(DefaultTorqueUnit.NewtonMeter)]
    [InlineData(DefaultTorqueUnit.KgfCm)]
    [InlineData(DefaultTorqueUnit.LbfFt)]
    [InlineData(DefaultTorqueUnit.LbfIn)]
    public void Display_RoundTrips_PreservesRegisterWhenDisplayUnchanged(DefaultTorqueUnit unit)
    {
        const int milliNm = 282;
        var display = TorqueUnitConverter.MilliNmToDisplay(milliNm, unit);
        Assert.Equal(milliNm, TorqueUnitConverter.DisplayToMilliNm(display, unit, milliNm));
    }

    [Fact]
    public void DisplayToMilliNm_WithoutCurrent_UsesNearestMilliNm()
    {
        // 无当前值时：2.50 lbf.in → 283 mNm（与设备设定后量化方向一致）
        Assert.Equal(283, TorqueUnitConverter.DisplayToMilliNm(2.50, DefaultTorqueUnit.LbfIn));
    }

    [Fact]
    public void MilliNmToDisplay_RoundsToTwoDecimals()
    {
        // 70 / 282 mNm → raw 0.61943 / 2.495418 lbf.in；两位小数对齐设备 0.62 / 2.50。
        Assert.Equal(0.62, TorqueUnitConverter.MilliNmToDisplay(70, DefaultTorqueUnit.LbfIn));
        Assert.Equal(0.71, TorqueUnitConverter.MilliNmToDisplay(70, DefaultTorqueUnit.KgfCm));
        Assert.Equal(2.50, TorqueUnitConverter.MilliNmToDisplay(282, DefaultTorqueUnit.LbfIn));
        Assert.Equal(2.88, TorqueUnitConverter.MilliNmToDisplay(282, DefaultTorqueUnit.KgfCm));
    }

    [Fact]
    public void GetUnitSymbol_MatchesControllerLabels()
    {
        Assert.Equal("N.m", TorqueUnitConverter.GetUnitSymbol(DefaultTorqueUnit.NewtonMeter));
        Assert.Equal("kgf.cm", TorqueUnitConverter.GetUnitSymbol(DefaultTorqueUnit.KgfCm));
        Assert.Equal("lbf.ft", TorqueUnitConverter.GetUnitSymbol(DefaultTorqueUnit.LbfFt));
        Assert.Equal("lbf.in", TorqueUnitConverter.GetUnitSymbol(DefaultTorqueUnit.LbfIn));
    }
}
