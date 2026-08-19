namespace AutoScrew.Infrastructure.ProcessLibrary;

internal readonly record struct LocalPresetOrigin(int Id, string? SourceProductPn, int? SourceIdentity);

/// <summary>本机 1–500 预设：同产品身份覆盖，跨产品或无主占用则新分配。</summary>
internal static class ProcessLibraryLocalIdAllocator
{
    public const int MinId = 1;
    public const int MaxId = 500;

    public static int Resolve(
        IReadOnlyList<LocalPresetOrigin> existing,
        string productPn,
        int sourceIdentity,
        int preferredId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productPn);
        if (preferredId is < MinId or > MaxId)
        {
            throw new ArgumentOutOfRangeException(
                nameof(preferredId),
                preferredId,
                $"首选 ID 须为 {MinId}–{MaxId}。");
        }

        foreach (var item in existing)
        {
            if (item.SourceIdentity == sourceIdentity
                && string.Equals(item.SourceProductPn, productPn, StringComparison.OrdinalIgnoreCase))
                return item.Id;
        }

        var used = existing.Select(e => e.Id).ToHashSet();
        LocalPresetOrigin? occupant = null;
        foreach (var item in existing)
        {
            if (item.Id == preferredId)
            {
                occupant = item;
                break;
            }
        }

        if (occupant is null)
            return preferredId;

        return AllocateFree(used);
    }

    public static int AllocateFree(IReadOnlyCollection<int> usedIds)
    {
        var used = usedIds as HashSet<int> ?? usedIds.ToHashSet();
        for (var id = MinId; id <= MaxId; id++)
        {
            if (!used.Contains(id))
                return id;
        }

        throw new InvalidOperationException($"本机预设 ID {MinId}–{MaxId} 已满，无法新增。");
    }
}
