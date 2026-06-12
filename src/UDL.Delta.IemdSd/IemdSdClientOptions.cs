namespace UDL.Delta.IemdSd;

public enum ControllerTransportType
{
    ModbusTcp,
    ModbusRtu,
}

public sealed class IemdSdClientOptions
{
    public ControllerTransportType Transport { get; set; } = ControllerTransportType.ModbusTcp;

    public string Host { get; set; } = "192.168.1.11";

    public int Port { get; set; } = 502;

    public string SerialPortName { get; set; } = "COM1";

    public int BaudRate { get; set; } = 115200;

    public int DataBits { get; set; } = 8;

    public string Parity { get; set; } = "None";

    public string StopBits { get; set; } = "One";

    public byte ModbusSlaveId { get; set; } = 1;

    public int ToolIndex { get; set; }

    public TighteningTriggerMode TriggerMode { get; set; } = TighteningTriggerMode.AutoDi;

    public bool AutoLockOnInit { get; set; }

    public bool SendUnlockAfterCycle { get; set; } = true;

    public bool UseLegacyFinishRegister { get; set; }

    public int CommandTimeoutMs { get; set; } = 3000;

    public int TighteningPollIntervalMs { get; set; } = 100;

    public int ReadWindowSize { get; set; } = 120;

    /// <summary>RTU inter-frame delay after each write (manual Appendix B, default 10 ms).</summary>
    public int RtuInterFrameDelayMs { get; set; } = 10;
}

public sealed class IemdSdInitOptions
{
    public bool ClearDi { get; set; } = true;

    public bool ReadCurveVersion { get; set; } = true;
}

public enum TighteningTriggerMode
{
    Manual,
    AutoDi,
}

public enum TighteningTrigger
{
    Manual,
    AutoDi,
}
