using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Configuration;

/// <summary>来源进阶设定（对齐 #301/#351 0x13B–0x14A；本地 JSON 与设备同步）。</summary>
public sealed class SourceAdvancedSettingsCore
{
    public int SettingsId { get; set; } = 1;

    public int StartConditionTorqueUnitIndex
    {
        get => (int)TorqueUnit;
        set => TorqueUnit = Enum.IsDefined(typeof(DefaultTorqueUnit), (ushort)value)
            ? (DefaultTorqueUnit)(ushort)value
            : DefaultTorqueUnit.LbfIn;
    }

    public int StartConditionTriggerIndex
    {
        get => (int)StartConditionTool1;
        set => StartConditionTool1 = Enum.IsDefined(typeof(ToolStartCondition), value)
            ? (ToolStartCondition)value
            : ToolStartCondition.PushStart;
    }

    public DefaultTorqueUnit TorqueUnit { get; set; } = DefaultTorqueUnit.LbfIn;

    public ToolStartCondition StartConditionTool1 { get; set; } = ToolStartCondition.PushStart;

    public ToolStartCondition StartConditionTool2 { get; set; } = ToolStartCondition.PushStart;

    public int DualToolParamSelect { get; set; }

    public bool ProhibitLoosenAfterTightenOk { get; set; }

    public bool ProhibitLoosenAfterTightenNg { get; set; }

    public bool LimitMaxTightenNgPerScrew { get; set; } = true;

    public int MaxTightenNgPerScrew { get; set; } = 999999;

    public bool LimitMaxLoosenNgPerScrew { get; set; } = true;

    public int MaxLoosenNgPerScrew { get; set; } = 999999;

    public bool AutoNextOnTightenNg { get; set; }

    public bool GoBackOnLoosenOk { get; set; }

    public bool ProhibitStartWhenBarcodeEmpty { get; set; }

    public bool ClearBarcodeWhenScrewCountComplete { get; set; }

    public bool ProhibitScanWhenScrewCountIncomplete { get; set; }

    public bool LimitMaxRunTime { get; set; } = true;

    public int MaxRunTimeSeconds { get; set; } = 9999999;

    public bool ResetCountWhenScrewCountComplete { get; set; } = true;

    public bool PromptWhenTightenSignalDisappearsEarly { get; set; }

    public bool ProhibitStartWhenBarcodeLengthMismatch { get; set; }

    public static SourceAdvancedSettingsCore CreateDefaults() => new();

    public static SourceAdvancedSettingsCore FromProtocol(TighteningSourceAdvancedCore proto)
    {
        ArgumentNullException.ThrowIfNull(proto);
        return new SourceAdvancedSettingsCore
        {
            ProhibitLoosenAfterTightenOk = proto.ProhibitLoosenAfterTightenOk,
            ProhibitLoosenAfterTightenNg = proto.ProhibitLoosenAfterTightenNg,
            LimitMaxTightenNgPerScrew = proto.LimitMaxTightenNgPerScrew,
            MaxTightenNgPerScrew = proto.MaxTightenNgPerScrew,
            LimitMaxLoosenNgPerScrew = proto.LimitMaxLoosenNgPerScrew,
            MaxLoosenNgPerScrew = proto.MaxLoosenNgPerScrew,
            AutoNextOnTightenNg = proto.AutoNextOnTightenNg,
            GoBackOnLoosenOk = proto.GoBackOnLoosenOk,
            ProhibitStartWhenBarcodeEmpty = proto.ProhibitStartWhenBarcodeEmpty,
            ClearBarcodeWhenScrewCountComplete = proto.ClearBarcodeWhenScrewCountComplete,
            ProhibitScanWhenScrewCountIncomplete = proto.ProhibitScanWhenScrewCountIncomplete,
            LimitMaxRunTime = proto.LimitMaxRunTime,
            MaxRunTimeSeconds = proto.MaxRunTimeSeconds,
            ResetCountWhenScrewCountComplete = proto.ResetCountWhenScrewCountComplete,
            PromptWhenTightenSignalDisappearsEarly = proto.PromptWhenTightenSignalDisappearsEarly,
            ProhibitStartWhenBarcodeLengthMismatch = proto.ProhibitStartWhenBarcodeLengthMismatch,
            DualToolParamSelect = proto.DualToolParamSelect,
            TorqueUnit = proto.TorqueUnit,
            StartConditionTool1 = proto.StartConditionTool1,
            StartConditionTool2 = proto.StartConditionTool2,
        };
    }

    public TighteningSourceAdvancedCore ToProtocol() => new()
    {
        ProhibitLoosenAfterTightenOk = ProhibitLoosenAfterTightenOk,
        ProhibitLoosenAfterTightenNg = ProhibitLoosenAfterTightenNg,
        LimitMaxTightenNgPerScrew = LimitMaxTightenNgPerScrew,
        MaxTightenNgPerScrew = MaxTightenNgPerScrew,
        LimitMaxLoosenNgPerScrew = LimitMaxLoosenNgPerScrew,
        MaxLoosenNgPerScrew = MaxLoosenNgPerScrew,
        AutoNextOnTightenNg = AutoNextOnTightenNg,
        GoBackOnLoosenOk = GoBackOnLoosenOk,
        ProhibitStartWhenBarcodeEmpty = ProhibitStartWhenBarcodeEmpty,
        ClearBarcodeWhenScrewCountComplete = ClearBarcodeWhenScrewCountComplete,
        ProhibitScanWhenScrewCountIncomplete = ProhibitScanWhenScrewCountIncomplete,
        LimitMaxRunTime = LimitMaxRunTime,
        MaxRunTimeSeconds = MaxRunTimeSeconds,
        ResetCountWhenScrewCountComplete = ResetCountWhenScrewCountComplete,
        PromptWhenTightenSignalDisappearsEarly = PromptWhenTightenSignalDisappearsEarly,
        ProhibitStartWhenBarcodeLengthMismatch = ProhibitStartWhenBarcodeLengthMismatch,
        DualToolParamSelect = DualToolParamSelect,
        TorqueUnit = TorqueUnit,
        StartConditionTool1 = StartConditionTool1,
        StartConditionTool2 = StartConditionTool2,
    };

    public SourceAdvancedSettingsCore Clone()
    {
        var copy = FromProtocol(ToProtocol());
        copy.SettingsId = SettingsId;
        return copy;
    }
}
