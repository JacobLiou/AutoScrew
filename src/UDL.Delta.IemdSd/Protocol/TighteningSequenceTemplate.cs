namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#200/#250 sequence block (raw words at 0xD2; layout per manual A.3).</summary>
public sealed class TighteningSequenceTemplate
{
    public int SequenceId { get; set; } = 1;

    public int[] RawBlock { get; set; } = CreateEmptyRawBlock();

    public static int SequenceBlockWordCount => 0x329 - 0xD2 + 1;

    public static int[] CreateEmptyRawBlock() => new int[SequenceBlockWordCount];
}
