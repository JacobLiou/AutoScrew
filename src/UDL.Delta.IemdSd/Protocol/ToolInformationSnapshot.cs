namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#650 tool information read payload (raw words).</summary>
public sealed class ToolInformationSnapshot
{
    public int ToolIndex { get; init; }

    public int[] RawWords { get; init; } = [];
}
