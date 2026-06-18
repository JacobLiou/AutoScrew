namespace AutoScrew.Application.Abstractions;

public sealed record MesRemoteTemplateEntry(
    string PartNumber,
    string ContentHash,
    DateTimeOffset ModifiedUtc,
    string PackageUrl);

public interface IMesTemplateCatalogClient
{
    Task<IReadOnlyList<MesRemoteTemplateEntry>> ListRemoteTemplatesAsync(
        CancellationToken cancellationToken = default);
}
