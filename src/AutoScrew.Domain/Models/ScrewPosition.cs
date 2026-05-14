namespace AutoScrew.Domain.Models;

/// <summary>
/// One screw station on the product template (aligns with TemplateBoard marker + optional PN fields).
/// </summary>
public sealed class ScrewPosition
{
    public ScrewPosition(int index, double centerX, double centerY, double? circleDiameterPx, int? screwTypeId, string? partNumber = null)
    {
        Index = index;
        CenterX = centerX;
        CenterY = centerY;
        CircleDiameterPx = circleDiameterPx;
        ScrewTypeId = screwTypeId;
        PartNumber = partNumber;
    }

    public int Index { get; }

    public double CenterX { get; }

    public double CenterY { get; }

    public double? CircleDiameterPx { get; }

    public int? ScrewTypeId { get; }

    public string? PartNumber { get; }

    public double NormalizedX(double boardWidth) => boardWidth > 0 ? CenterX / boardWidth : 0;

    public double NormalizedY(double boardHeight) => boardHeight > 0 ? CenterY / boardHeight : 0;
}
