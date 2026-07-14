using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Session;

namespace UDL.Delta.IemdSd.Tests;

public sealed class DeviceSessionTests
{
    [Fact]
    public async Task RunAsync_SerializesConcurrentMailboxTransactions()
    {
        await using var session = new DeviceSession();
        var transport = new InterleavedTransport();
        var mailbox = new FakeMailbox(transport);
        var executor = new IemdSdCommandExecutor(transport, mailbox, session);

        var t1 = executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(150, 2, word3: 1),
            CancellationToken.None);
        var t2 = executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(160, 2, word3: 0),
            CancellationToken.None);

        await Task.WhenAll(t1, t2);

        Assert.Null(transport.InterleaveFailure);
        Assert.Equal(2, transport.CompletedTransactions);
    }

    [Fact]
    public async Task RunAsync_Reentrancy_AllowsNestedCallsOnSameFlow()
    {
        await using var session = new DeviceSession();
        var depth = 0;
        var maxDepth = 0;

        await session.RunAsync(async ct =>
        {
            depth++;
            maxDepth = Math.Max(maxDepth, depth);
            await session.RunAsync(async _ =>
            {
                depth++;
                maxDepth = Math.Max(maxDepth, depth);
                depth--;
                await Task.CompletedTask;
            }, ct);
            depth--;
        }, CancellationToken.None);

        Assert.Equal(2, maxDepth);
        Assert.False(session.IsBusy);
    }

    private sealed class InterleavedTransport : IModbusTransport
    {
        private int _active;
        public string? InterleaveFailure { get; private set; }
        public int CompletedTransactions { get; private set; }

        public bool IsConnected => true;

        public void Invalidate()
        {
        }

        public Task ConnectAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken) =>
            Task.FromResult(0);

        public async Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _active) > 1)
                InterleaveFailure ??= "concurrent holding read";
            await Task.Delay(20, cancellationToken).ConfigureAwait(false);
            Interlocked.Decrement(ref _active);
            return Enumerable.Repeat(1, count).ToArray();
        }

        public Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public async Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _active) > 1)
                InterleaveFailure ??= "concurrent write";
            await Task.Delay(20, cancellationToken).ConfigureAwait(false);
            Interlocked.Decrement(ref _active);
        }

        public void Dispose()
        {
        }

        public void MarkTransactionComplete() => CompletedTransactions++;
    }

    private sealed class FakeMailbox : ICommandMailbox
    {
        private readonly InterleavedTransport _transport;

        public FakeMailbox(InterleavedTransport transport) => _transport = transport;

        public async Task SendCommandAsync(int commandCode, int[] requestWords, CancellationToken cancellationToken)
        {
            await _transport.WriteMultipleAsync(ModbusRegisterMap.CommandRequest, requestWords, cancellationToken)
                .ConfigureAwait(false);
            await Task.Delay(15, cancellationToken).ConfigureAwait(false);
            await _transport.ReadHoldingAsync(ModbusRegisterMap.CommandResponse, 3, cancellationToken)
                .ConfigureAwait(false);
            _transport.MarkTransactionComplete();
        }
    }
}
