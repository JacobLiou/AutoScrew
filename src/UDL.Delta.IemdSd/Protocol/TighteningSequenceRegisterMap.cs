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

    public const int ParameterIdStart = 0x154 - 0xD2;

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
    public const int SourceContentWordCount = 0x14A - 0xD2 + 1;
}
