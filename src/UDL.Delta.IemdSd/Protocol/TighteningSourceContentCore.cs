namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningSourceModeCore
{
    public int ToolIndex { get; set; }

    public TighteningOperatingMode OperatingMode { get; set; } = TighteningOperatingMode.SingleTool;

    public TighteningSwitchingMethod SwitchingMethod { get; set; } = TighteningSwitchingMethod.Manual;
}

public sealed class TighteningSourceContentCore
{
    public int ToolIndex { get; set; }

    public int SwitchingMethodId { get; set; } = 1;

    public string Barcode { get; set; } = "";

    public TighteningSourceBindingType BindingType { get; set; } = TighteningSourceBindingType.Sequence;

    public int TargetId { get; set; } = 1;

    public int ScrewCount { get; set; } = 1;

    public int BitId { get; set; }

    public TighteningSourceAdvancedCore Advanced { get; set; } = TighteningSourceAdvancedCore.CreateDefaults();
}
