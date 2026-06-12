namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#517 / #561 per-screw result export format (manual: 0=off, 3=BIN on HMI disk).</summary>
public enum PerScrewExportMode : ushort
{
    Off = 0,
    CsvHmiDisk = 1,
    CsvUsb = 2,
    BinHmiDisk = 3,
}
