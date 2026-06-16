using System.Security.Cryptography;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Application.Services;

public static class ProductTemplateSyncOperations
{
    public static string ComputeFileHash(string path)
    {
        using var stream = File.OpenRead(path);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    public static async Task UpsertFromFileAsync(
        IProductTemplateSyncRepository repository,
        IProductTemplateLocalStore localStore,
        string partNumber,
        ProductTemplateSyncState syncState,
        string? lastError,
        DateTimeOffset? lastMesPullUtc,
        CancellationToken cancellationToken)
    {
        var path = localStore.GetDefaultTemplatePath(partNumber);
        if (!File.Exists(path))
            return;

        var record = new ProductTemplateSyncRecord(
            partNumber,
            localStore.ToRelativePath(path),
            syncState,
            ComputeFileHash(path),
            File.GetLastWriteTimeUtc(path),
            lastMesPullUtc,
            null,
            null,
            lastError);
        await repository.UpsertAsync(record, cancellationToken).ConfigureAwait(false);
    }
}
