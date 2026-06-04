using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningParameterTemplate
{
    public int ParameterId { get; set; } = 1;
    public int ToolIndex { get; set; }
    public int[] RawBlock { get; set; } = CreateEmptyRawBlock();
    public TighteningParameterCore Core { get; set; } = new();

    public static int[] CreateEmptyRawBlock() =>
        new int[ModbusRegisterMap.ParameterBlockWordCount];

    public void SyncCoreFromRaw() => Core = TighteningParameterCodec.ExtractCoreFromRaw(RawBlock);

    public void ApplyCoreToRaw() => TighteningParameterCodec.ApplyCoreToRaw(RawBlock, Core);
}
