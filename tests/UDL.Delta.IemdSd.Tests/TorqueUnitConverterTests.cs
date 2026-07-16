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
        Assert.Equal(4.58865, kgfCm, 3);
    }
}
