using Microsoft.Extensions.Logging;
using NModbus;
using System.Net.Sockets;
using UDL.Delta.IemdSd.Exceptions;

namespace UDL.Delta.IemdSd.Modbus;

internal sealed class ModbusTransport : IDisposable
{
    private readonly string _host;
    private readonly int _port;
    private readonly ILogger _logger;
    private readonly int _readWindowSize;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private TcpClient? _tcpClient;
    private IModbusMaster? _master;
    private const byte SlaveId = 1;

    public ModbusTransport(IemdSdClientOptions options, ILogger logger)
    {
        _logger = logger;
        _host = options.Host;
        _port = options.Port;
        _readWindowSize = options.ReadWindowSize;
    }

    public bool IsConnected => _tcpClient?.Connected == true;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_tcpClient?.Connected == true)
                return;

            _master?.Dispose();
            _tcpClient?.Dispose();

            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_host, _port, cancellationToken).ConfigureAwait(false);
            var factory = new ModbusFactory();
            _master = factory.CreateMaster(_tcpClient);
            _logger.LogInformation("IEMD-SD Modbus connected to {Host}:{Port}", _host, _port);
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
            _master?.Dispose();
            _master = null;
            _tcpClient?.Dispose();
            _tcpClient = null;
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
            return await ReadHoldingCoreAsync(address, count, cancellationToken).ConfigureAwait(false);
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
            EnsureMaster();
            await _master!.WriteSingleRegisterAsync(SlaveId, (ushort)address, (ushort)value).ConfigureAwait(false);
        }
        catch (IemdSdCommunicationException)
        {
            throw;
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
            await WriteMultipleCoreAsync(address, values, cancellationToken).ConfigureAwait(false);
        }
        catch (IemdSdCommunicationException)
        {
            throw;
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

    private async Task<int[]> ReadHoldingCoreAsync(int address, int count, CancellationToken cancellationToken)
    {
        EnsureMaster();
        var result = new int[count];
        for (var offset = 0; offset < count; offset += _readWindowSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var chunk = (ushort)Math.Min(_readWindowSize, count - offset);
            var data = await _master!.ReadHoldingRegistersAsync(SlaveId, (ushort)(address + offset), chunk).ConfigureAwait(false);
            for (var i = 0; i < chunk; i++)
                result[offset + i] = data[i];
        }
        return result;
    }

    private async Task WriteMultipleCoreAsync(int address, int[] values, CancellationToken cancellationToken)
    {
        EnsureMaster();
        for (var offset = 0; offset < values.Length; offset += _readWindowSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var chunk = Math.Min(_readWindowSize, values.Length - offset);
            var slice = new ushort[chunk];
            for (var i = 0; i < chunk; i++)
                slice[i] = (ushort)values[offset + i];
            await _master!.WriteMultipleRegistersAsync(SlaveId, (ushort)(address + offset), slice).ConfigureAwait(false);
        }
    }

    private void EnsureMaster()
    {
        if (_master is null || _tcpClient?.Connected != true)
            throw new IemdSdCommunicationException("Modbus not connected.");
    }

    public void Dispose()
    {
        _gate.Wait();
        try
        {
            _master?.Dispose();
            _master = null;
            _tcpClient?.Dispose();
            _tcpClient = null;
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }
}
