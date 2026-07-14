using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace UDL.Delta.IemdSd.Tests;

public sealed class ParameterListSnapshotTests
{
    [Fact]
    public void GetConfiguredIds_BinaryFlags_PrefersBitmapOverCompactCountOne()
    {
        // Firmware occupancy bitmap: word[i]==1 ⇒ parameter ID i+1 exists.
        // Old compact parser treated word0==1 as "count=1" and returned only [1].
        var snapshot = new ParameterListSnapshot
        {
            RawWords = [1, 1, 0, 1, 1, 0, 0, 1],
        };

        var ids = snapshot.GetConfiguredIds();

        Assert.Equal([1, 2, 4, 5, 8], ids);
    }

    [Fact]
    public void GetConfiguredIds_CompactList_StillWorksWhenValuesExceedOne()
    {
        var snapshot = new ParameterListSnapshot
        {
            RawWords = [3, 111, 112, 200, 0],
        };

        var ids = snapshot.GetConfiguredIds();

        Assert.Equal([111, 112, 200], ids);
    }

    [Fact]
    public void GetConfiguredIds_ReturnsOneBasedIndicesForNonZeroWords()
    {
        // Non-binary values → not flags; compact fails (count=0); bitmap used.
        var snapshot = new ParameterListSnapshot
        {
            RawWords = [0, 2, 0, 3, 0],
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
