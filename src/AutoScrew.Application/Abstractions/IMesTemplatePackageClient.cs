namespace AutoScrew.Application.Abstractions;

public interface IMesTemplatePackageClient
{
    Task<string> DownloadTemplatePackageAsync(
        string partNumber,
        string packageUrl,
        CancellationToken cancellationToken = default);
}

public sealed record MesTemplateUploadResult(string ContentHash, string? Revision);

public interface IMesTemplateUploadService
{
    Task<MesTemplateUploadResult?> UploadTemplateAsync(
        string partNumber,
        string localProductFolder,
        CancellationToken cancellationToken = default);
}
