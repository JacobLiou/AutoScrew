namespace AutoScrew.Application.Configuration;

/// <summary>来源进阶设定（线框 §7.4）；P1 先本地 JSON 持久化，设备 Modbus 待协议对齐。</summary>
public sealed class SourceAdvancedSettingsCore
{
    public int SettingsId { get; set; } = 1;

    public int StartConditionTorqueUnitIndex { get; set; }

    public int StartConditionTriggerIndex { get; set; }

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

    public static SourceAdvancedSettingsCore CreateDefaults() => new();
}
