namespace AutoScrew.Application.Abstractions;

public interface IMesTemplatePackageClient
{
    Task<string> DownloadTemplatePackageAsync(
        string partNumber,
        string packageUrl,
        CancellationToken cancellationToken = default);
}

public interface IMesTemplateUploadService
{
    Task UploadTemplateAsync(string partNumber, string localProductFolder, CancellationToken cancellationToken = default);
}
