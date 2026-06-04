using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ParameterBlockWriter
{
    private readonly IModbusTransport _transport;
    private readonly CommandMailbox _mailbox;
    private readonly int _toolIndex;

    public ParameterBlockWriter(IModbusTransport transport, CommandMailbox mailbox, int toolIndex)
    {
        _transport = transport;
        _mailbox = mailbox;
        _toolIndex = toolIndex;
    }

    public async Task WriteAsync(TighteningParameterTemplate template, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(template);
        ParameterBlockReader.ValidateParameterId(template.ParameterId);

        if (template.RawBlock.Length != ModbusRegisterMap.ParameterBlockWordCount)
            throw new ArgumentException($"Raw block must contain {ModbusRegisterMap.ParameterBlockWordCount} words.");

        template.ApplyCoreToRaw();

        await _transport.WriteMultipleAsync(
                ModbusRegisterMap.CommandData,
                template.RawBlock,
                cancellationToken)
            .ConfigureAwait(false);

        var req = CommandMailbox.CreateRequest(
            ModbusFunctionCodes.WriteParameter,
            word2: _toolIndex,
            word3: template.ParameterId);
        await _mailbox.SendCommandAsync(ModbusFunctionCodes.WriteParameter, req, cancellationToken)
            .ConfigureAwait(false);
    }
}
