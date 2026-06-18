using AutoScrew.Application.Editing;
using Xunit;

namespace AutoScrew.Tests;

public sealed class SurfaceBoardEditorMoveMarkersTests
{
    [Fact]
    public void ApplyDelta_moves_marker()
    {
        var (x, y) = BoardMarkerMovement.ApplyDelta(100, 100, 5, 0, 800, 600);
        Assert.Equal(105, x);
        Assert.Equal(100, y);
    }

    [Fact]
    public void ApplyDelta_clamps_to_board_edges()
    {
        var (x, y) = BoardMarkerMovement.ApplyDelta(795, 10, 20, -20, 800, 600);
        Assert.Equal(800, x);
        Assert.Equal(0, y);
    }

    [Fact]
    public void ApplyDelta_preserves_relative_offset_for_multi_marker_simulation()
    {
        var first = BoardMarkerMovement.ApplyDelta(100, 100, 30, 20, 800, 600);
        var second = BoardMarkerMovement.ApplyDelta(300, 200, 30, 20, 800, 600);

        Assert.Equal(130, first.X);
        Assert.Equal(120, first.Y);
        Assert.Equal(330, second.X);
        Assert.Equal(220, second.Y);
    }
}
