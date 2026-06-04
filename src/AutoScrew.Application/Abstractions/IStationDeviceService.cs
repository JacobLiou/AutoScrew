using UDL.Delta.IemdSd;

namespace AutoScrew.Application.Abstractions;

public enum ControllerTransport
{
    ModbusTcp,
    ModbusRtu,
}

public sealed class StationDeviceEndpoint
{
    public const int MaxSlots = 3;

    public int SlotIndex { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public ControllerTransport Transport { get; set; } = ControllerTransport.ModbusTcp;

    public string Host { get; set; } = "192.168.1.11";

    public int Port { get; set; } = 502;

    public string SerialPortName { get; set; } = "COM1";

    public int BaudRate { get; set; } = 115200;

    public int DataBits { get; set; } = 8;

    public string Parity { get; set; } = "None";

    public string StopBits { get; set; } = "One";

    public int ToolIndex { get; set; }

    public string TriggerMode { get; set; } = "AutoDi";

    public bool AutoLockOnInit { get; set; }

    public bool SendUnlockAfterCycle { get; set; } = true;

    public bool UseLegacyFinishRegister { get; set; }

    public int CommandTimeoutMs { get; set; } = 3000;

    public string DescribeConnection() =>
        Transport == ControllerTransport.ModbusRtu
            ? $"{SerialPortName} @ {BaudRate}"
            : $"{Host}:{Port}";
}

public sealed class StationDeviceConfiguration
{
    public string StationId { get; set; } = "STATION-01";

    public int ActiveDeviceSlot { get; set; }

    public List<StationDeviceEndpoint> Devices { get; set; } = CreateDefaultDevices();

    public static List<StationDeviceEndpoint> CreateDefaultDevices()
    {
        return
        [
            new StationDeviceEndpoint { SlotIndex = 0, DisplayName = "Device 1", Enabled = true },
            new StationDeviceEndpoint { SlotIndex = 1, DisplayName = "Device 2" },
            new StationDeviceEndpoint { SlotIndex = 2, DisplayName = "Device 3" },
        ];
    }

    public StationDeviceEndpoint? GetActiveDevice()
    {
        if (ActiveDeviceSlot < 0 || ActiveDeviceSlot >= Devices.Count)
            return null;
        return Devices[ActiveDeviceSlot];
    }
}

public sealed record TestConnectionResult(bool Success, string Message);

public sealed record ActiveDeviceSummary(
    string StationId,
    int SlotIndex,
    string DisplayName,
    string ConnectionDescription,
    bool IsEnabled);

public interface IStationDeviceService
{
    string StationId { get; }

    bool IsSimulatedHardware { get; }

    bool IsRuntimeDeviceAvailable { get; }

    Task<StationDeviceConfiguration> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(StationDeviceConfiguration configuration, CancellationToken cancellationToken = default);

    Task<TestConnectionResult> TestConnectionAsync(int slotIndex, CancellationToken cancellationToken = default);

    Task<TestConnectionResult> ApplyActiveDeviceAsync(CancellationToken cancellationToken = default);

    Task EnsureActiveClientAsync(CancellationToken cancellationToken = default);

    ActiveDeviceSummary? GetActiveDeviceSummary();

    IIemdSdClient? GetActiveClient();
}
