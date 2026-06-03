using AutoScrew.Application.Templates;

namespace AutoScrew.Application.Templates;

/// <summary>按 <c>surfaceOrderThenLocalIndex</c> 计算跨面全局位号。</summary>
public static class ProductTemplateSequence
{
    public static SurfaceLayoutDto GetPrimarySurface(ProductTemplateDto product) =>
        product.Surfaces
            .Where(s => s.Enabled)
            .OrderBy(s => s.Order)
            .ThenBy(s => s.SurfaceId, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault()
        ?? throw new InvalidOperationException("Product template has no enabled surfaces.");

    public static TemplateLayoutDto FlattenPrimarySurface(ProductTemplateDto product)
    {
        var surface = GetPrimarySurface(product);
        return new TemplateLayoutDto
        {
            SchemaVersion = 1,
            BoardWidth = surface.BoardWidth,
            BoardHeight = surface.BoardHeight,
            CircleDiameter = surface.CircleDiameter,
            ProductImageRelativePath = surface.ProductImageRelativePath,
            ProductImageAbsolutePath = surface.ProductImageAbsolutePath,
            ProductImageOpacity = surface.ProductImageOpacity,
            Markers = surface.Markers,
        };
    }

    public static IReadOnlyList<(SurfaceLayoutDto Surface, MarkerDto Marker, int GlobalIndex)> ExpandGlobalSequence(
        ProductTemplateDto product)
    {
        var list = new List<(SurfaceLayoutDto, MarkerDto, int)>();
        var global = 1;
        foreach (var surface in product.Surfaces.Where(s => s.Enabled).OrderBy(s => s.Order).ThenBy(s => s.SurfaceId))
        {
            foreach (var marker in surface.Markers.OrderBy(m => m.Index))
            {
                list.Add((surface, marker, global));
                global++;
            }
        }

        return list;
    }
}
