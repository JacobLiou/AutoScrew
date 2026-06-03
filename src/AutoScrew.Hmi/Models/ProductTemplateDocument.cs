namespace AutoScrew.Hmi.Models;

/// <summary>多面产品模板包（schemaVersion 2）。</summary>
public sealed class ProductTemplateDocument
{
    public const int CurrentSchemaVersion = 2;

    public const string DefaultAssemblySequence = "surfaceOrderThenLocalIndex";

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    public string ProductId { get; set; } = "";

    public string? DisplayName { get; set; }

    public string? Revision { get; set; }

    public int SurfaceCount { get; set; }

    public string AssemblySequence { get; set; } = DefaultAssemblySequence;

    public List<SurfaceLayoutDocument> Surfaces { get; set; } = new();

    public Dictionary<string, string>? Metadata { get; set; }
}
