using System.Text.Json.Serialization;

namespace AutoScrew.Application.Templates;

/// <summary>多面产品模板 JSON 根（schemaVersion 2）。</summary>
public sealed class ProductTemplateDto
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 2;

    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("revision")]
    public string? Revision { get; set; }

    [JsonPropertyName("surfaceCount")]
    public int SurfaceCount { get; set; }

    [JsonPropertyName("assemblySequence")]
    public string AssemblySequence { get; set; } = "surfaceOrderThenLocalIndex";

    [JsonPropertyName("surfaces")]
    public List<SurfaceLayoutDto> Surfaces { get; set; } = new();

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }
}

public sealed class SurfaceLayoutDto
{
    [JsonPropertyName("surfaceId")]
    public string SurfaceId { get; set; } = "DEFAULT";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("order")]
    public int Order { get; set; } = 1;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("boardWidth")]
    public double BoardWidth { get; set; }

    [JsonPropertyName("boardHeight")]
    public double BoardHeight { get; set; }

    [JsonPropertyName("circleDiameter")]
    public double CircleDiameter { get; set; } = 28;

    [JsonPropertyName("productImageRelativePath")]
    public string? ProductImageRelativePath { get; set; }

    [JsonPropertyName("productImageAbsolutePath")]
    public string? ProductImageAbsolutePath { get; set; }

    [JsonPropertyName("productImageOpacity")]
    public double? ProductImageOpacity { get; set; }

    [JsonPropertyName("markers")]
    public List<MarkerDto> Markers { get; set; } = new();
}
