using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ParameterBlockReader
{
    private readonly IModbusTransport _transport;
    private readonly CommandMailbox _mailbox;
    private readonly int _toolIndex;

    public ParameterBlockReader(IModbusTransport transport, CommandMailbox mailbox, int toolIndex)
    {
        _transport = transport;
        _mailbox = mailbox;
        _toolIndex = toolIndex;
    }

    public async Task<TighteningParameterTemplate> ReadAsync(int parameterId, CancellationToken cancellationToken)
    {
        ValidateParameterId(parameterId);

        var req = CommandMailbox.CreateRequest(
            ModbusFunctionCodes.ReadParameter,
            word2: _toolIndex,
            word3: parameterId);
        await _mailbox.SendCommandAsync(ModbusFunctionCodes.ReadParameter, req, cancellationToken)
            .ConfigureAwait(false);

        var words = await _transport.ReadHoldingAsync(
                ModbusRegisterMap.CommandData,
                ModbusRegisterMap.ParameterBlockWordCount,
                cancellationToken)
            .ConfigureAwait(false);

        var template = new TighteningParameterTemplate
        {
            ParameterId = parameterId,
            ToolIndex = _toolIndex,
            RawBlock = words,
        };
        template.SyncCoreFromRaw();
        return template;
    }

    internal static void ValidateParameterId(int parameterId)
    {
        if (parameterId is < 1 or > 500)
            throw new ArgumentOutOfRangeException(nameof(parameterId), "Parameter ID must be 1-500.");
    }
}
