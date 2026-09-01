using UDL.Delta.IemdSd.Internal;

namespace UDL.Delta.IemdSd.Tests;

public class CurveReaderTests
{
    [Fact]
    public void ComposePoints_UsesContiguousTorqueWords()
    {
        // Simulate torque chunks written at dest = pointIndex * 2 (1500 points).
        const int pointCount = 1500;
        var angle = new int[pointCount];
        var torque = new int[pointCount * 2];

        for (var i = 0; i < pointCount; i++)
        {
            angle[i] = i;
            var milliNm = (i + 1) * 10; // 0.01, 0.02, ... N·m
            torque[i * 2] = milliNm & 0xFFFF;
            torque[i * 2 + 1] = milliNm >> 16;
        }

        var points = CurveReader.ComposePoints(angle, torque, pointCount);

        Assert.Equal(pointCount, points.Count);
        Assert.Equal(0, points[0].AngleDeg);
        Assert.Equal(0.01, points[0].TorqueNm, 3);
        Assert.Equal(999, points[999].AngleDeg);
        Assert.Equal(10.0, points[999].TorqueNm, 3);
        Assert.Equal(1499, points[1499].AngleDeg);
        Assert.Equal(15.0, points[1499].TorqueNm, 3);
    }
}
