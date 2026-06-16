namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#300 CB operating mode (SD3 manual A.3.3).</summary>
public enum TighteningOperatingMode
{
    SingleTool = 0,
    DualToolAlternation = 1,
    DualToolSynchronization = 2,
}

/// <summary>#300 CC switching method.</summary>
public enum TighteningSwitchingMethod
{
    Manual = 0,
    ScrewBitSelector = 1,
    BarcodeScanner = 2,
}

/// <summary>#301 source binding type.</summary>
public enum TighteningSourceBindingType
{
    Parameter = 0,
    Sequence = 1,
}
