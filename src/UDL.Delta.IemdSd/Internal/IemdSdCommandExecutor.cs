using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Session;

namespace UDL.Delta.IemdSd.Internal;

internal interface IIemdSdCommandExecutor
{
    Task<ModbusCommandResult> ExecuteAsync(ModbusCommandInvocation invocation, CancellationToken cancellationToken);
}

/// <summary>
/// Mailbox transaction: WritePayload? → WriteMailbox(0xC8) → Poll(0xCF) → ReadPayload?
/// entirely under <see cref="DeviceSession"/> (no lock release between steps).
/// </summary>
internal sealed class IemdSdCommandExecutor : IIemdSdCommandExecutor
{
    private readonly IModbusTransport _transport;
    private readonly ICommandMailbox _mailbox;
    private readonly DeviceSession _session;

    public IemdSdCommandExecutor(IModbusTransport transport, ICommandMailbox mailbox, DeviceSession session)
    {
        _transport = transport;
        _mailbox = mailbox;
        _session = session;
    }

    public Task<ModbusCommandResult> ExecuteAsync(ModbusCommandInvocation invocation, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invocation);
        if (invocation.MailboxWords.Length != 10)
            throw new ArgumentException("MailboxWords must contain exactly 10 elements.", nameof(invocation));

        return _session.RunAsync(ct => ExecuteCoreAsync(invocation, ct), cancellationToken);
    }

    private async Task<ModbusCommandResult> ExecuteCoreAsync(
        ModbusCommandInvocation invocation,
        CancellationToken cancellationToken)
    {
        var mailbox = (int[])invocation.MailboxWords.Clone();
        mailbox[0] = invocation.FunctionCode;

        if (invocation.WritePayload is { Length: > 0 })
        {
            await _transport.WriteMultipleAsync(
                    ModbusRegisterMap.CommandData,
                    invocation.WritePayload,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        await _mailbox.SendCommandAsync(invocation.FunctionCode, mailbox, cancellationToken).ConfigureAwait(false);

        int[]? readPayload = null;
        if (invocation.ReadWordCount is > 0)
        {
            readPayload = await _transport.ReadHoldingAsync(
                    ModbusRegisterMap.CommandData,
                    (int)invocation.ReadWordCount.Value,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        int? readback = null;
        if (invocation.ReadbackFromCommandRequest)
            readback = await _transport.ReadSingleAsync(ModbusRegisterMap.CommandRequest, cancellationToken).ConfigureAwait(false);

        return new ModbusCommandResult
        {
            FunctionCode = invocation.FunctionCode,
            MailboxWords = mailbox,
            ReadPayload = readPayload,
            ReadbackValue = readback,
        };
    }
}
