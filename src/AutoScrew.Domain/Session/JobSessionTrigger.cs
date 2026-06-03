namespace AutoScrew.Domain.Session;

public enum JobSessionTrigger
{
    RequestScan,
    SnValidated,
    SnRejected,
    RecipeLoaded,
    LoadFailed,
    ScrewNg,
    SurfaceComplete,
    SurfaceAdvanceConfirmed,
    AllScrewsComplete,
    TechUnlockContinue,
    Abort,
    ResetToIdle
}
