using System.Text.Json;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Templates;
using AutoScrew.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Templates;

public sealed class TemplateLayoutJsonLoader(ILogger<TemplateLayoutJsonLoader> logger) : ITemplateLayoutLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<TemplateLoadResult> LoadAsync(string jsonFilePath, CancellationToken cancellationToken = default)
    {
        var product = await LoadProductInternalAsync(jsonFilePath, cancellationToken).ConfigureAwait(false);
        var primary = ProductTemplateSequence.GetPrimarySurface(product);
        var flat = ProductTemplateSequence.FlattenPrimarySurface(product);
        var baseDir = Path.GetDirectoryName(jsonFilePath) ?? AppContext.BaseDirectory;
        var imagePath = ResolveImagePath(baseDir, flat.ProductImageRelativePath, flat.ProductImageAbsolutePath);
        var positions = BuildPositions(flat);

        logger.LogInformation(
            "Loaded template {Path}: {SurfaceCount} surface(s), operating on primary {SurfaceId} with {Count} markers.",
            jsonFilePath,
            product.Surfaces.Count,
            primary.SurfaceId,
            positions.Count);

        return new TemplateLoadResult(
            flat,
            positions,
            imagePath,
            product.Surfaces.Count,
            primary.SurfaceId,
            primary.Name);
    }

    public async Task<ProductTemplateLoadResult> LoadProductAsync(
        string jsonFilePath,
        CancellationToken cancellationToken = default)
    {
        var product = await LoadProductInternalAsync(jsonFilePath, cancellationToken).ConfigureAwait(false);
        var baseDir = Path.GetDirectoryName(jsonFilePath) ?? AppContext.BaseDirectory;
        return new ProductTemplateLoadResult(product, jsonFilePath, baseDir);
    }

    private static async Task<ProductTemplateDto> LoadProductInternalAsync(
        string jsonFilePath,
        CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(jsonFilePath, cancellationToken).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        var schema = doc.RootElement.TryGetProperty("schemaVersion", out var sv) ? sv.GetInt32() : 1;

        if (schema >= 2)
        {
            var product = JsonSerializer.Deserialize<ProductTemplateDto>(json, JsonOptions)
                          ?? throw new InvalidOperationException("Invalid product template JSON.");
            if (product.SurfaceCount != product.Surfaces.Count)
                throw new InvalidOperationException("surfaceCount does not match surfaces length.");
            return product;
        }

        var legacy = JsonSerializer.Deserialize<TemplateLayoutDto>(json, JsonOptions)
                     ?? throw new InvalidOperationException("Invalid template JSON.");

        return new ProductTemplateDto
        {
            SchemaVersion = 2,
            ProductId = Path.GetFileNameWithoutExtension(jsonFilePath),
            DisplayName = Path.GetFileNameWithoutExtension(jsonFilePath),
            SurfaceCount = 1,
            AssemblySequence = "surfaceOrderThenLocalIndex",
            Surfaces =
            [
                new SurfaceLayoutDto
                {
                    SurfaceId = "DEFAULT",
                    Name = "默认面",
                    Order = 1,
                    BoardWidth = legacy.BoardWidth,
                    BoardHeight = legacy.BoardHeight,
                    CircleDiameter = legacy.CircleDiameter,
                    ProductImageRelativePath = legacy.ProductImageRelativePath,
                    ProductImageAbsolutePath = legacy.ProductImageAbsolutePath,
                    ProductImageOpacity = legacy.ProductImageOpacity,
                    Markers = legacy.Markers,
                }
            ],
        };
    }

    private static string? ResolveImagePath(string baseDir, string? relative, string? absolute)
    {
        if (!string.IsNullOrWhiteSpace(relative))
        {
            var rel = Path.Combine(baseDir, relative);
            if (File.Exists(rel))
                return rel;
        }

        if (!string.IsNullOrWhiteSpace(absolute) && File.Exists(absolute))
            return absolute;

        return null;
    }

    private static List<ScrewPosition> BuildPositions(TemplateLayoutDto raw)
    {
        var markers = raw.Markers.OrderBy(m => m.Index).ToList();
        var positions = new List<ScrewPosition>(markers.Count);
        foreach (var m in markers)
        {
            var diameter = m.CircleDiameter ?? raw.CircleDiameter;
            positions.Add(new ScrewPosition(m.Index, m.CenterX, m.CenterY, diameter, m.ScrewTypeId, m.PartNo));
        }

        return positions;
    }
}
