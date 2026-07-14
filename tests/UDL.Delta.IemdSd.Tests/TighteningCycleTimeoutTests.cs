using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Session;

namespace UDL.Delta.IemdSd.Tests;

public sealed class TighteningCycleTimeoutTests
{
    [Fact]
    public async Task WaitFinish_ThrowsAfterConfiguredTimeout()
    {
        var options = new IemdSdClientOptions
        {
            TighteningCycleTimeoutMs = 250,
            TighteningPollIntervalMs = 30,
            TriggerMode = TighteningTriggerMode.Manual,
            SendUnlockAfterCycle = false,
        };
        await using var session = new DeviceSession();
        var transport = new NeverFinishTransport();
        var mailbox = new CommandMailbox(transport, options, NullLogger.Instance);
        var runner = new TighteningCycleRunner(transport, mailbox, options, session);

        var ex = await Assert.ThrowsAsync<IemdSdCommunicationException>(
            () => runner.RunAsync(TighteningTrigger.Manual, CancellationToken.None));

        Assert.Contains("timed out", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(session.IsBusy);
    }

    [Fact]
    public async Task BusySession_RejectsParallelMailboxUntilReleased()
    {
        await using var session = new DeviceSession();
        var gateOpened = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowFinish = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var holder = session.RunAsync(async ct =>
        {
            gateOpened.TrySetResult();
            await allowFinish.Task.WaitAsync(ct);
        }, CancellationToken.None);

        await gateOpened.Task;
        Assert.True(session.IsBusy);

        var contested = session.RunAsync(_ => Task.FromResult(42), CancellationToken.None);
        Assert.False(contested.IsCompleted);

        allowFinish.TrySetResult();
        await holder;
        Assert.Equal(42, await contested);
        Assert.False(session.IsBusy);
    }

    private sealed class NeverFinishTransport : IModbusTransport
    {
        public bool IsConnected => true;

        public void Invalidate()
        {
        }

        public Task ConnectAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken) =>
            Task.FromResult(0);

        public Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken) =>
            Task.FromResult(new int[count]);

        public Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public void Dispose()
        {
        }
    }
}
