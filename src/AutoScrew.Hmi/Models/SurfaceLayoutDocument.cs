namespace AutoScrew.Hmi.Models;

/// <summary>单面 2D 引导布局（v2 中 <c>surfaces[]</c> 元素；字段与 v1 <see cref="TemplateDocument"/> 对齐）。</summary>
public sealed class SurfaceLayoutDocument
{
    public string SurfaceId { get; set; } = "DEFAULT";

    public string Name { get; set; } = "面 1";

    public int Order { get; set; } = 1;

    public bool Enabled { get; set; } = true;

    public double BoardWidth { get; set; }

    public double BoardHeight { get; set; }

    public double CircleDiameter { get; set; } = 28;

    public string? ProductImageRelativePath { get; set; }

    public string? ProductImageAbsolutePath { get; set; }

    public double? ProductImageOpacity { get; set; }

    public List<MarkerRecord> Markers { get; set; } = new();
}
