using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;
using UDL.Delta.IemdSd.Session;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class TighteningCycleRunner
{
    private readonly IModbusTransport _transport;
    private readonly CommandMailbox _mailbox;
    private readonly IemdSdClientOptions _options;
    private readonly DeviceSession _session;
    private readonly ILogger _logger;

    public TighteningCycleRunner(
        IModbusTransport transport,
        CommandMailbox mailbox,
        IemdSdClientOptions options,
        DeviceSession session,
        ILogger? logger = null)
    {
        _transport = transport;
        _mailbox = mailbox;
        _options = options;
        _session = session;
        _logger = logger ?? NullLogger.Instance;
    }

    public Task<TighteningResult> RunAsync(TighteningTrigger trigger, CancellationToken cancellationToken) =>
        _session.RunAsync(ct => RunCoreAsync(trigger, ct), cancellationToken);

    private async Task<TighteningResult> RunCoreAsync(TighteningTrigger trigger, CancellationToken cancellationToken)
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

        var reportId = await ReadReportIdCoreAsync(cancellationToken).ConfigureAwait(false);

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

        throw new IemdSdCommunicationException("Controller not ready (0x1F52).");
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

        throw new IemdSdCommunicationException("DI bit0 not acknowledged.");
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
        var primaryAddr = _options.UseLegacyFinishRegister
            ? ModbusRegisterMap.TighteningResultLegacy
            : ModbusRegisterMap.TighteningFinish;
        var secondaryAddr = _options.UseLegacyFinishRegister
            ? ModbusRegisterMap.TighteningFinish
            : ModbusRegisterMap.TighteningResultLegacy;

        var timeoutMs = Math.Max(1_000, _options.TighteningCycleTimeoutMs);
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        var started = DateTime.UtcNow;
        var lastPrimary = 0;
        var lastSecondary = 0;
        var nextDebugUtc = started;

        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lastPrimary = await _transport.ReadSingleAsync(primaryAddr, cancellationToken).ConfigureAwait(false);
            lastSecondary = await _transport.ReadSingleAsync(secondaryAddr, cancellationToken).ConfigureAwait(false);

            var primaryHit = IsFinishValue(lastPrimary);
            var secondaryHit = IsFinishValue(lastSecondary);

            if (primaryHit || secondaryHit)
            {
                int hitAddr;
                int hitValue;
                if (primaryHit && secondaryHit && lastPrimary != lastSecondary)
                {
                    hitAddr = primaryAddr;
                    hitValue = lastPrimary;
                    _logger.LogWarning(
                        "Finish registers conflict: 0x{PrimaryAddr:X}={PrimaryValue} 0x{SecondaryAddr:X}={SecondaryValue}; using preferred 0x{ChosenAddr:X}={ChosenValue} (UseLegacyFinishRegister={UseLegacy})",
                        primaryAddr,
                        lastPrimary,
                        secondaryAddr,
                        lastSecondary,
                        hitAddr,
                        hitValue,
                        _options.UseLegacyFinishRegister);
                }
                else if (primaryHit)
                {
                    hitAddr = primaryAddr;
                    hitValue = lastPrimary;
                }
                else
                {
                    hitAddr = secondaryAddr;
                    hitValue = lastSecondary;
                }

                var elapsedMs = (int)(DateTime.UtcNow - started).TotalMilliseconds;
                _logger.LogInformation(
                    "Tightening finish detected: register 0x{Addr:X}={Value} after {ElapsedMs} ms (peer 0x{PeerAddr:X}={PeerValue})",
                    hitAddr,
                    hitValue,
                    elapsedMs,
                    hitAddr == primaryAddr ? secondaryAddr : primaryAddr,
                    hitAddr == primaryAddr ? lastSecondary : lastPrimary);

                return (DeviceTighteningStatus)hitValue;
            }

            if (_logger.IsEnabled(LogLevel.Debug) && DateTime.UtcNow >= nextDebugUtc)
            {
                _logger.LogDebug(
                    "Waiting for tightening finish: 0x{PrimaryAddr:X}={PrimaryValue} 0x{SecondaryAddr:X}={SecondaryValue} elapsedMs={ElapsedMs}",
                    primaryAddr,
                    lastPrimary,
                    secondaryAddr,
                    lastSecondary,
                    (int)(DateTime.UtcNow - started).TotalMilliseconds);
                nextDebugUtc = DateTime.UtcNow.AddSeconds(2);
            }

            await Task.Delay(_options.TighteningPollIntervalMs, cancellationToken).ConfigureAwait(false);
        }

        throw new IemdSdCommunicationException(
            $"Tightening finish wait timed out after {timeoutMs} ms " +
            $"(0x{primaryAddr:X}={lastPrimary}, 0x{secondaryAddr:X}={lastSecondary}).");
    }

    private static bool IsFinishValue(int value) => value is 1 or 2 or 5;

    private async Task ClearFinishAsync(CancellationToken cancellationToken)
    {
        // Align with supplier Demo: clear both new and legacy finish flags.
        await _transport.WriteSingleAsync(ModbusRegisterMap.TighteningFinish, 0, cancellationToken)
            .ConfigureAwait(false);
        await _transport.WriteSingleAsync(ModbusRegisterMap.TighteningResultLegacy, 0, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<uint> ReadReportIdAsync(CancellationToken cancellationToken) =>
        _session.RunAsync(ReadReportIdCoreAsync, cancellationToken);

    private async Task<uint> ReadReportIdCoreAsync(CancellationToken cancellationToken)
    {
        var row = await _transport.ReadHoldingAsync(ModbusRegisterMap.ReportIdLow, 2, cancellationToken)
            .ConfigureAwait(false);
        return (uint)(row[1] * 65536 + (ushort)row[0]);
    }
}
