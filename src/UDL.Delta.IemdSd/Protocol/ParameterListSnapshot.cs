namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#160 created tightening parameter sets (raw payload words).</summary>
public sealed class ParameterListSnapshot
{
    public const int MaxParameterSlots = 500;

    public int[] RawWords { get; init; } = [];

    /// <summary>
    /// Returns configured parameter IDs (1–500). Each payload word is 0 = empty, &gt;0 = created.
    /// </summary>
    public IReadOnlyList<int> GetConfiguredIds()
    {
        var ids = new List<int>();
        var limit = Math.Min(RawWords.Length, MaxParameterSlots);
        for (var i = 0; i < limit; i++)
        {
            if (RawWords[i] > 0)
                ids.Add(i + 1);
        }

        return ids;
    }
}
