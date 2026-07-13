namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#160 created tightening parameter sets (raw payload words).</summary>
public sealed class ParameterListSnapshot
{
    public const int MaxParameterSlots = 500;

    public int[] RawWords { get; init; } = [];

    /// <summary>
    /// Returns configured parameter IDs (1–500).
    /// Supports slot bitmap (word[i]&gt;0 ⇒ ID i+1) and compact ID list (word0=count, word1..N=IDs).
    /// </summary>
    public IReadOnlyList<int> GetConfiguredIds()
    {
        if (RawWords.Length == 0)
            return [];

        var compact = TryParseCompactIdList();
        if (compact.Count > 0)
            return compact;

        return ParseSlotBitmap();
    }

    private List<int> TryParseCompactIdList()
    {
        var count = RawWords[0];
        if (count is < 1 or > MaxParameterSlots)
            return [];

        var ids = new List<int>(count);
        for (var i = 1; i < RawWords.Length && ids.Count < count; i++)
        {
            var id = RawWords[i];
            if (id is < 1 or > MaxParameterSlots)
                break;

            ids.Add(id);
        }

        return ids.Count == count ? ids : [];
    }

    private List<int> ParseSlotBitmap()
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
