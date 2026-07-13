using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace UDL.Delta.IemdSd.Tests;

public sealed class ParameterListSnapshotTests
{
    [Fact]
    public void GetConfiguredIds_ReturnsOneBasedIndicesForNonZeroWords()
    {
        var snapshot = new ParameterListSnapshot
        {
            RawWords = [0, 1, 0, 1, 0],
        };

        var ids = snapshot.GetConfiguredIds();

        Assert.Equal([2, 4], ids);
    }

    [Fact]
    public void GetConfiguredIds_RespectsMaxParameterSlots()
    {
        var words = new int[ParameterListSnapshot.MaxParameterSlots + 10];
        words[499] = 1;
        words[500] = 1;

        var ids = new ParameterListSnapshot { RawWords = words }.GetConfiguredIds();

        Assert.Single(ids);
        Assert.Equal(500, ids[0]);
    }
}
