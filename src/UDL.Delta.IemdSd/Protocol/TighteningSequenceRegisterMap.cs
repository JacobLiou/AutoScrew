namespace UDL.Delta.IemdSd.Protocol;

/// <summary>Sequence block relative 0xD2 (manual A.3.2 #200/#250, 0xD2–0x2E3).</summary>
public static class TighteningSequenceRegisterMap
{
    public const int NameWordCount = 20;
    public const int NameStart = 0xD2 - 0xD2;

    public const int NavigatorMode = 0xE6 - 0xD2;
    public const int PositioningArmEnabled = 0xE7 - 0xD2;

    public const int ToolIdStart = 0xF0 - 0xD2;
    public const int MaxSteps = 100;

    /// <summary>第 1–100 组拧紧参数 ID（手册 0x154–0x1B7）。</summary>
    public const int ParameterIdStart = 0x154 - 0xD2;

    /// <summary>第 1–100 组螺丝数量 DWORD L/H（手册 0x1B8–0x27F）。</summary>
    public const int QuantityStart = 0x1B8 - 0xD2;

    /// <summary>第 1–100 组提示批头编号（手册 0x280–0x2E3）。</summary>
    public const int BitIdStart = 0x280 - 0xD2;

    public const int BlockWordCount = 0x2E3 - 0xD2 + 1;

    public const int NavigatorCoordinateWordCount = 0x199 - 0xD2 + 1;
    public const int NavigatorImageCodeWordCount = 0x135 - 0xD2 + 1;
    public const int PositioningArmWordCount = 0x329 - 0xD2 + 1;

    public const int SourceBarcodeWordCount = 0x135 - 0xD2 + 1;
    public const int SourceType = 0x136 - 0xD2;
    public const int SourceTargetId = 0x137 - 0xD2;
    public const int SourceScrewCountLow = 0x138 - 0xD2;
    public const int SourceScrewCountHigh = 0x139 - 0xD2;
    public const int SourceBitId = 0x13A - 0xD2;

    /// <summary>#301/#351 进阶设定标志(L) Bit0–15。</summary>
    public const int SourceAdvancedFlagsLow = 0x13B - 0xD2;

    /// <summary>#301/#351 进阶设定标志(H)。</summary>
    public const int SourceAdvancedFlagsHigh = 0x13C - 0xD2;

    public const int SourceMaxTightenNgLow = 0x13D - 0xD2;
    public const int SourceMaxTightenNgHigh = 0x13E - 0xD2;
    public const int SourceMaxLoosenNgLow = 0x13F - 0xD2;
    public const int SourceMaxLoosenNgHigh = 0x140 - 0xD2;
    public const int SourceReserved141 = 0x141 - 0xD2;
    public const int SourceMaxRunTimeLow = 0x145 - 0xD2;
    public const int SourceMaxRunTimeHigh = 0x146 - 0xD2;
    public const int SourceDualToolParamSelect = 0x147 - 0xD2;
    public const int SourceTorqueUnit = 0x148 - 0xD2;
    public const int SourceStartConditionTool1 = 0x149 - 0xD2;
    public const int SourceStartConditionTool2 = 0x14A - 0xD2;

    public const int SourceContentWordCount = 0x14A - 0xD2 + 1;

    public const int AdvBitProhibitLoosenAfterTightenOk = 0;
    public const int AdvBitProhibitLoosenAfterTightenNg = 1;
    public const int AdvBitLimitMaxTightenNg = 2;
    public const int AdvBitLimitMaxLoosenNg = 3;
    public const int AdvBitAutoNextOnTightenNg = 4;
    public const int AdvBitGoBackOnLoosenOk = 5;
    public const int AdvBitProhibitStartWhenBarcodeEmpty = 6;
    public const int AdvBitClearBarcodeWhenScrewComplete = 7;
    public const int AdvBitProhibitScanWhenScrewIncomplete = 8;
    public const int AdvBitLimitMaxRunTime = 9;
    public const int AdvBitResetCountWhenScrewComplete = 10;
    public const int AdvBitPromptTightenSignalEarly = 11;
    public const int AdvBitProhibitStartWhenBarcodeLengthMismatch = 12;
}
