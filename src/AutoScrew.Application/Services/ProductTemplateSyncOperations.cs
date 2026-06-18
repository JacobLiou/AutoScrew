using AutoScrew.Application.Abstractions;

namespace AutoScrew.Application.Services;

public static class ProductTemplateSyncOperations
{
    public static string ComputeFileHash(string path)
    {
        using var stream = File.OpenRead(path);
        var hash = System.Security.Cryptography.SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    public static string ComputePackageHash(string productFolder) =>
        ProductTemplatePackageHash.ComputePackageHash(productFolder);

    public static async Task UpsertFromFileAsync(
        IProductTemplateSyncRepository repository,
        IProductTemplateLocalStore localStore,
        string partNumber,
        ProductTemplateSyncState syncState,
        string? lastError,
        DateTimeOffset? lastMesPullUtc,
        DateTimeOffset? lastMesPushUtc,
        string? mesRevision,
        CancellationToken cancellationToken)
    {
        var path = localStore.GetDefaultTemplatePath(partNumber);
        if (!File.Exists(path))
            return;

        var folder = localStore.GetProductFolder(partNumber);
        var packageHash = ComputePackageHash(folder);
        var modifiedUtc = ProductTemplatePackageHash.GetPackageModifiedUtc(folder);

        var record = new ProductTemplateSyncRecord(
            partNumber,
            localStore.ToRelativePath(path),
            syncState,
            packageHash,
            modifiedUtc,
            lastMesPullUtc,
            lastMesPushUtc,
            mesRevision,
            lastError);
        await repository.UpsertAsync(record, cancellationToken).ConfigureAwait(false);
    }
}
