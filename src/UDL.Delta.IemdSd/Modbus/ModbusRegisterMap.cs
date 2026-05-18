namespace UDL.Delta.IemdSd.Modbus;

/// <summary>保持寄存器地址（与厂商 Demo / driverAnaC 一致，十六进制直接作为 EasyModbus 地址）。</summary>
public static class ModbusRegisterMap
{
    public const int CommandRequest = 0xC8;
    public const int CommandResponse = 0xCF;
    public const int CommandData = 0xD2;

    public const int DiStatus = 0x67;
    public const int DiCommand = 0x68;
    public const int ReportIdLow = 0x6B;
    public const int ReportIdHigh = 0x6C;

    public const int TotalAngle = 0x24;
    public const int TighteningResultLegacy = 0x26;

    public const int Ready = 0x1F52;
    public const int TighteningFinish = 0x1F5D;
    public const int FinalTorqueLow = 0x1F46;
    public const int FinalTorqueHigh = 0x1F47;
}

public static class ModbusFunctionCodes
{
    public const int SwitchParameter = 302;
    public const int LimitTightening = 406;
    public const int AutoExportBin = 517;
    public const int AutoLock = 533;
    public const int CurveSampleRate = 562;
    public const int ReadReport = 750;
    public const int ReadCurve = 751;
}
