using System.Text.Json;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Services;
using AutoScrew.Infrastructure.ProcessLibrary;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Templates;

public sealed class ProductTemplateLanShare : IProductTemplateLanShare
{
    private readonly ProcessLibraryStore _processStore;
    private readonly IProductTemplateLocalStore _localStore;
    private readonly ILogger<ProductTemplateLanShare> _logger;

    public ProductTemplateLanShare(
        ProcessLibraryStore processStore,
        IProductTemplateLocalStore localStore,
        ILogger<ProductTemplateLanShare> logger)
    {
        _processStore = processStore;
        _localStore = localStore;
        _logger = logger;
    }

    public Task<ProductTemplateLanPublishResult> PublishAsync(
        string partNumber,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partNumber);
        cancellationToken.ThrowIfCancellationRequested();

        var pn = partNumber.Trim();
        var sourceFolder = _localStore.GetProductFolder(pn);
        if (!Directory.Exists(sourceFolder) || _localStore.TryResolveLocalTemplate(pn) is null)
            throw new InvalidOperationException($"本地螺钉模板不存在：{pn}");

        _processStore.EnsureRoot();
        _processStore.EnsureLanConnectedForWrite();

        var targetFolder = _processStore.GetProductTemplateFolder(pn);
        var templateFile = Path.Combine(targetFolder, $"{Sanitize(pn)}.product-template.json");
        var localHash = ProductTemplatePackageHash.ComputePackageHash(sourceFolder);

        string? previousLanHash = null;
        var lanExisted = Directory.Exists(targetFolder)
                         && File.Exists(templateFile);
        if (lanExisted)
        {
            previousLanHash = ProductTemplatePackageHash.ComputePackageHash(targetFolder);
            if (string.Equals(localHash, previousLanHash, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(
                    "LAN screw template unchanged PN={PartNumber} hash={Hash}",
                    pn,
                    localHash);
                return Task.FromResult(new ProductTemplateLanPublishResult(
                    targetFolder,
                    templateFile,
                    ProductTemplateLanPublishOutcome.Unchanged,
                    localHash,
                    previousLanHash));
            }
        }

        Directory.CreateDirectory(targetFolder);
        MirrorCopyDirectory(sourceFolder, targetFolder, cancellationToken);

        if (!File.Exists(templateFile))
            throw new InvalidOperationException($"发布后未找到模板文件：{templateFile}");

        var outcome = lanExisted
            ? ProductTemplateLanPublishOutcome.Updated
            : ProductTemplateLanPublishOutcome.Created;

        _logger.LogInformation(
            "Published screw template PN={PartNumber} outcome={Outcome} to {Target} hash={Hash}",
            pn,
            outcome,
            targetFolder,
            localHash);

        return Task.FromResult(new ProductTemplateLanPublishResult(
            targetFolder,
            templateFile,
            outcome,
            localHash,
            previousLanHash));
    }

    public IReadOnlyList<ProductTemplateLanEntry> ListLanTemplates()
    {
        try
        {
            _processStore.EnsureRoot();
            var entries = new List<ProductTemplateLanEntry>();
            foreach (var pn in _processStore.ListProductPns())
            {
                var path = TryResolveLanTemplate(pn);
                if (path is null)
                    continue;

                entries.Add(new ProductTemplateLanEntry(pn, path, TryReadSurfaceCount(path)));
            }

            return entries;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "List LAN screw templates failed.");
            return Array.Empty<ProductTemplateLanEntry>();
        }
    }

    public Task<string> PullToLocalAsync(string partNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partNumber);
        cancellationToken.ThrowIfCancellationRequested();

        var pn = partNumber.Trim();
        var lanJson = TryResolveLanTemplate(pn)
                      ?? throw new InvalidOperationException($"局域网不存在螺钉模板：{pn}");

        var lanFolder = Path.GetDirectoryName(lanJson);
        if (string.IsNullOrWhiteSpace(lanFolder) || !Directory.Exists(lanFolder))
            throw new InvalidOperationException($"局域网模板目录无效：{pn}");

        _processStore.EnsureLanConnectedForWrite();

        var localFolder = _localStore.GetProductFolder(pn);
        Directory.CreateDirectory(localFolder);
        MirrorCopyDirectory(lanFolder, localFolder, cancellationToken);

        var resolved = _localStore.TryResolveLocalTemplate(pn);
        if (resolved is null)
            throw new InvalidOperationException($"拉取后本机未找到模板文件：{pn}");

        _logger.LogInformation(
            "Pulled LAN screw template PN={PartNumber} to {Folder}",
            pn,
            localFolder);

        return Task.FromResult(resolved);
    }

    public string? TryResolveLanTemplate(string partNumber)
    {
        if (string.IsNullOrWhiteSpace(partNumber))
            return null;

        try
        {
            var path = _processStore.GetProductTemplateJsonPath(partNumber.Trim());
            return File.Exists(path) ? path : null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "LAN template resolve failed for PN={PartNumber}", partNumber);
            return null;
        }
    }

    public async Task<string?> TryMaterializeLanTemplateToLocalAsync(
        string partNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(partNumber))
            return null;

        var pn = partNumber.Trim();
        var local = _localStore.TryResolveLocalTemplate(pn);
        if (local is not null)
            return local;

        try
        {
            // UNC 镜像拷贝可能秒级；放线程池执行并透传取消令牌，避免占用调用线程。
            return await Task.Run(() => PullToLocalAsync(pn, cancellationToken), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Materialize LAN template failed for PN={PartNumber}", partNumber);
            return null;
        }
    }

    /// <summary>
    /// 镜像同步：覆盖同名文件，并删除目标侧多余文件（使目标与源一致）。
    /// </summary>
    private static void MirrorCopyDirectory(string sourceDir, string destDir, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(destDir);

        var sourceFiles = Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories)
            .Select(f => (
                Full: f,
                Rel: Path.GetRelativePath(sourceDir, f).Replace('/', Path.DirectorySeparatorChar)))
            .ToList();

        var sourceRelSet = new HashSet<string>(
            sourceFiles.Select(x => x.Rel),
            StringComparer.OrdinalIgnoreCase);

        foreach (var (full, rel) in sourceFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var dest = Path.Combine(destDir, rel);
            var destParent = Path.GetDirectoryName(dest);
            if (!string.IsNullOrEmpty(destParent))
                Directory.CreateDirectory(destParent);
            File.Copy(full, dest, overwrite: true);
        }

        if (!Directory.Exists(destDir))
            return;

        foreach (var destFile in Directory.EnumerateFiles(destDir, "*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rel = Path.GetRelativePath(destDir, destFile);
            if (!sourceRelSet.Contains(rel))
                File.Delete(destFile);
        }

        foreach (var dir in Directory.EnumerateDirectories(destDir, "*", SearchOption.AllDirectories)
                     .OrderByDescending(d => d.Length))
        {
            if (!Directory.EnumerateFileSystemEntries(dir).Any())
                Directory.Delete(dir);
        }
    }

    private static int TryReadSurfaceCount(string templateJsonPath)
    {
        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(templateJsonPath));
            if (doc.RootElement.TryGetProperty("surfaces", out var surfaces)
                && surfaces.ValueKind == JsonValueKind.Array)
                return surfaces.GetArrayLength();
        }
        catch
        {
            // ignore malformed
        }

        return 0;
    }

    private static string Sanitize(string pn) => pn.Trim();
}
