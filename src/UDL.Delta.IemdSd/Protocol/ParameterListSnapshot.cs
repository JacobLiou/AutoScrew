namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#160 created tightening parameter sets (raw payload words).</summary>
public sealed class ParameterListSnapshot
{
    public int[] RawWords { get; init; } = [];
}
