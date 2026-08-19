using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Mes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class LocalRecipeMesClientTests
{
    [Fact]
    public async Task ValidateSnAsync_WhenRegistryMissing_FallsBackToLegacyMock()
    {
        var root = CreateTempRoot();
        try
        {
            var client = CreateClient(root, templateDir: null);
            var result = await client.ValidateSnAsync("ABC", CancellationToken.None);

            Assert.True(result.IsValid);
            Assert.Equal("PNDEMO", result.PartNumber);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task ValidateSnAsync_WhenSnRegistered_ReturnsPartNumber()
    {
        var root = CreateTempRoot();
        try
        {
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(templateDir);
            await WriteRegistryAsync(templateDir, "PN-TEST", ["SN-OK-001"], "PN-TEST.product-template.json");
            File.WriteAllText(Path.Combine(templateDir, "PN-TEST.product-template.json"), "{}");

            var client = CreateClient(root, templateDir);
            var result = await client.ValidateSnAsync("SN-OK-001", CancellationToken.None);

            Assert.True(result.IsValid);
            Assert.Equal("PN-TEST", result.PartNumber);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task ValidateSnAsync_WhenSnNotRegistered_ReturnsInvalid()
    {
        var root = CreateTempRoot();
        try
        {
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(templateDir);
            await WriteRegistryAsync(templateDir, "PN-TEST", ["SN-OK-001"], "PN-TEST.product-template.json");

            var client = CreateClient(root, templateDir);
            var result = await client.ValidateSnAsync("SN-UNKNOWN", CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.Null(result.PartNumber);
            Assert.Contains("local-recipes.json", result.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task GetRecipeAsync_UsesDefaultTemplateFileName_WhenOmitted()
    {
        var root = CreateTempRoot();
        try
        {
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(templateDir);
            await WriteRegistryAsync(templateDir, "PN-ABC", ["SN1"], templateFile: null);
            File.WriteAllText(Path.Combine(templateDir, "PN-ABC.product-template.json"), "{}");

            var client = CreateClient(root, templateDir);
            var recipe = await client.GetRecipeAsync("SN1", "PN-ABC", CancellationToken.None);

            Assert.Equal("PN-ABC", recipe.PartNumber);
            Assert.Equal("PN-ABC.product-template.json", recipe.TemplateJsonPath);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task LocalJsonRecipeStore_SaveAndLoad_RoundTrips()
    {
        var root = CreateTempRoot();
        try
        {
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(templateDir);
            var store = CreateStore(root, templateDir);
            var doc = new LocalRecipeDocument
            {
                Products =
                [
                    new LocalRecipeProductEntry
                    {
                        PartNumber = "PN-X",
                        SerialNumbers = ["A", "B", "A"],
                    },
                ],
            };

            await store.SaveAsync(doc, CancellationToken.None);
            var loaded = await store.LoadAsync(CancellationToken.None);

            Assert.True(loaded.Exists);
            Assert.Single(loaded.Document.Products);
            Assert.Equal("PN-X.product-template.json", loaded.Document.Products[0].TemplateFile);
            Assert.Equal(2, loaded.Document.Products[0].SerialNumbers.Count);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    private static LocalRecipeMesClient CreateClient(string dataDirectory, string? templateDir)
    {
        var store = CreateStore(dataDirectory, templateDir);
        var options = Options.Create(new AutoScrewAppOptions
        {
            DataDirectory = dataDirectory,
            TemplateDirectory = templateDir ?? "",
        });
        return new LocalRecipeMesClient(store, options, NullLogger<LocalRecipeMesClient>.Instance);
    }

    private static LocalJsonRecipeStore CreateStore(string dataDirectory, string? templateDir) =>
        new(Options.Create(new AutoScrewAppOptions
        {
            DataDirectory = dataDirectory,
            TemplateDirectory = templateDir ?? "",
        }));

    private static async Task WriteRegistryAsync(
        string templateDir,
        string partNumber,
        string[] serialNumbers,
        string? templateFile)
    {
        var store = CreateStore(templateDir, templateDir);
        await store.SaveAsync(new LocalRecipeDocument
        {
            Products =
            [
                new LocalRecipeProductEntry
                {
                    PartNumber = partNumber,
                    TemplateFile = templateFile,
                    SerialNumbers = serialNumbers.ToList(),
                },
            ],
        }, CancellationToken.None);
    }

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-local-recipes-test", Guid.NewGuid().ToString("N"));
}
