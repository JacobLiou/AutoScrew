namespace AutoScrew.Domain.Session;

/// <summary>
/// Explicit transitions for the operator job session state machine.
/// </summary>
public static class JobSessionPhaseMachine
{
    public static bool TryTransition(JobSessionPhase current, JobSessionTrigger trigger, out JobSessionPhase next)
    {
        next = current;
        switch (current)
        {
            case JobSessionPhase.Idle:
                if (trigger == JobSessionTrigger.RequestScan)
                {
                    next = JobSessionPhase.SnPending;
                    return true;
                }

                break;

            case JobSessionPhase.SnPending:
                if (trigger == JobSessionTrigger.SnValidated)
                {
                    next = JobSessionPhase.LoadingRecipe;
                    return true;
                }

                if (trigger == JobSessionTrigger.SnRejected)
                {
                    next = JobSessionPhase.SnRejected;
                    return true;
                }

                break;

            case JobSessionPhase.SnRejected:
                if (trigger == JobSessionTrigger.RequestScan)
                {
                    next = JobSessionPhase.SnPending;
                    return true;
                }

                if (trigger == JobSessionTrigger.ResetToIdle || trigger == JobSessionTrigger.Abort)
                {
                    next = JobSessionPhase.Idle;
                    return true;
                }

                break;

            case JobSessionPhase.LoadingRecipe:
                if (trigger == JobSessionTrigger.RecipeLoaded)
                {
                    next = JobSessionPhase.Running;
                    return true;
                }

                if (trigger == JobSessionTrigger.LoadFailed)
                {
                    next = JobSessionPhase.Idle;
                    return true;
                }

                // 换产取消 / 下发失败：回到可重扫，不清成 Idle
                if (trigger == JobSessionTrigger.Abort)
                {
                    next = JobSessionPhase.SnPending;
                    return true;
                }

                break;

            case JobSessionPhase.Running:
                if (trigger == JobSessionTrigger.ScrewNg)
                {
                    next = JobSessionPhase.NgLocked;
                    return true;
                }

                if (trigger == JobSessionTrigger.SurfaceComplete)
                {
                    next = JobSessionPhase.AwaitFlip;
                    return true;
                }

                if (trigger == JobSessionTrigger.AllScrewsComplete)
                {
                    next = JobSessionPhase.Completed;
                    return true;
                }

                if (trigger == JobSessionTrigger.Abort)
                {
                    next = JobSessionPhase.Idle;
                    return true;
                }

                break;

            case JobSessionPhase.AwaitFlip:
                if (trigger == JobSessionTrigger.SurfaceAdvanceConfirmed)
                {
                    next = JobSessionPhase.Running;
                    return true;
                }

                if (trigger == JobSessionTrigger.Abort)
                {
                    next = JobSessionPhase.Idle;
                    return true;
                }

                break;

            case JobSessionPhase.NgLocked:
                if (trigger == JobSessionTrigger.TechUnlockContinue)
                {
                    next = JobSessionPhase.Running;
                    return true;
                }

                if (trigger == JobSessionTrigger.Abort)
                {
                    next = JobSessionPhase.Idle;
                    return true;
                }

                break;

            case JobSessionPhase.Completed:
                if (trigger == JobSessionTrigger.ResetToIdle)
                {
                    next = JobSessionPhase.Idle;
                    return true;
                }

                break;
        }

        return false;
    }
}
