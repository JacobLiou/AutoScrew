namespace AutoScrew.TemplateBoard.Models;

/// <summary>
/// Root JSON document for screw layout templates (see README).
/// </summary>
public sealed class TemplateDocument
{
    public int SchemaVersion { get; set; } = 1;

    public double BoardWidth { get; set; }

    public double BoardHeight { get; set; }

    /// <summary>新建标注时的默认直径（旧文件兼容）；单点可覆盖。</summary>
    public double CircleDiameter { get; set; } = 28;

    /// <summary>相对 JSON 文件所在目录的产品图路径（优先）。</summary>
    public string? ProductImageRelativePath { get; set; }

    /// <summary>当相对路径不可用（跨盘符等）时回退为绝对路径。</summary>
    public string? ProductImageAbsolutePath { get; set; }

    /// <summary>底图不透明度 0..1。</summary>
    public double? ProductImageOpacity { get; set; }

    public List<MarkerRecord> Markers { get; set; } = new();
}

public sealed class MarkerRecord
{
    public int Index { get; set; }

    public double CenterX { get; set; }

    public double CenterY { get; set; }

    /// <summary>螺钉类型 Id，1..6，与 <see cref="ScrewTypeCatalog"/> 对应。</summary>
    public int? ScrewTypeId { get; set; }

    /// <summary>圆圈直径（像素）。若缺省则使用文档级 <see cref="TemplateDocument.CircleDiameter"/>。</summary>
    public double? CircleDiameter { get; set; }
}
