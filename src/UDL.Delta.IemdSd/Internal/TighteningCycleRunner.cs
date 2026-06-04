using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class TighteningCycleRunner
{
    private readonly IModbusTransport _transport;
    private readonly CommandMailbox _mailbox;
    private readonly IemdSdClientOptions _options;

    public TighteningCycleRunner(IModbusTransport transport, CommandMailbox mailbox, IemdSdClientOptions options)
    {
        _transport = transport;
        _mailbox = mailbox;
        _options = options;
    }

    public async Task<TighteningResult> RunAsync(TighteningTrigger trigger, CancellationToken cancellationToken)
    {
        var useDi = trigger == TighteningTrigger.AutoDi
            || (_options.TriggerMode == TighteningTriggerMode.AutoDi && trigger != TighteningTrigger.Manual);

        if (useDi)
        {
            await WaitReadyAsync(cancellationToken).ConfigureAwait(false);
            await _transport.WriteSingleAsync(ModbusRegisterMap.DiCommand, 1, cancellationToken).ConfigureAwait(false);
            await WaitDiAckAsync(cancellationToken).ConfigureAwait(false);
        }

        var status = await WaitFinishAsync(cancellationToken).ConfigureAwait(false);

        var totalAngle = await _transport.ReadSingleAsync(ModbusRegisterMap.TotalAngle, cancellationToken)
            .ConfigureAwait(false);
        var torqueWords = await _transport.ReadHoldingAsync(ModbusRegisterMap.FinalTorqueLow, 2, cancellationToken)
            .ConfigureAwait(false);
        var dword = (uint)(torqueWords[1] * 65536 + (ushort)torqueWords[0]);
        var finalNm = dword / 1000.0;

        var reportId = await ReadReportIdAsync(cancellationToken).ConfigureAwait(false);

        if (useDi)
        {
            await _transport.WriteSingleAsync(ModbusRegisterMap.DiCommand, 0, cancellationToken).ConfigureAwait(false);
            await WaitDiClearAsync(cancellationToken).ConfigureAwait(false);
        }

        await ClearFinishAsync(cancellationToken).ConfigureAwait(false);

        if (_options.SendUnlockAfterCycle)
        {
            var unlock = CommandMailbox.CreateRequest(ModbusFunctionCodes.LimitTightening);
            await _mailbox.SendCommandAsync(ModbusFunctionCodes.LimitTightening, unlock, cancellationToken)
                .ConfigureAwait(false);
        }

        return new TighteningResult
        {
            Status = status,
            TotalAngle = totalAngle,
            FinalTorqueNm = finalNm,
            PrevailTorqueNm = finalNm,
            ReportId = reportId,
        };
    }

    private async Task WaitReadyAsync(CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(_options.CommandTimeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var ready = await _transport.ReadSingleAsync(ModbusRegisterMap.Ready, cancellationToken).ConfigureAwait(false);
            if (ready == 1)
                return;
            await Task.Delay(_options.TighteningPollIntervalMs, cancellationToken).ConfigureAwait(false);
        }

        throw new Exceptions.IemdSdCommunicationException("Controller not ready (0x1F52).");
    }

    private async Task WaitDiAckAsync(CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(_options.CommandTimeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var di = await _transport.ReadSingleAsync(ModbusRegisterMap.DiStatus, cancellationToken).ConfigureAwait(false);
            if ((di & 1) == 1)
                return;
            await Task.Delay(_options.TighteningPollIntervalMs, cancellationToken).ConfigureAwait(false);
        }

        throw new Exceptions.IemdSdCommunicationException("DI bit0 not acknowledged.");
    }

    private async Task WaitDiClearAsync(CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(_options.CommandTimeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var di = await _transport.ReadSingleAsync(ModbusRegisterMap.DiStatus, cancellationToken).ConfigureAwait(false);
            if ((di & 1) == 0)
                return;
            await Task.Delay(_options.TighteningPollIntervalMs, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<DeviceTighteningStatus> WaitFinishAsync(CancellationToken cancellationToken)
    {
        var addr = _options.UseLegacyFinishRegister
            ? ModbusRegisterMap.TighteningResultLegacy
            : ModbusRegisterMap.TighteningFinish;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var v = await _transport.ReadSingleAsync(addr, cancellationToken).ConfigureAwait(false);
            if (v is 1 or 2 or 5)
                return (DeviceTighteningStatus)v;
            await Task.Delay(_options.TighteningPollIntervalMs, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ClearFinishAsync(CancellationToken cancellationToken)
    {
        var addr = _options.UseLegacyFinishRegister
            ? ModbusRegisterMap.TighteningResultLegacy
            : ModbusRegisterMap.TighteningFinish;
        await _transport.WriteSingleAsync(addr, 0, cancellationToken).ConfigureAwait(false);
    }

    public async Task<uint> ReadReportIdAsync(CancellationToken cancellationToken)
    {
        var row = await _transport.ReadHoldingAsync(ModbusRegisterMap.ReportIdLow, 2, cancellationToken)
            .ConfigureAwait(false);
        return (uint)(row[1] * 65536 + (ushort)row[0]);
    }
}
