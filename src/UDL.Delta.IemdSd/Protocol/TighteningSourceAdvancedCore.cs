namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#301/#351 来源进阶设定（手册 A.3.3 0x13B–0x14A）。</summary>
public sealed class TighteningSourceAdvancedCore
{
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

    /// <summary>Bit12：扫码长度不正确禁止启动（手册英文 CH07；长度阈值字段待真机确认）。</summary>
    public bool ProhibitStartWhenBarcodeLengthMismatch { get; set; }

    public int DualToolParamSelect { get; set; }

    public DefaultTorqueUnit TorqueUnit { get; set; } = DefaultTorqueUnit.LbfIn;

    public ToolStartCondition StartConditionTool1 { get; set; } = ToolStartCondition.PushStart;

    public ToolStartCondition StartConditionTool2 { get; set; } = ToolStartCondition.PushStart;

    public static TighteningSourceAdvancedCore CreateDefaults() => new();
}

/// <summary>#301 0x149/0x14A 工具启动条件（手册；英文版含 DI 组合项）。</summary>
public enum ToolStartCondition
{
    PushStart = 0,
    DigitalDi = 1,
    LeverStart = 2,
    PushOrLever = 3,
    PushAndLever = 4,
    DiOrPush = 5,
    DiOrLever = 6,
}
