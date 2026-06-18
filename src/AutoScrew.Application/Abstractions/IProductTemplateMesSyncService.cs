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
    Task<ProductTemplateMesSyncResult> SyncWithMesAsync(CancellationToken cancellationToken = default);
}
