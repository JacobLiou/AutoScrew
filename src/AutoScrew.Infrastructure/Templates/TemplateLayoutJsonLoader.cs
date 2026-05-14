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
        await using var stream = File.OpenRead(jsonFilePath);
        var raw = await JsonSerializer.DeserializeAsync<TemplateLayoutDto>(stream, JsonOptions, cancellationToken)
                  .ConfigureAwait(false);
        if (raw is null)
            throw new InvalidOperationException("Invalid template JSON.");

        var baseDir = Path.GetDirectoryName(jsonFilePath) ?? AppContext.BaseDirectory;
        string? imagePath = null;
        if (!string.IsNullOrWhiteSpace(raw.ProductImageRelativePath))
        {
            var rel = Path.Combine(baseDir, raw.ProductImageRelativePath);
            if (File.Exists(rel))
                imagePath = rel;
        }

        if (imagePath is null && !string.IsNullOrWhiteSpace(raw.ProductImageAbsolutePath) && File.Exists(raw.ProductImageAbsolutePath))
            imagePath = raw.ProductImageAbsolutePath;

        var markers = raw.Markers.OrderBy(m => m.Index).ToList();
        var positions = new List<ScrewPosition>(markers.Count);
        foreach (var m in markers)
        {
            var diameter = m.CircleDiameter ?? raw.CircleDiameter;
            positions.Add(new ScrewPosition(m.Index, m.CenterX, m.CenterY, diameter, m.ScrewTypeId, m.PartNo));
        }

        logger.LogInformation("Loaded template {Path} with {Count} markers.", jsonFilePath, positions.Count);
        return new TemplateLoadResult(raw, positions, imagePath);
    }
}
