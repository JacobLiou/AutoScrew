using System.IO;
using System.Text.Json;
using AutoScrew.Application.Templates;
using AutoScrew.Hmi.Models;

namespace AutoScrew.Hmi.Services;

public static class ProductTemplateJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public static void Save(string path, ProductTemplateDocument document)
    {
        var errors = ProductTemplateValidator.Validate(document);
        if (errors.Count > 0)
            throw new InvalidDataException(string.Join(Environment.NewLine, errors));

        document.SchemaVersion = ProductTemplateDocument.CurrentSchemaVersion;
        document.SurfaceCount = document.Surfaces.Count;
        var json = JsonSerializer.Serialize(document, Options);
        File.WriteAllText(path, json);
    }

    public static ProductTemplateDocument Load(string path)
    {
        var json = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(json);
        var schema = doc.RootElement.TryGetProperty("schemaVersion", out var sv) ? sv.GetInt32() : 1;

        return schema >= 2
            ? DeserializeV2(json)
            : WrapV1(TemplateJsonSerializer.Load(path));
    }

    public static ProductTemplateDocument WrapV1(TemplateDocument v1, string? productId = null)
    {
        return new ProductTemplateDocument
        {
            SchemaVersion = ProductTemplateDocument.CurrentSchemaVersion,
            ProductId = productId ?? "IMPORTED",
            DisplayName = productId ?? "导入的单面模板",
            SurfaceCount = 1,
            AssemblySequence = ProductTemplateDocument.DefaultAssemblySequence,
            Surfaces =
            [
                FromLegacyTemplate(v1, "DEFAULT", "默认面", 1)
            ],
        };
    }

    public static ProductTemplateDocument FromDto(ProductTemplateDto dto) =>
        new()
        {
            SchemaVersion = dto.SchemaVersion,
            ProductId = dto.ProductId,
            DisplayName = dto.DisplayName,
            Revision = dto.Revision,
            SurfaceCount = dto.SurfaceCount,
            AssemblySequence = dto.AssemblySequence,
            Metadata = dto.Metadata,
            Surfaces = dto.Surfaces.Select(FromSurfaceDto).ToList(),
        };

    public static ProductTemplateDto ToDto(ProductTemplateDocument doc) =>
        new()
        {
            SchemaVersion = ProductTemplateDocument.CurrentSchemaVersion,
            ProductId = doc.ProductId,
            DisplayName = doc.DisplayName,
            Revision = doc.Revision,
            SurfaceCount = doc.Surfaces.Count,
            AssemblySequence = doc.AssemblySequence,
            Metadata = doc.Metadata,
            Surfaces = doc.Surfaces.Select(ToSurfaceDto).ToList(),
        };

    public static SurfaceLayoutDocument FromLegacyTemplate(
        TemplateDocument v1,
        string surfaceId,
        string name,
        int order) =>
        new()
        {
            SurfaceId = surfaceId,
            Name = name,
            Order = order,
            BoardWidth = v1.BoardWidth,
            BoardHeight = v1.BoardHeight,
            CircleDiameter = v1.CircleDiameter,
            ProductImageRelativePath = v1.ProductImageRelativePath,
            ProductImageAbsolutePath = v1.ProductImageAbsolutePath,
            ProductImageOpacity = v1.ProductImageOpacity,
            Markers = v1.Markers.Select(m => new MarkerRecord
            {
                Index = m.Index,
                CenterX = m.CenterX,
                CenterY = m.CenterY,
                ScrewTypeId = m.ScrewTypeId,
                CircleDiameter = m.CircleDiameter,
            }).ToList(),
        };

    private static ProductTemplateDocument DeserializeV2(string json)
    {
        var doc = JsonSerializer.Deserialize<ProductTemplateDocument>(json, Options)
                  ?? throw new InvalidDataException("Empty or invalid product template JSON.");
        return doc;
    }

    private static SurfaceLayoutDocument FromSurfaceDto(SurfaceLayoutDto dto) =>
        new()
        {
            SurfaceId = dto.SurfaceId,
            Name = dto.Name,
            Order = dto.Order,
            Enabled = dto.Enabled,
            BoardWidth = dto.BoardWidth,
            BoardHeight = dto.BoardHeight,
            CircleDiameter = dto.CircleDiameter,
            ProductImageRelativePath = dto.ProductImageRelativePath,
            ProductImageAbsolutePath = dto.ProductImageAbsolutePath,
            ProductImageOpacity = dto.ProductImageOpacity,
            Markers = dto.Markers.Select(m => new MarkerRecord
            {
                Index = m.Index,
                CenterX = m.CenterX,
                CenterY = m.CenterY,
                ScrewTypeId = m.ScrewTypeId,
                CircleDiameter = m.CircleDiameter,
            }).ToList(),
        };

    private static SurfaceLayoutDto ToSurfaceDto(SurfaceLayoutDocument surface) =>
        new()
        {
            SurfaceId = surface.SurfaceId,
            Name = surface.Name,
            Order = surface.Order,
            Enabled = surface.Enabled,
            BoardWidth = surface.BoardWidth,
            BoardHeight = surface.BoardHeight,
            CircleDiameter = surface.CircleDiameter,
            ProductImageRelativePath = surface.ProductImageRelativePath,
            ProductImageAbsolutePath = surface.ProductImageAbsolutePath,
            ProductImageOpacity = surface.ProductImageOpacity,
            Markers = surface.Markers.Select(m => new MarkerDto
            {
                Index = m.Index,
                CenterX = m.CenterX,
                CenterY = m.CenterY,
                ScrewTypeId = m.ScrewTypeId,
                CircleDiameter = m.CircleDiameter,
            }).ToList(),
        };
}
