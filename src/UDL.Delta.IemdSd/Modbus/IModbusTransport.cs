namespace UDL.Delta.IemdSd.Modbus;

internal interface IModbusTransport : IDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken);

    Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken);

    Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken);

    Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken);
}
