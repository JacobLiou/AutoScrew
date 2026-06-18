using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Services;
using AutoScrew.Infrastructure.Mes;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Templates;

public sealed class MesTemplateUploadService : IMesTemplateUploadService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMesSettingsService _mesSettings;
    private readonly ILogger<MesTemplateUploadService> _logger;

    public MesTemplateUploadService(
        IHttpClientFactory httpClientFactory,
        IMesSettingsService mesSettings,
        ILogger<MesTemplateUploadService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mesSettings = mesSettings;
        _logger = logger;
    }

    public async Task<MesTemplateUploadResult?> UploadTemplateAsync(
        string partNumber,
        string localProductFolder,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(localProductFolder);

        var snapshot = _mesSettings.GetSnapshot();
        if (!snapshot.UseMockMes)
        {
            _logger.LogInformation(
                "MES template upload not implemented for production MES; PN={PartNumber}, folder={Folder}",
                partNumber,
                localProductFolder);
            return null;
        }

        if (!Directory.Exists(localProductFolder))
            throw new DirectoryNotFoundException(localProductFolder);

        var uri = BuildUri($"api/templates/{Uri.EscapeDataString(partNumber.Trim())}/package");
        await using var zipStream = new MemoryStream();
        TemplatePackageZip.CreateFromFolder(localProductFolder, zipStream);
        var zipBytes = zipStream.ToArray();

        using var content = new ByteArrayContent(zipBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

        var http = _httpClientFactory.CreateClient("mes");
        using var request = CreateRequest(HttpMethod.Post, uri);
        request.Content = content;

        using var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<UploadResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var contentHash = body?.ContentHash ?? ProductTemplatePackageHash.ComputePackageHash(localProductFolder);
        _logger.LogInformation("Uploaded MES template package for PN={PartNumber}", partNumber);
        return new MesTemplateUploadResult(contentHash, body?.Revision);
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

    private sealed class UploadResponse
    {
        [JsonPropertyName("accepted")]
        public bool Accepted { get; set; }

        [JsonPropertyName("revision")]
        public string? Revision { get; set; }

        [JsonPropertyName("contentHash")]
        public string? ContentHash { get; set; }
    }
}

internal static class TemplatePackageZip
{
    public static void CreateFromFolder(string sourceFolder, Stream destination)
    {
        using var archive = new ZipArchive(destination, ZipArchiveMode.Create, leaveOpen: true);
        foreach (var file in Directory.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceFolder, file).Replace('\\', '/');
            var entry = archive.CreateEntry(relative, CompressionLevel.Fastest);
            using var entryStream = entry.Open();
            using var fileStream = File.OpenRead(file);
            fileStream.CopyTo(entryStream);
        }
    }

    public static void ExtractToFolder(Stream zipStream, string destinationFolder)
    {
        Directory.CreateDirectory(destinationFolder);
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
}
