using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class ParameterBlockWriterTimeoutTests
{
    [Fact]
    public void NormalizeTimeoutsForDevice_ZeroBecomes32767()
    {
        var core = new TighteningParameterCore
        {
            MaxTighteningTimeTenthSec = 0,
            MaxLoosenTimeTenthSec = 0,
        };

        ParameterBlockWriter.NormalizeTimeoutsForDevice(core);

        Assert.Equal(ParameterBlockWriter.UnlimitedTimeTenthSec, core.MaxTighteningTimeTenthSec);
        Assert.Equal(ParameterBlockWriter.UnlimitedTimeTenthSec, core.MaxLoosenTimeTenthSec);
    }

    [Fact]
    public void NormalizeTimeoutsForDevice_NonZeroPreserved()
    {
        var core = new TighteningParameterCore
        {
            MaxTighteningTimeTenthSec = 10,
            MaxLoosenTimeTenthSec = 50,
        };

        ParameterBlockWriter.NormalizeTimeoutsForDevice(core);

        Assert.Equal(10, core.MaxTighteningTimeTenthSec);
        Assert.Equal(50, core.MaxLoosenTimeTenthSec);
    }

    [Fact]
    public void ValidateTimeoutsForDevice_OverLimit_ThrowsChinese()
    {
        var core = new TighteningParameterCore
        {
            MaxTighteningTimeTenthSec = 32768,
            MaxLoosenTimeTenthSec = 10,
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ParameterBlockWriter.ValidateTimeoutsForDevice(core));
        Assert.Contains("32767", ex.Message, StringComparison.Ordinal);
        Assert.Contains("拧紧超时", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateTimeoutsForDevice_AtLimit_Ok()
    {
        var core = new TighteningParameterCore
        {
            MaxTighteningTimeTenthSec = 32767,
            MaxLoosenTimeTenthSec = 32767,
        };

        ParameterBlockWriter.ValidateTimeoutsForDevice(core);
    }
}
