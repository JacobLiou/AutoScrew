namespace AutoScrew.Domain.Session;

/// <summary>
/// High-level job session lifecycle (see doc/Design.md §5).
/// </summary>
public enum JobSessionPhase
{
    Idle,
    SnPending,
    SnRejected,
    LoadingRecipe,
    Running,
    AwaitFlip,
    NgLocked,
    Completed
}
