using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Tests;

public class LoggingModbusTransportTests
{
    [Fact]
    public async Task Operations_DelegateOnceAndLogResults()
    {
        var inner = new FakeTransport();
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        await transport.ConnectAsync(default);
        Assert.True(transport.IsConnected);
        Assert.Equal(42, await transport.ReadSingleAsync(0x24, default));
        Assert.Same(inner.Data, await transport.ReadHoldingAsync(0xD2, 2, default));
        await transport.WriteSingleAsync(0x68, 1, default);
        await transport.WriteMultipleAsync(0xD2, inner.Data, default);
        transport.Invalidate();
        transport.Dispose();

        Assert.Equal(new[] { "Connect", "ReadSingle:36", "ReadHolding:210:2", "WriteSingle:104:1", "WriteMultiple:210", "Invalidate", "Dispose" }, inner.Calls);
        Assert.Equal(7, logger.Events.Count);
        Assert.All(logger.Events, entry => Assert.Equal(LogLevel.Information, entry.Level));
        Assert.Contains("Address=0x0024", logger.Events[1].Message);
        Assert.Contains("Data=Words=1 End=0x0024 Sha256=", logger.Events[1].Message);
        Assert.Contains("002A", logger.Events[1].Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    [InlineData(17)]
    public async Task Summary_DumpsAllWordsAndDoesNotMutatePayload(int count)
    {
        var inner = new FakeTransport();
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        var payload = Enumerable.Range(1, count).ToArray();
        var original = (int[])payload.Clone();
        await transport.WriteMultipleAsync(0xD2, payload, default);

        Assert.Equal(original, payload);
        Assert.Same(payload, inner.LastPayload);
        var message = logger.Events.Single().Message;
        Assert.DoesNotContain("truncated", message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains($"Words={count}", message);
        if (count == 0)
        {
            Assert.Contains("Data=[]", message);
            return;
        }

        Assert.Contains("@0000:", message);
        Assert.Contains(unchecked((ushort)count).ToString("X4"), message);
        if (count > 16)
        {
            Assert.Contains("@0010:", message);
            Assert.Contains("0011", message);
        }

        Assert.Contains($"Sha256={ModbusPayloadFormatter.ComputeSha256Hex(payload)}", message);
    }

    [Fact]
    public async Task Summary_LargeParameterBlock_IncludesTailWordAndOffsets()
    {
        var inner = new FakeTransport();
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        var payload = Enumerable.Range(0, ModbusRegisterMap.ParameterBlockWordCount).Select(i => i + 1).ToArray();
        await transport.WriteMultipleAsync(ModbusRegisterMap.CommandData, payload, default);

        var message = logger.Events.Single().Message;
        Assert.Contains($"Words={ModbusRegisterMap.ParameterBlockWordCount}", message);
        Assert.Contains($"End=0x{ModbusRegisterMap.ParameterBlockEnd:X4}", message);
        Assert.Contains("@0000:", message);
        Assert.Contains("@0010:", message);
        Assert.Contains(unchecked((ushort)ModbusRegisterMap.ParameterBlockWordCount).ToString("X4"), message);
        Assert.DoesNotContain("truncated", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WriteFailure_IncludesRequestPayload()
    {
        var failure = new IOException("offline");
        var inner = new FakeTransport { Failure = failure };
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        var payload = new[] { 0x11, 0x22, 0x33 };
        Assert.Same(failure, await Assert.ThrowsAsync<IOException>(() => transport.WriteMultipleAsync(0xD2, payload, default)));
        Assert.Equal(LogLevel.Warning, logger.Events.Single().Level);
        Assert.Same(failure, logger.Events.Single().Exception);
        var message = logger.Events.Single().Message;
        Assert.Contains("Words=3", message);
        Assert.Contains("0011", message);
        Assert.Contains("0022", message);
        Assert.Contains("0033", message);
        Assert.Contains($"Sha256={ModbusPayloadFormatter.ComputeSha256Hex(payload)}", message);
    }

    [Fact]
    public async Task Failure_PreservesExceptionAndLogsWarning()
    {
        var failure = new IOException("offline");
        var inner = new FakeTransport { Failure = failure };
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        Assert.Same(failure, await Assert.ThrowsAsync<IOException>(() => transport.ReadSingleAsync(0x24, default)));
        Assert.Equal(LogLevel.Warning, logger.Events.Single().Level);
        Assert.Same(failure, logger.Events.Single().Exception);
        Assert.DoesNotContain("Sha256=", logger.Events.Single().Message);
    }

    [Fact]
    public async Task Cancellation_IsNotLoggedAsFailure()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var failure = new OperationCanceledException(source.Token);
        var inner = new FakeTransport { Failure = failure };
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        Assert.Same(failure, await Assert.ThrowsAsync<OperationCanceledException>(() => transport.ReadSingleAsync(0x24, source.Token)));
        Assert.Equal(source.Token, inner.LastToken);
        Assert.Equal(LogLevel.Debug, logger.Events.Single().Level);
    }

    [Fact]
    public async Task DisabledInformation_DoesNotProduceSuccessSummary()
    {
        var inner = new FakeTransport();
        var logger = new RecordingLogger { Enabled = false };
        var transport = new LoggingModbusTransport(inner, new IemdSdClientOptions(), logger);
        await transport.WriteMultipleAsync(0xD2, new int[1000], default);
        Assert.Empty(logger.Events);
        Assert.Single(inner.Calls);
    }

    [Theory]
    [InlineData(ControllerTransportType.ModbusTcp, "TCP:192.168.0.10:502")]
    [InlineData(ControllerTransportType.ModbusRtu, "RTU:COM9@19200")]
    public async Task RecordsTransportEndpointAndSlave(ControllerTransportType type, string endpoint)
    {
        var logger = new RecordingLogger();
        var transport = new LoggingModbusTransport(new FakeTransport(), new IemdSdClientOptions
        {
            Transport = type,
            Host = "192.168.0.10",
            SerialPortName = "COM9",
            BaudRate = 19200,
            ModbusSlaveId = 3
        }, logger);
        await transport.ReadSingleAsync(0x24, default);
        Assert.Contains(endpoint, logger.Events.Single().Message);
        Assert.Contains("Slave=3", logger.Events.Single().Message);
    }

    internal sealed class RecordingLogger : ILogger
    {
        public bool Enabled { get; init; } = true;
        public List<(LogLevel Level, string Message, Exception? Exception)> Events { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => Enabled;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Events.Add((logLevel, formatter(state, exception), exception));
    }

    internal sealed class FakeTransport : IModbusTransport
    {
        public List<string> Calls { get; } = [];
        public int[] Data { get; set; } = [1, 2];
        public int[]? LastPayload { get; private set; }
        public Exception? Failure { get; init; }
        public CancellationToken LastToken { get; private set; }
        public bool IsConnected => true;
        public void Invalidate() => Calls.Add("Invalidate");
        public void Dispose() => Calls.Add("Dispose");
        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            Calls.Add("Connect");
            return Task.CompletedTask;
        }
        public Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken)
        {
            Calls.Add($"ReadSingle:{address}");
            LastToken = cancellationToken;
            return Failure is null ? Task.FromResult(42) : Task.FromException<int>(Failure);
        }
        public Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken)
        {
            Calls.Add($"ReadHolding:{address}:{count}");
            return Failure is null ? Task.FromResult(Data) : Task.FromException<int[]>(Failure);
        }
        public Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken)
        {
            Calls.Add($"WriteSingle:{address}:{value}");
            return Failure is null ? Task.CompletedTask : Task.FromException(Failure);
        }
        public Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken)
        {
            Calls.Add($"WriteMultiple:{address}");
            LastPayload = values;
            return Failure is null ? Task.CompletedTask : Task.FromException(Failure);
        }
    }
}
