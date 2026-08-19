using System.IO.Compression;
using AutoScrew.Application.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var mesStoreRoot = Path.Combine(AppContext.BaseDirectory, "MesStore");
Directory.CreateDirectory(mesStoreRoot);

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "MesMockServer" }));

app.MapGet("/api/sn/validate", (string sn, string stationId) =>
{
    if (string.Equals(sn, "__PING__", StringComparison.Ordinal))
        return Results.Ok(new { valid = false, partNumber = (string?)null, message = "ping ok" });

    return Results.Ok(new { valid = true, partNumber = "PNDEMO", message = (string?)null });
});

app.MapGet("/api/recipe", (string sn, string pn, string stationId) =>
    Results.Ok(new
    {
        templateJsonPath = "PNDEMO/PNDEMO.product-template.json",
        templatePackageUrl = "api/templates/PNDEMO/package",
        productImageUrl = (string?)null,
        screws = new[]
        {
            new
            {
                index = 1,
                partNo = "SCR-01",
                targetTorqueNm = 0.5,
                torqueLowerNm = 0.45,
                torqueUpperNm = 0.55,
                angleLimitDeg = 360.0,
                controllerParameterId = 1,
            },
        },
    }));

app.MapGet("/api/templates", (string? stationId) =>
{
    var entries = new List<object>();
    var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    if (Directory.Exists(mesStoreRoot))
    {
        foreach (var folder in Directory.EnumerateDirectories(mesStoreRoot))
        {
            var pn = Path.GetFileName(folder);
            if (string.IsNullOrWhiteSpace(pn) || !HasTemplateJson(folder, pn))
                continue;

            seen.Add(pn);
            entries.Add(CreateCatalogEntry(pn, folder));
        }
    }

    foreach (var pn in new[] { "PNDEMO" })
    {
        if (seen.Contains(pn))
            continue;

        var folder = ResolveTemplateFolder(pn);
        if (folder is null)
            continue;

        seen.Add(pn);
        entries.Add(CreateCatalogEntry(pn, folder));
    }

    return Results.Ok(entries);
});

app.MapGet("/api/templates/{pn}/package", (string pn) =>
{
    var folder = ResolveStoredTemplateFolder(pn);
    if (folder is null)
        return Results.NotFound(new { message = $"Template folder not found for PN {pn}" });

    return Results.File(CreateZipBytes(folder), "application/zip", $"{pn}-template.zip");
});

app.MapPost("/api/templates/{pn}/package", async (string pn, HttpRequest request) =>
{
    var targetFolder = Path.Combine(mesStoreRoot, SanitizePn(pn));
    if (Directory.Exists(targetFolder))
        Directory.Delete(targetFolder, recursive: true);

    Directory.CreateDirectory(targetFolder);

    await using var body = request.Body;
    await using var ms = new MemoryStream();
    await body.CopyToAsync(ms);
    ms.Position = 0;

    ExtractZip(ms, targetFolder);

    if (!HasTemplateJson(targetFolder, pn))
        return Results.BadRequest(new { message = $"Uploaded package for PN {pn} does not contain a product template JSON." });

    var contentHash = ProductTemplatePackageHash.ComputePackageHash(targetFolder);
    var modifiedUtc = ProductTemplatePackageHash.GetPackageModifiedUtc(targetFolder);
    var revision = modifiedUtc.ToString("O");

    return Results.Ok(new
    {
        accepted = true,
        revision,
        contentHash,
        modifiedUtc,
    });
});

app.MapPost("/api/results", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    _ = await reader.ReadToEndAsync();
    return Results.Ok(new { accepted = true });
});

app.Run("http://localhost:5080");

static object CreateCatalogEntry(string pn, string folder) =>
    new
    {
        partNumber = pn,
        contentHash = ProductTemplatePackageHash.ComputePackageHash(folder),
        modifiedUtc = ProductTemplatePackageHash.GetPackageModifiedUtc(folder),
        packageUrl = $"api/templates/{pn}/package",
    };

static string? ResolveStoredTemplateFolder(string pn)
{
    var stored = Path.Combine(Path.Combine(AppContext.BaseDirectory, "MesStore"), SanitizePn(pn));
    if (Directory.Exists(stored) && HasTemplateJson(stored, pn))
        return stored;

    return ResolveTemplateFolder(pn);
}

static string? ResolveTemplateFolder(string pn)
{
    var candidates = new[]
    {
        Path.Combine(AppContext.BaseDirectory, "Fixtures", pn),
        Path.Combine(AppContext.BaseDirectory, "Samples", pn),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "AutoScrew.Hmi", "Samples", pn)),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "AutoScrew.Hmi", "Templates", pn)),
    };

    foreach (var path in candidates)
    {
        if (Directory.Exists(path) && HasTemplateJson(path, pn))
            return path;
    }

    var demoJson = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "AutoScrew.Hmi", "Samples", "PNDEMO", "PNDEMO.product-template.json"));
    if (string.Equals(pn, "PNDEMO", StringComparison.OrdinalIgnoreCase) && File.Exists(demoJson))
    {
        var temp = Path.Combine(Path.GetTempPath(), "MesMockServer", pn);
        Directory.CreateDirectory(temp);
        var dest = Path.Combine(temp, "PNDEMO.product-template.json");
        File.Copy(demoJson, dest, overwrite: true);
        return temp;
    }

    return null;
}

static bool HasTemplateJson(string folder, string pn)
{
    var expected = Path.Combine(folder, $"{SanitizePn(pn)}.product-template.json");
    if (File.Exists(expected))
        return true;

    return Directory.EnumerateFiles(folder, "*.product-template.json", SearchOption.AllDirectories).Any();
}

static string SanitizePn(string pn) => pn.Trim();

static byte[] CreateZipBytes(string folder)
{
    using var ms = new MemoryStream();
    using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
    {
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(folder, file).Replace('\\', '/');
            var entry = archive.CreateEntry(relative, CompressionLevel.Fastest);
            using var entryStream = entry.Open();
            using var fileStream = File.OpenRead(file);
            fileStream.CopyTo(entryStream);
        }
    }

    ms.Position = 0;
    return ms.ToArray();
}

static void ExtractZip(Stream zipStream, string destinationFolder)
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
