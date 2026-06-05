using UDL.Delta.IemdSd;

namespace AutoScrew.Application.Abstractions;

public enum ControllerTransport
{
    ModbusTcp,
    ModbusRtu,
}

public sealed class StationDeviceEndpoint
{
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

    public StationDeviceEndpoint Device { get; set; } = CreateDefaultDevice();

    public static StationDeviceEndpoint CreateDefaultDevice() =>
        new() { DisplayName = "IEMD-SD", Enabled = true };

    public StationDeviceEndpoint? GetDevice() => Device;
}

public sealed record TestConnectionResult(bool Success, string Message);

public sealed record DeviceSummary(
    string StationId,
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

    Task<TestConnectionResult> TestConnectionAsync(CancellationToken cancellationToken = default);

    Task<TestConnectionResult> ApplyDeviceAsync(CancellationToken cancellationToken = default);

    Task EnsureClientAsync(CancellationToken cancellationToken = default);

    DeviceSummary? GetDeviceSummary();

    IIemdSdClient? GetClient();
}
