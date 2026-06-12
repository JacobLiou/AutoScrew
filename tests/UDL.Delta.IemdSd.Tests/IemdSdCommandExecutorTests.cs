using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Tests;

public class IemdSdCommandExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_WriteThenMailbox_SendsPayloadBeforeMailbox()
    {
        var transport = new RecordingTransport();
        var mailbox = new FakeMailbox();
        var executor = new IemdSdCommandExecutor(transport, mailbox);

        var payload = Enumerable.Repeat(7, ModbusRegisterMap.ParameterBlockWordCount).ToArray();
        await executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(100, payload, word2: 1, word3: 42),
            CancellationToken.None);

        Assert.Equal(["write-payload"], transport.Steps);
        Assert.Equal(100, mailbox.LastCommand);
        Assert.Equal(1, mailbox.LastRequest[2]);
        Assert.Equal(42, mailbox.LastRequest[3]);
    }

    [Fact]
    public async Task ExecuteAsync_MailboxThenRead_ReadsDataBlockAfterAck()
    {
        var transport = new RecordingTransport();
        var mailbox = new FakeMailbox();
        var executor = new IemdSdCommandExecutor(transport, mailbox);

        var result = await executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(150, 4, word2: 0, word3: 9),
            CancellationToken.None);

        Assert.Equal(["read"], transport.Steps);
        Assert.Equal(150, mailbox.LastCommand);
        Assert.Equal(4, result.ReadPayload?.Length);
    }

    private sealed class RecordingTransport : IModbusTransport
    {
        public List<string> Steps { get; } = [];

        public Task ConnectAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken)
        {
            Steps.Add("read-single");
            return Task.FromResult(0);
        }

        public Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken)
        {
            Steps.Add("read");
            return Task.FromResult(Enumerable.Repeat(1, count).ToArray());
        }

        public Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken)
        {
            Steps.Add("write-single");
            return Task.CompletedTask;
        }

        public Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken)
        {
            Steps.Add("write-payload");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class FakeMailbox : ICommandMailbox
    {
        public int LastCommand { get; private set; }

        public int[] LastRequest { get; private set; } = [];

        public Task SendCommandAsync(int commandCode, int[] requestWords, CancellationToken cancellationToken)
        {
            LastCommand = commandCode;
            LastRequest = (int[])requestWords.Clone();
            return Task.CompletedTask;
        }
    }
}
