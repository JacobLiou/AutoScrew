using Microsoft.Extensions.Logging;
using NModbus;
using NModbus.IO;
using System.IO.Ports;
using UDL.Delta.IemdSd.Exceptions;

namespace UDL.Delta.IemdSd.Modbus;

internal sealed class ModbusRtuTransport : IModbusTransport
{
    private readonly ILogger _logger;
    private readonly IemdSdClientOptions _options;
    private readonly int _readWindowSize;
    private readonly int _interFrameDelayMs;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private SerialPort? _serialPort;
    private IModbusMaster? _master;

    public ModbusRtuTransport(IemdSdClientOptions options, ILogger logger)
    {
        _options = options;
        _logger = logger;
        _readWindowSize = options.ReadWindowSize;
        _interFrameDelayMs = Math.Max(0, options.RtuInterFrameDelayMs);
    }

    public bool IsConnected => _serialPort?.IsOpen == true;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_serialPort?.IsOpen == true)
                return;

            _master?.Dispose();
            _serialPort?.Dispose();

            _serialPort = new SerialPort(
                _options.SerialPortName,
                _options.BaudRate,
                ModbusTransportFactory.ParseParity(_options.Parity),
                _options.DataBits,
                ModbusTransportFactory.ParseStopBits(_options.StopBits))
            {
                ReadTimeout = _options.CommandTimeoutMs,
                WriteTimeout = _options.CommandTimeoutMs,
            };
            _serialPort.Open();

            var factory = new ModbusFactory();
            var stream = new SerialPortStreamResource(_serialPort);
            _master = factory.CreateRtuMaster(stream);
            _logger.LogInformation(
                "IEMD-SD Modbus RTU connected on {Port} @ {Baud}",
                _options.SerialPortName,
                _options.BaudRate);
        }
        catch (Exception ex)
        {
            throw new IemdSdCommunicationException($"Modbus RTU connect failed on {_options.SerialPortName}.", ex);
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
            await _master!.WriteSingleRegisterAsync(_options.ModbusSlaveId, (ushort)address, (ushort)value)
                .ConfigureAwait(false);
            await ApplyInterFrameDelayAsync(cancellationToken).ConfigureAwait(false);
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
            var data = await _master!.ReadHoldingRegistersAsync(
                    _options.ModbusSlaveId,
                    (ushort)(address + offset),
                    chunk)
                .ConfigureAwait(false);
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
            await _master!.WriteMultipleRegistersAsync(_options.ModbusSlaveId, (ushort)(address + offset), slice)
                .ConfigureAwait(false);
            await ApplyInterFrameDelayAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private Task ApplyInterFrameDelayAsync(CancellationToken cancellationToken) =>
        _interFrameDelayMs > 0
            ? Task.Delay(_interFrameDelayMs, cancellationToken)
            : Task.CompletedTask;

    private void EnsureMaster()
    {
        if (_master is null || _serialPort?.IsOpen != true)
            throw new IemdSdCommunicationException("Modbus RTU not connected.");
    }

    public void Dispose()
    {
        _gate.Wait();
        try
        {
            _master?.Dispose();
            _master = null;
            if (_serialPort?.IsOpen == true)
                _serialPort.Close();
            _serialPort?.Dispose();
            _serialPort = null;
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }
}
