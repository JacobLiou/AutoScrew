namespace UDL.Delta.IemdSd.Tests;

public class TorqueUnitConverterTests
{
    [Fact]
    public void ToNm_SystemUnit_IsIdentityAt1001()
    {
        var nm = TorqueUnitConverter.ToNm(1500, 1001);
        Assert.Equal(1.5, nm, 3);
    }
}
