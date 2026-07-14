namespace UDL.Delta.IemdSd.Modbus;

internal interface IModbusTransport : IDisposable
{
    bool IsConnected { get; }

    /// <summary>Force-drop local TCP/serial after IO failure so callers must reconnect.</summary>
    void Invalidate();

    Task ConnectAsync(CancellationToken cancellationToken);

    Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken);

    Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken);

    Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken);

    Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken);
}
