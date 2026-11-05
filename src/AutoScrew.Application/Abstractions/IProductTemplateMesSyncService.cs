namespace AutoScrew.Application.Abstractions;

public sealed record ProductTemplateMesSyncResult(
    int UploadedCount,
    int DownloadedCount,
    int SkippedCount,
    IReadOnlyList<string> DownloadedPartNumbers,
    IReadOnlyList<string> Errors,
    string SummaryMessage);

public interface IProductTemplateMesSyncService
{
    /// <param name="partNumber">当前编辑产品 PN；LegacyHttp / ProductKey 必填。</param>
    Task<ProductTemplateMesSyncResult> SyncWithMesAsync(
        string? partNumber = null,
        CancellationToken cancellationToken = default);
}
