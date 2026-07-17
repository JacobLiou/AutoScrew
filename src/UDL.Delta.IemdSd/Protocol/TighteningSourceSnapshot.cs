namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningSourceSnapshot
{
    public int ToolIndex { get; init; }

    public int OperatingMode { get; init; }

    public int SwitchingMethod { get; init; }

    public int SourceId { get; init; }

    public int ParameterId { get; init; }

    public int SequenceId { get; init; }

    public int ScrewCount { get; init; }

    public TighteningSourceBindingType BindingType { get; init; }

    public string Barcode { get; init; } = "";

    public int BitId { get; init; }

    public TighteningSourceAdvancedCore Advanced { get; init; } = TighteningSourceAdvancedCore.CreateDefaults();

    public static TighteningSourceSnapshot FromMode(TighteningSourceModeCore mode) => new()
    {
        ToolIndex = mode.ToolIndex,
        OperatingMode = (int)mode.OperatingMode,
        SwitchingMethod = (int)mode.SwitchingMethod,
    };

    public static TighteningSourceSnapshot FromContent(TighteningSourceContentCore content) => new()
    {
        SourceId = content.SwitchingMethodId,
        ToolIndex = content.ToolIndex,
        ParameterId = content.BindingType == TighteningSourceBindingType.Parameter ? content.TargetId : 0,
        SequenceId = content.BindingType == TighteningSourceBindingType.Sequence ? content.TargetId : 0,
        ScrewCount = content.ScrewCount,
        BindingType = content.BindingType,
        Barcode = content.Barcode,
        BitId = content.BitId,
        Advanced = content.Advanced ?? TighteningSourceAdvancedCore.CreateDefaults(),
    };

    public TighteningSourceModeCore ToModeCore() => new()
    {
        ToolIndex = ToolIndex,
        OperatingMode = (TighteningOperatingMode)OperatingMode,
        SwitchingMethod = (TighteningSwitchingMethod)SwitchingMethod,
    };

    public TighteningSourceContentCore ToContentCore() => new()
    {
        ToolIndex = ToolIndex,
        SwitchingMethodId = SourceId > 0 ? SourceId : 1,
        Barcode = Barcode,
        BindingType = BindingType,
        TargetId = BindingType == TighteningSourceBindingType.Sequence ? SequenceId : ParameterId,
        ScrewCount = ScrewCount,
        BitId = BitId,
        Advanced = Advanced ?? TighteningSourceAdvancedCore.CreateDefaults(),
    };
}
