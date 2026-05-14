namespace AutoScrew.Domain.Session;

public enum JobSessionTrigger
{
    RequestScan,
    SnValidated,
    SnRejected,
    RecipeLoaded,
    LoadFailed,
    ScrewNg,
    AllScrewsComplete,
    TechUnlockContinue,
    Abort,
    ResetToIdle
}
