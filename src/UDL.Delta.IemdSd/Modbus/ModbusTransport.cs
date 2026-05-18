using EasyModbus;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Exceptions;

namespace UDL.Delta.IemdSd.Modbus;

internal sealed class ModbusTransport : IDisposable
{
    private readonly ModbusClient _client;
    private readonly ILogger _logger;
    private readonly int _readWindowSize;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public ModbusTransport(IemdSdClientOptions options, ILogger logger)
    {
        _logger = logger;
        _readWindowSize = options.ReadWindowSize;
        _client = new ModbusClient(options.Host, options.Port);
    }

    public bool IsConnected => _client.Connected;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_client.Connected)
                return;
            await Task.Run(() => _client.Connect(), cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("IEMD-SD Modbus connected to {Host}:{Port}", _client.IPAddress, _client.Port);
        }
        catch (Exception ex)
        {
            throw new IemdSdCommunicationException("Modbus connect failed.", ex);
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Disconnect()
    {
        _gate.Wait();
        try
        {
            if (_client.Connected)
                _client.Disconnect();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken)
    {
        var data = await ReadHoldingAsync(address, 1, cancellationToken).ConfigureAwait(false);
        return data[0];
    }

    public async Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await Task.Run(() => ReadHoldingCore(address, count), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await Task.Run(() => _client.WriteSingleRegister(address, value), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new IemdSdCommunicationException($"Write register 0x{address:X} failed.", ex);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await Task.Run(() => WriteMultipleCore(address, values), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new IemdSdCommunicationException($"Write registers 0x{address:X} len={values.Length} failed.", ex);
        }
        finally
        {
            _gate.Release();
        }
    }

    private int[] ReadHoldingCore(int address, int count)
    {
        var result = new int[count];
        for (var offset = 0; offset < count; offset += _readWindowSize)
        {
            var chunk = Math.Min(_readWindowSize, count - offset);
            var data = _client.ReadHoldingRegisters(address + offset, chunk);
            Array.Copy(data, 0, result, offset, chunk);
        }

        return result;
    }

    private void WriteMultipleCore(int address, int[] values)
    {
        for (var offset = 0; offset < values.Length; offset += _readWindowSize)
        {
            var chunk = Math.Min(_readWindowSize, values.Length - offset);
            var slice = new int[chunk];
            Array.Copy(values, offset, slice, 0, chunk);
            _client.WriteMultipleRegisters(address + offset, slice);
        }
    }

    public void Dispose()
    {
        _gate.Wait();
        try
        {
            Disconnect();
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }
}
