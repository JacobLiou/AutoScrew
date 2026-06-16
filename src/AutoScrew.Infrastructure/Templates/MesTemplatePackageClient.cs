using System.IO.Compression;
using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Mes;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Templates;

public sealed class MesTemplatePackageClient : IMesTemplatePackageClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMesSettingsService _mesSettings;
    private readonly IProductTemplateLocalStore _localStore;
    private readonly ILogger<MesTemplatePackageClient> _logger;

    public MesTemplatePackageClient(
        IHttpClientFactory httpClientFactory,
        IMesSettingsService mesSettings,
        IProductTemplateLocalStore localStore,
        ILogger<MesTemplatePackageClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mesSettings = mesSettings;
        _localStore = localStore;
        _logger = logger;
    }

    public async Task<string> DownloadTemplatePackageAsync(
        string partNumber,
        string packageUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(packageUrl);

        var folder = _localStore.GetProductFolder(partNumber);
        Directory.CreateDirectory(folder);

        var uri = BuildUri(packageUrl);
        var http = _httpClientFactory.CreateClient("mes");
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        if (contentType.Contains("zip", StringComparison.OrdinalIgnoreCase)
            || packageUrl.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
            || packageUrl.Contains("/package", StringComparison.OrdinalIgnoreCase))
        {
            ExtractZip(stream, folder);
        }
        else
        {
            var dest = _localStore.GetDefaultTemplatePath(partNumber);
            await using var file = File.Create(dest);
            await stream.CopyToAsync(file, cancellationToken).ConfigureAwait(false);
        }

        var resolved = _localStore.TryResolveLocalTemplate(partNumber)
                       ?? _localStore.TryResolveTemplatePath($"{Sanitize(partNumber)}/{Sanitize(partNumber)}.product-template.json");
        if (resolved is null)
            throw new InvalidOperationException($"Template package downloaded but no JSON found for PN {partNumber}.");

        _logger.LogInformation("Downloaded MES template package for PN={PartNumber} to {Folder}", partNumber, folder);
        return resolved;
    }

    private Uri BuildUri(string relativeOrAbsolute)
    {
        if (Uri.TryCreate(relativeOrAbsolute, UriKind.Absolute, out var absolute))
            return absolute;

        var snapshot = _mesSettings.GetSnapshot();
        var baseUrl = snapshot.BaseUrl.Trim();
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        return new Uri(new Uri(baseUrl, UriKind.Absolute), relativeOrAbsolute.TrimStart('/'));
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, Uri uri)
    {
        var request = new HttpRequestMessage(method, uri);
        var snapshot = _mesSettings.GetSnapshot();
        if (!string.IsNullOrWhiteSpace(snapshot.ApiKey))
            request.Headers.TryAddWithoutValidation("X-Api-Key", snapshot.ApiKey);
        return request;
    }

    private static void ExtractZip(Stream zipStream, string destinationFolder)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            var targetPath = Path.Combine(destinationFolder, entry.FullName.Replace('/', Path.DirectorySeparatorChar));
            var targetDir = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(targetDir))
                Directory.CreateDirectory(targetDir);

            entry.ExtractToFile(targetPath, overwrite: true);
        }
    }

    private static string Sanitize(string pn) => pn.Trim();
}
