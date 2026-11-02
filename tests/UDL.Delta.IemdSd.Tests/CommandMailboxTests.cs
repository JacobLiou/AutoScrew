using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Tests;

public class CommandMailboxTests
{
    [Fact]
    public async Task SendCommandAsync_AckLogsCommandAndSuccess()
    {
        var transport = new LoggingModbusTransportTests.FakeTransport { Data = [100, 1, 0] };
        var logger = new LoggingModbusTransportTests.RecordingLogger();
        var mailbox = new CommandMailbox(transport, new IemdSdClientOptions(), logger);
        await mailbox.SendCommandAsync(100, CommandMailbox.CreateRequest(100), default);
        Assert.Equal(2, logger.Events.Count);
        Assert.Contains("#100 Started", logger.Events[0].Message);
        Assert.Contains("#100 Success", logger.Events[1].Message);
        Assert.Equal(1, transport.LastPayload![6]);
    }

    [Fact]
    public async Task SendCommandAsync_RejectionLogsDeviceError()
    {
        var transport = new LoggingModbusTransportTests.FakeTransport { Data = [100, 2, 7] };
        var logger = new LoggingModbusTransportTests.RecordingLogger();
        var mailbox = new CommandMailbox(transport, new IemdSdClientOptions(), logger);
        var exception = await Assert.ThrowsAsync<IemdSdCommunicationException>(() =>
            mailbox.SendCommandAsync(100, CommandMailbox.CreateRequest(100), default));
        Assert.Equal(7, exception.DeviceErrorCode);
        Assert.Equal(LogLevel.Warning, logger.Events.Last().Level);
        Assert.Contains("DeviceErrorCode=7", logger.Events.Last().Message);
    }

    [Fact]
    public async Task SendCommandAsync_TimeoutLogsWarning()
    {
        var transport = new LoggingModbusTransportTests.FakeTransport { Data = [0, 0, 0] };
        var logger = new LoggingModbusTransportTests.RecordingLogger();
        var mailbox = new CommandMailbox(transport, new IemdSdClientOptions { CommandTimeoutMs = 1 }, logger);
        var exception = await Assert.ThrowsAsync<IemdSdCommunicationException>(() =>
            mailbox.SendCommandAsync(100, CommandMailbox.CreateRequest(100), default));
        Assert.Equal(100, exception.CommandCode);
        Assert.Equal(LogLevel.Warning, logger.Events.Last().Level);
        Assert.Contains("TimedOut", logger.Events.Last().Message);
    }

    [Fact]
    public async Task SendCommandAsync_CancelDoesNotLogWarning()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var logger = new LoggingModbusTransportTests.RecordingLogger();
        var mailbox = new CommandMailbox(new LoggingModbusTransportTests.FakeTransport(), new IemdSdClientOptions(), logger);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            mailbox.SendCommandAsync(100, CommandMailbox.CreateRequest(100), source.Token));
        Assert.DoesNotContain(logger.Events, entry => entry.Level >= LogLevel.Warning);
    }

    [Fact]
    public void CreateRequest_SetsCommandInFirstWord()
    {
        var req = CommandMailbox.CreateRequest(750, word2: 1, word3: 2);
        Assert.Equal(750, req[0]);
        Assert.Equal(1, req[2]);
        Assert.Equal(2, req[3]);
        Assert.Equal(0, req[6]);
    }

    [Fact]
    public void SetReportId_SplitsLowHighWords()
    {
        var req = CommandMailbox.CreateRequest(751);
        CommandMailbox.SetReportId(req, 70000);
        Assert.Equal(70000 % 65536, req[2]);
        Assert.Equal(70000 / 65536, req[3]);
        Assert.Equal(0, req[6]);
    }

    [Fact]
    public void CreateRequest_ReadParameter_Layout()
    {
        var req = CommandMailbox.CreateRequest(150, word2: 1, word3: 42);
        Assert.Equal(150, req[0]);
        Assert.Equal(0, req[1]);
        Assert.Equal(1, req[2]);
        Assert.Equal(42, req[3]);
    }

    [Fact]
    public void CreateRequest_WriteParameter_Layout()
    {
        var req = CommandMailbox.CreateRequest(100, word2: 1, word3: 99);
        Assert.Equal(100, req[0]);
        Assert.Equal(0, req[1]);
        Assert.Equal(1, req[2]);
        Assert.Equal(99, req[3]);
    }
}
