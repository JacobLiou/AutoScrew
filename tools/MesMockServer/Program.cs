using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "MesMockServer" }));

app.MapGet("/api/sn/validate", (string sn, string stationId) =>
{
    if (string.Equals(sn, "__PING__", StringComparison.Ordinal))
        return Results.Ok(new { valid = false, partNumber = (string?)null, message = "ping ok" });

    return Results.Ok(new { valid = true, partNumber = "PN-DEMO", message = (string?)null });
});

app.MapGet("/api/recipe", (string sn, string pn, string stationId) =>
    Results.Ok(new
    {
        templateJsonPath = "PN-DEMO/PN-DEMO.product-template.json",
        templatePackageUrl = "api/templates/PN-DEMO/package",
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

app.MapGet("/api/templates/{pn}/package", (string pn) =>
{
    var folder = ResolveTemplateFolder(pn);
    if (folder is null)
        return Results.NotFound(new { message = $"Template folder not found for PN {pn}" });

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
    return Results.File(ms.ToArray(), "application/zip", $"{pn}-template.zip");
});

app.MapPost("/api/results", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    _ = await reader.ReadToEndAsync();
    return Results.Ok(new { accepted = true });
});

app.Run("http://localhost:5080");

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
        if (Directory.Exists(path))
            return path;
    }

    var demoJson = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "AutoScrew.Hmi", "Samples", "demo-product-multisurface.product-template.json"));
    if (string.Equals(pn, "PN-DEMO", StringComparison.OrdinalIgnoreCase) && File.Exists(demoJson))
    {
        var temp = Path.Combine(Path.GetTempPath(), "MesMockServer", pn);
        Directory.CreateDirectory(temp);
        var dest = Path.Combine(temp, "PN-DEMO.product-template.json");
        File.Copy(demoJson, dest, overwrite: true);
        return temp;
    }

    return null;
}
