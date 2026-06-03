namespace AutoScrew.Domain.Models;

/// <summary>作业台多面进度（严格顺序）。</summary>
public enum SurfaceProgressState
{
    Locked,
    Active,
    Complete,
    NgLocked
}
