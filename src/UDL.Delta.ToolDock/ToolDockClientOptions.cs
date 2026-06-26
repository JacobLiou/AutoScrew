namespace UDL.Delta.ToolDock;

public enum ToolDockTransportType
{
    Stub,
}

public sealed class ToolDockClientOptions
{
    public ToolDockTransportType Transport { get; set; } = ToolDockTransportType.Stub;

    public string Host { get; set; } = "192.168.1.13";

    public int Port { get; set; } = 502;

    public string SerialPortName { get; set; } = "COM3";

    public int BaudRate { get; set; } = 115200;

    public byte ModbusSlaveId { get; set; } = 1;

    public int PollIntervalMs { get; set; } = 100;

    public int DebounceMs { get; set; } = 50;

    public Protocol.ToolDockState InitialState { get; set; } = Protocol.ToolDockState.Placed;
}
