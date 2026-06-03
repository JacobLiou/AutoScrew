using AutoScrew.Application.Templates;
using AutoScrew.Domain.Models;

namespace AutoScrew.Application.Abstractions;

public interface ITemplateLayoutLoader
{
    /// <summary>加载模板并返回作业用「首面」扁平布局（v2 取 order 最小启用面）。</summary>
    Task<TemplateLoadResult> LoadAsync(string jsonFilePath, CancellationToken cancellationToken = default);

    /// <summary>加载完整多面产品包。</summary>
    Task<ProductTemplateLoadResult> LoadProductAsync(string jsonFilePath, CancellationToken cancellationToken = default);
}

public sealed record TemplateLoadResult(
    TemplateLayoutDto Raw,
    IReadOnlyList<ScrewPosition> Positions,
    string? ResolvedProductImagePath,
    int TotalSurfaceCount = 1,
    string? ActiveSurfaceId = null,
    string? ActiveSurfaceName = null);

public sealed record ProductTemplateLoadResult(
    ProductTemplateDto Product,
    string FilePath,
    string BaseDirectory);
