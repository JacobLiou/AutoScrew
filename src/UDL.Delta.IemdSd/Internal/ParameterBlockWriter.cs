using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Internal;

internal sealed class ParameterBlockWriter
{
    private readonly IIemdSdCommandExecutor _executor;
    private readonly int _toolIndex;

    public ParameterBlockWriter(IIemdSdCommandExecutor executor, int toolIndex)
    {
        _executor = executor;
        _toolIndex = toolIndex;
    }

    public async Task WriteAsync(TighteningParameterTemplate template, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(template);
        ParameterBlockReader.ValidateParameterId(template.ParameterId);

        if (template.RawBlock.Length != ModbusRegisterMap.ParameterBlockWordCount)
            throw new ArgumentException($"Raw block must contain {ModbusRegisterMap.ParameterBlockWordCount} words.");

        template.ApplyCoreToRaw();

        await _executor.ExecuteAsync(
            ModbusCommandInvocation.WithWritePayload(
                ModbusFunctionCodes.WriteParameter,
                template.RawBlock,
                word2: _toolIndex,
                word3: template.ParameterId),
            cancellationToken).ConfigureAwait(false);
    }
}
