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

    public void SyncCoreFromRaw(IemdSdControllerProfile profile = IemdSdControllerProfile.Sd3_101) =>
        Core = TighteningParameterCodec.ExtractCoreFromRaw(RawBlock, profile);

    public void ApplyCoreToRaw(IemdSdControllerProfile profile = IemdSdControllerProfile.Sd3_101) =>
        TighteningParameterCodec.ApplyCoreToRaw(RawBlock, Core, profile);
}
