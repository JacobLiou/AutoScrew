namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#160 created tightening parameter sets (raw payload words).</summary>
public sealed class ParameterListSnapshot
{
    public const int MaxParameterSlots = 500;

    public int[] RawWords { get; init; } = [];

    /// <summary>
    /// Returns configured parameter IDs (1–500).
    /// Supports slot bitmap (word[i]&gt;0 ⇒ ID i+1) and compact ID list (word0=count, word1..N=IDs).
    /// Binary 0/1 occupancy flags prefer bitmap (compact would mis-read leading 1 as "count=1").
    /// </summary>
    public IReadOnlyList<int> GetConfiguredIds()
    {
        if (RawWords.Length == 0)
            return [];

        if (IsBinaryOccupiedFlags())
            return ParseSlotBitmap();

        var compact = TryParseCompactIdList();
        if (compact.Count > 0)
            return compact;

        return ParseSlotBitmap();
    }

    /// <summary>True when payload looks like per-slot occupied flags (only 0/1).</summary>
    private bool IsBinaryOccupiedFlags()
    {
        var limit = Math.Min(RawWords.Length, MaxParameterSlots);
        var sawOne = false;
        for (var i = 0; i < limit; i++)
        {
            var w = RawWords[i];
            if (w is not (0 or 1))
                return false;
            if (w == 1)
                sawOne = true;
        }

        return sawOne;
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
