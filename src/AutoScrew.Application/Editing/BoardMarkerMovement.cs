namespace AutoScrew.Application.Editing;

public static class BoardMarkerMovement
{
    public static double ClampCenter(double value, double boardExtent) =>
        Math.Clamp(value, 0, boardExtent);

    public static (double X, double Y) ApplyDelta(
        double centerX,
        double centerY,
        double deltaX,
        double deltaY,
        double boardWidth,
        double boardHeight) =>
        (
            ClampCenter(centerX + deltaX, boardWidth),
            ClampCenter(centerY + deltaY, boardHeight));
}
