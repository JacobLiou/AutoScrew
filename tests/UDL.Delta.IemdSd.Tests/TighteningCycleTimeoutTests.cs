using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;
using UDL.Delta.IemdSd.Session;

namespace UDL.Delta.IemdSd.Tests;

public sealed class TighteningCycleTimeoutTests
{
    [Fact]
    public async Task WaitFinish_ThrowsAfterConfiguredTimeout()
    {
        var options = CreateOptions(timeoutMs: 250);
        await using var session = new DeviceSession();
        var transport = new FinishProbeTransport();
        var mailbox = new CommandMailbox(transport, options, NullLogger.Instance);
        var runner = new TighteningCycleRunner(transport, mailbox, options, session, NullLogger.Instance);

        var ex = await Assert.ThrowsAsync<IemdSdCommunicationException>(
            () => runner.RunAsync(TighteningTrigger.Manual, CancellationToken.None));

        Assert.Contains("timed out", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("0x1F5D", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("0x26", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(session.IsBusy);
    }

    [Fact]
    public async Task StaleLegacyFinish_DoesNotInstantOk_WhenPrimaryPreferred()
    {
        var options = CreateOptions(timeoutMs: 250, useLegacy: false);
        await using var session = new DeviceSession();
        var transport = new FinishProbeTransport { LegacyFinish = 1 };
        var mailbox = new CommandMailbox(transport, options, NullLogger.Instance);
        var runner = new TighteningCycleRunner(transport, mailbox, options, session, NullLogger.Instance);

        var ex = await Assert.ThrowsAsync<IemdSdCommunicationException>(
            () => runner.RunAsync(TighteningTrigger.Manual, CancellationToken.None));

        Assert.Contains("timed out", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(ModbusRegisterMap.TighteningFinish, transport.ClearedAddresses);
        Assert.Contains(ModbusRegisterMap.TighteningResultLegacy, transport.ClearedAddresses);
        Assert.Equal(0, transport.LegacyFinish);
        Assert.False(session.IsBusy);
    }

    [Fact]
    public async Task WaitFinish_OnlyPeerRegister_TimesOut_WhenLegacyPreferred()
    {
        var options = CreateOptions(timeoutMs: 250, useLegacy: true);
        await using var session = new DeviceSession();
        // Peer (new) would look finished, but primary is legacy — must not succeed from peer alone.
        var transport = new FinishProbeTransport
        {
            FinishAfterPreClear = 1,
        };
        var mailbox = new CommandMailbox(transport, options, NullLogger.Instance);
        var runner = new TighteningCycleRunner(transport, mailbox, options, session, NullLogger.Instance);

        var ex = await Assert.ThrowsAsync<IemdSdCommunicationException>(
            () => runner.RunAsync(TighteningTrigger.Manual, CancellationToken.None));

        Assert.Contains("timed out", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitFinish_SucceedsOnPrimary_AfterPreClear()
    {
        var options = CreateOptions(useLegacy: false);
        await using var session = new DeviceSession();
        var transport = new FinishProbeTransport
        {
            LegacyFinish = 2, // stale peer must be ignored after clear / primary-only
            FinishAfterPreClear = 1,
        };
        var mailbox = new CommandMailbox(transport, options, NullLogger.Instance);
        var runner = new TighteningCycleRunner(transport, mailbox, options, session, NullLogger.Instance);

        var result = await runner.RunAsync(TighteningTrigger.Manual, CancellationToken.None);

        Assert.Equal(DeviceTighteningStatus.Ok, result.Status);
        Assert.Contains(ModbusRegisterMap.TighteningFinish, transport.ClearedAddresses);
        Assert.Contains(ModbusRegisterMap.TighteningResultLegacy, transport.ClearedAddresses);
        Assert.Equal(0, transport.Finish);
        Assert.Equal(0, transport.LegacyFinish);
    }

    [Fact]
    public async Task WaitFinish_SucceedsOnLegacyPrimary_AfterPreClear()
    {
        var options = CreateOptions(useLegacy: true);
        await using var session = new DeviceSession();
        var transport = new FinishProbeTransport
        {
            Finish = 1, // peer stale before clear
            LegacyAfterPreClear = 2,
        };
        var mailbox = new CommandMailbox(transport, options, NullLogger.Instance);
        var runner = new TighteningCycleRunner(transport, mailbox, options, session, NullLogger.Instance);

        var result = await runner.RunAsync(TighteningTrigger.Manual, CancellationToken.None);

        Assert.Equal(DeviceTighteningStatus.Ng, result.Status);
        Assert.Contains(ModbusRegisterMap.TighteningFinish, transport.ClearedAddresses);
        Assert.Contains(ModbusRegisterMap.TighteningResultLegacy, transport.ClearedAddresses);
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

    private static IemdSdClientOptions CreateOptions(int timeoutMs = 2_000, bool useLegacy = false) =>
        new()
        {
            TighteningCycleTimeoutMs = timeoutMs,
            TighteningPollIntervalMs = 20,
            TriggerMode = TighteningTriggerMode.Manual,
            SendUnlockAfterCycle = false,
            UseLegacyFinishRegister = useLegacy,
        };

    /// <summary>
    /// Simulates device Finish registers. Pre-wait ClearFinish zeroes both;
    /// optional AfterPreClear values inject the real cycle result after the first clear pair.
    /// </summary>
    private sealed class FinishProbeTransport : IModbusTransport
    {
        private bool _preClearDone;

        public int Finish { get; set; }
        public int LegacyFinish { get; set; }
        public int? FinishAfterPreClear { get; set; }
        public int? LegacyAfterPreClear { get; set; }
        public List<int> ClearedAddresses { get; } = new();

        public bool IsConnected => true;

        public void Invalidate()
        {
        }

        public Task ConnectAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<int> ReadSingleAsync(int address, CancellationToken cancellationToken) =>
            Task.FromResult(address switch
            {
                ModbusRegisterMap.TighteningFinish => Finish,
                ModbusRegisterMap.TighteningResultLegacy => LegacyFinish,
                ModbusRegisterMap.TotalAngle => 100,
                _ => 0,
            });

        public Task<int[]> ReadHoldingAsync(int address, int count, CancellationToken cancellationToken)
        {
            var values = new int[count];
            if (address == ModbusRegisterMap.FinalTorqueLow && count >= 2)
            {
                values[0] = 250;
                values[1] = 0;
            }

            return Task.FromResult(values);
        }

        public Task WriteSingleAsync(int address, int value, CancellationToken cancellationToken)
        {
            if (address == ModbusRegisterMap.TighteningFinish)
            {
                Finish = value;
                if (value == 0)
                    ClearedAddresses.Add(address);
            }
            else if (address == ModbusRegisterMap.TighteningResultLegacy)
            {
                LegacyFinish = value;
                if (value == 0)
                    ClearedAddresses.Add(address);
            }

            // After the first dual-clear (pre-wait), inject the cycle finish like a real trigger.
            if (!_preClearDone
                && ClearedAddresses.Count(a => a == ModbusRegisterMap.TighteningFinish) >= 1
                && ClearedAddresses.Count(a => a == ModbusRegisterMap.TighteningResultLegacy) >= 1)
            {
                _preClearDone = true;
                if (FinishAfterPreClear is int f)
                    Finish = f;
                if (LegacyAfterPreClear is int l)
                    LegacyFinish = l;
            }

            return Task.CompletedTask;
        }

        public Task WriteMultipleAsync(int address, int[] values, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public void Dispose()
        {
        }
    }
}
