namespace UDL.Delta.IemdSd;

public sealed class IemdSdClientOptions
{
    public string Host { get; set; } = "192.168.1.11";

    public int Port { get; set; } = 502;

    public int ToolIndex { get; set; }

    public TighteningTriggerMode TriggerMode { get; set; } = TighteningTriggerMode.AutoDi;

    public bool AutoLockOnInit { get; set; }

    public bool SendUnlockAfterCycle { get; set; } = true;

    public bool UseLegacyFinishRegister { get; set; }

    public int CommandTimeoutMs { get; set; } = 3000;

    public int TighteningPollIntervalMs { get; set; } = 100;

    public int ReadWindowSize { get; set; } = 120;
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
