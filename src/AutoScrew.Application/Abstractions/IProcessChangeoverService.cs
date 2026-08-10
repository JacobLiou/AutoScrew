namespace AutoScrew.Application.Abstractions;

public enum ChangeoverReason
{
    SameSkip,
    FirstDeploy,
    ProductPnChanged,
    ProcessVersionChanged,
    ProductMissing,
}

public sealed record ChangeoverDecision(
    ChangeoverReason Reason,
    string NewProductPn,
    string? PreviousProductPn,
    DateTimeOffset? LibraryUpdatedUtc)
{
    public bool NeedsChangeover => Reason != ChangeoverReason.SameSkip;
}

/// <summary>扫 SN 后评估是否需换产，并覆盖下发工艺库参数+顺序。</summary>
public interface IProcessChangeoverService
{
    StationProcessState? GetStationState();

    Task<ChangeoverDecision> EvaluateAsync(string productPn, CancellationToken cancellationToken = default);

    /// <summary>覆盖下发参数与顺序；全部成功后写入工位状态。失败不更新状态。</summary>
    Task DeployAndCommitAsync(string productPn, CancellationToken cancellationToken = default);
}
