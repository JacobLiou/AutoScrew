using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace UDL.Delta.IemdSd.Modbus;

internal sealed class LoggingModbusTransport : IModbusTransport
{
    private readonly IModbusTransport _inner;
    private readonly ILogger _logger;
    private readonly string _endpoint;
    private readonly byte _slaveId;

    public LoggingModbusTransport(IModbusTransport inner, IemdSdClientOptions options, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
        _endpoint = options.Transport == ControllerTransportType.ModbusRtu
            ? $"RTU:{options.SerialPortName}@{options.BaudRate}"
            : $"TCP:{options.Host}:{options.Port}";
        _slaveId = options.ModbusSlaveId;
    }

    public bool IsConnected => _inner.IsConnected;

    public Task ConnectAsync(CancellationToken cancellationToken) =>
        ExecuteWriteAsync("Connect", 0, 0, null, () => _inner.ConnectAsync(cancellationToken));

    public Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken) =>
        ExecuteAsync("ReadSingle", address, 1, () => _inner.ReadSingleAsync(address, cancellationToken), value => new[] { value });

    public Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken) =>
        ExecuteAsync("ReadHolding", address, count, () => _inner.ReadHoldingAsync(address, count, cancellationToken), values => values);

    public Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken) =>
        ExecuteWriteAsync("WriteSingle", address, 1, () => new[] { value }, () => _inner.WriteSingleAsync(address, value, cancellationToken));

    public Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken) =>
        ExecuteWriteAsync("WriteMultiple", address, values.Length, () => values, () => _inner.WriteMultipleAsync(address, values, cancellationToken));

    public void Invalidate() => ExecuteLifecycle("Invalidate", _inner.Invalidate);

    public void Dispose() => ExecuteLifecycle("Dispose", _inner.Dispose);

    private async Task ExecuteWriteAsync(string operation, int address, int count, Func<int[]>? data, Func<Task> action)
    {
        await ExecuteAsync(
            operation,
            address,
            count,
            async () =>
            {
                await action().ConfigureAwait(false);
                return true;
            },
            _ => data?.Invoke(),
            requestData: data).ConfigureAwait(false);
    }

    private async Task<T> ExecuteAsync<T>(
        string operation,
        int address,
        int count,
        Func<Task<T>> action,
        Func<T, int[]?> resultData,
        Func<int[]?>? requestData = null)
    {
        var started = Stopwatch.GetTimestamp();
        try
        {
            var result = await action().ConfigureAwait(false);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    "COMM {Endpoint} Slave={SlaveId} {Operation} Address=0x{Address:X4} Count={Count} Data={Data} Success ElapsedMs={ElapsedMs:F3}",
                    _endpoint,
                    _slaveId,
                    operation,
                    address,
                    count,
                    ModbusPayloadFormatter.Format(address, resultData(result)),
                    Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug(
                "COMM {Endpoint} Slave={SlaveId} {Operation} Address=0x{Address:X4} Count={Count} Cancelled",
                _endpoint,
                _slaveId,
                operation,
                address,
                count);
            throw;
        }
        catch (Exception exception)
        {
            var request = requestData?.Invoke();
            if (request is not null)
            {
                _logger.LogWarning(
                    exception,
                    "COMM {Endpoint} Slave={SlaveId} {Operation} Address=0x{Address:X4} Count={Count} Data={Data} Failed ElapsedMs={ElapsedMs:F3}",
                    _endpoint,
                    _slaveId,
                    operation,
                    address,
                    count,
                    ModbusPayloadFormatter.Format(address, request),
                    Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    exception,
                    "COMM {Endpoint} Slave={SlaveId} {Operation} Address=0x{Address:X4} Count={Count} Failed ElapsedMs={ElapsedMs:F3}",
                    _endpoint,
                    _slaveId,
                    operation,
                    address,
                    count,
                    Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            }

            throw;
        }
    }

    private void ExecuteLifecycle(string operation, Action action)
    {
        try
        {
            action();
            _logger.LogInformation("COMM {Endpoint} Slave={SlaveId} {Operation} Success", _endpoint, _slaveId, operation);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "COMM {Endpoint} Slave={SlaveId} {Operation} Failed", _endpoint, _slaveId, operation);
            throw;
        }
    }
}
