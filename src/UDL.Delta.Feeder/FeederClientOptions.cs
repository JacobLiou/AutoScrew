namespace UDL.Delta.Feeder;

public enum FeederTransportType
{
    Stub,
}

public enum FeederSimulatedFailureMode
{
    None,
    Timeout,
    Empty,
    Jam,
}

public sealed class FeederClientOptions
{
    public FeederTransportType Transport { get; set; } = FeederTransportType.Stub;

    public string Host { get; set; } = "192.168.1.12";

    public int Port { get; set; } = 502;

    public string SerialPortName { get; set; } = "COM2";

    public int BaudRate { get; set; } = 115200;

    public byte ModbusSlaveId { get; set; } = 1;

    public int FeedTimeoutMs { get; set; } = 5000;

    public int SimulatedFeedDelayMs { get; set; } = 80;

    public FeederSimulatedFailureMode SimulatedFailureMode { get; set; } = FeederSimulatedFailureMode.None;
}
