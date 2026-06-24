namespace AutoScrew.Application.Configuration;

/// <summary>步骤③ 单行来源绑定（可多工具）。</summary>
public sealed class ControllerSourceBindingEntry
{
    public int ToolIndex { get; set; }

    /// <summary>0 = 参数，1 = 顺序（与 <see cref="UDL.Delta.IemdSd.Protocol.TighteningSourceBindingType"/> 一致）。</summary>
    public int BindingType { get; set; } = 1;

    public int TargetId { get; set; } = 1;

    public int ScrewCount { get; set; } = 1;

    public int BitId { get; set; }

    public string Barcode { get; set; } = "";

    public SourceAdvancedSettingsCore? Advanced { get; set; }
}
