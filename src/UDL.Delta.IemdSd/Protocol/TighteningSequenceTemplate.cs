namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#200/#250 sequence block (raw words at 0xD2; layout per manual A.3.2, 0xD2–0x2E3).</summary>
public sealed class TighteningSequenceTemplate
{
    public int SequenceId { get; set; } = 1;

    public int[] RawBlock { get; set; } = CreateEmptyRawBlock();

    public TighteningSequenceCore? Core { get; set; }

    public static int SequenceBlockWordCount => TighteningSequenceRegisterMap.BlockWordCount;

    public static int[] CreateEmptyRawBlock() => new int[SequenceBlockWordCount];

    public void ApplyCoreToRaw()
    {
        Core ??= new TighteningSequenceCore();
        TighteningSequenceCodec.ApplyCoreToRaw(RawBlock, Core);
    }

    public void ExtractCoreFromRaw() => Core = TighteningSequenceCodec.ExtractCoreFromRaw(RawBlock);
}
