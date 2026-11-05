namespace UDL.Delta.IemdSd;

public enum ControllerTransportType
{
    ModbusTcp,
    ModbusRtu,
}

/// <summary>
/// 控制器协议档。对齐 SD3 10.1 寸 / SD30X0 4.3 寸手册附录 A.3.1 差异。
/// 缺省 <see cref="Sd3_101"/> 与现网行为一致。
/// </summary>
public enum IemdSdControllerProfile
{
    Sd3_101 = 0,
    Sd3_43 = 1,
}

public sealed class IemdSdClientOptions
{
    public ControllerTransportType Transport { get; set; } = ControllerTransportType.ModbusTcp;

    /// <summary>控制器协议档；影响 #100 阶段控制字、0xCC、0xEC 与初始化 #533。</summary>
    public IemdSdControllerProfile ControllerProfile { get; set; } = IemdSdControllerProfile.Sd3_101;

    public string Host { get; set; } = "192.168.1.11";

    public int Port { get; set; } = 502;

    public string SerialPortName { get; set; } = "COM1";

    public int BaudRate { get; set; } = 115200;

    public int DataBits { get; set; } = 8;

    public string Parity { get; set; } = "None";

    public string StopBits { get; set; } = "One";

    public byte ModbusSlaveId { get; set; } = 1;

    public int ToolIndex { get; set; }

    public TighteningTriggerMode TriggerMode { get; set; } = TighteningTriggerMode.Manual;

    public bool AutoLockOnInit { get; set; }

    public bool SendUnlockAfterCycle { get; set; } = true;

    public bool UseLegacyFinishRegister { get; set; }

    public int CommandTimeoutMs { get; set; } = 3000;

    /// <summary>Max wait for tighten finish status (0x1F5D / legacy). Default 120 seconds.</summary>
    public int TighteningCycleTimeoutMs { get; set; } = 120_000;

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
