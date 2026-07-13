using AutoScrew.Infrastructure.Hardware;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class IemdSdProductionSetupTests
{
    [Fact]
    public void SwitchingMethodManual_MatchesEnumAndHandbook()
    {
        Assert.Equal((int)TighteningSwitchingMethod.Manual, IemdSdProductionSetup.SwitchingMethodManual);
        Assert.Equal(0, IemdSdProductionSetup.SwitchingMethodManual);
    }

    [Fact]
    public void SingleToolOperatingMode_MatchesEnumAndHandbook()
    {
        Assert.Equal((int)TighteningOperatingMode.SingleTool, IemdSdProductionSetup.SingleToolOperatingMode);
    }
}
