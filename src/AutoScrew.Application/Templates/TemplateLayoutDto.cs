using System.Text.Json.Serialization;

namespace AutoScrew.Application.Templates;

/// <summary>
/// JSON root compatible with AutoScrew.TemplateBoard (schemaVersion 1).
/// </summary>
public sealed class TemplateLayoutDto
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("boardWidth")]
    public double BoardWidth { get; set; }

    [JsonPropertyName("boardHeight")]
    public double BoardHeight { get; set; }

    [JsonPropertyName("circleDiameter")]
    public double CircleDiameter { get; set; }

    [JsonPropertyName("productImageRelativePath")]
    public string? ProductImageRelativePath { get; set; }

    [JsonPropertyName("productImageAbsolutePath")]
    public string? ProductImageAbsolutePath { get; set; }

    [JsonPropertyName("productImageOpacity")]
    public double? ProductImageOpacity { get; set; }

    [JsonPropertyName("markers")]
    public List<MarkerDto> Markers { get; set; } = new();
}

public sealed class MarkerDto
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("centerX")]
    public double CenterX { get; set; }

    [JsonPropertyName("centerY")]
    public double CenterY { get; set; }

    [JsonPropertyName("screwTypeId")]
    public int? ScrewTypeId { get; set; }

    [JsonPropertyName("circleDiameter")]
    public double? CircleDiameter { get; set; }

    [JsonPropertyName("partNo")]
    public string? PartNo { get; set; }
}
