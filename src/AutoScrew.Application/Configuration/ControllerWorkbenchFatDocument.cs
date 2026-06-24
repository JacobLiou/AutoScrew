namespace AutoScrew.Application.Configuration;

public sealed class ControllerWorkbenchFatDocument
{
    public List<ControllerWorkbenchFatItemState> Items { get; set; } = CreateDefaultItems();

    public DateTimeOffset? LastRunUtc { get; set; }

    public static List<ControllerWorkbenchFatItemState> CreateDefaultItems() =>
    [
        new(1, "S.Workbench.Fat.ParamDiff"),
        new(2, "S.Workbench.Fat.ParamWrite"),
        new(3, "S.Workbench.Fat.SourceManual"),
        new(4, "S.Workbench.Fat.HostGuidedActivate"),
        new(5, "S.Workbench.Fat.SequenceDiff"),
        new(6, "S.Workbench.Fat.DeviceProgramTrial"),
        new(7, "S.Workbench.Fat.ModeSwitchBack"),
    ];
}

public sealed class ControllerWorkbenchFatItemState
{
    public ControllerWorkbenchFatItemState()
    {
    }

    public ControllerWorkbenchFatItemState(int id, string resourceKey)
    {
        Id = id;
        ResourceKey = resourceKey;
    }

    public int Id { get; set; }

    public string ResourceKey { get; set; } = string.Empty;

    public bool IsChecked { get; set; }

    public string? LastResult { get; set; }
}
