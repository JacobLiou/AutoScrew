namespace AutoScrew.Infrastructure.Hardware;

public sealed class IemdSdOptions
{
    public const string SectionName = "IemdSd";

    public bool Enabled { get; set; }

    public string Host { get; set; } = "192.168.1.11";

    public int Port { get; set; } = 502;

    public int ToolIndex { get; set; }

    public string TriggerMode { get; set; } = "AutoDi";

    public bool AutoLockOnInit { get; set; }

    public bool SendUnlockAfterCycle { get; set; } = true;

    public bool UseLegacyFinishRegister { get; set; }

    public int CommandTimeoutMs { get; set; } = 3000;

    public Dictionary<string, int> ParameterIdByPosition { get; set; } = new();
}
