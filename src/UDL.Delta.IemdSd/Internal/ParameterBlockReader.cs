using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ParameterBlockReader
{
    private readonly IIemdSdCommandExecutor _executor;
    private readonly int _toolIndex;

    public ParameterBlockReader(IIemdSdCommandExecutor executor, int toolIndex)
    {
        _executor = executor;
        _toolIndex = toolIndex;
    }

    public async Task<TighteningParameterTemplate> ReadAsync(int parameterId, CancellationToken cancellationToken)
    {
        ValidateParameterId(parameterId);

        var result = await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithReadPayload(
                ModbusFunctionCodes.ReadParameter,
                ModbusRegisterMap.ParameterBlockWordCount,
                word2: _toolIndex,
                word3: parameterId),
            cancellationToken).ConfigureAwait(false);

        var words = result.ReadPayload
                      ?? throw new InvalidOperationException("Parameter read returned no payload.");

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
