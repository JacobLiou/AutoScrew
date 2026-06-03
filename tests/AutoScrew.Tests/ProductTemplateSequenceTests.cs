using AutoScrew.Application.Templates;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateSequenceTests
{
    [Fact]
    public void GetPrimarySurface_UsesLowestOrderEnabledSurface()
    {
        var product = new ProductTemplateDto
        {
            SurfaceCount = 2,
            Surfaces =
            [
                new SurfaceLayoutDto { SurfaceId = "S2", Name = "底面", Order = 2, Enabled = true },
                new SurfaceLayoutDto { SurfaceId = "S1", Name = "顶面", Order = 1, Enabled = true },
            ],
        };

        var primary = ProductTemplateSequence.GetPrimarySurface(product);

        Assert.Equal("S1", primary.SurfaceId);
    }

    [Fact]
    public void ExpandGlobalSequence_AssignsGlobalIndexAcrossSurfaces()
    {
        var product = new ProductTemplateDto
        {
            SurfaceCount = 2,
            Surfaces =
            [
                new SurfaceLayoutDto
                {
                    SurfaceId = "S1",
                    Order = 1,
                    Markers =
                    [
                        new MarkerDto { Index = 1, CenterX = 1, CenterY = 1 },
                        new MarkerDto { Index = 2, CenterX = 2, CenterY = 2 },
                    ],
                },
                new SurfaceLayoutDto
                {
                    SurfaceId = "S2",
                    Order = 2,
                    Markers = [new MarkerDto { Index = 1, CenterX = 3, CenterY = 3 }],
                },
            ],
        };

        var sequence = ProductTemplateSequence.ExpandGlobalSequence(product);

        Assert.Equal(3, sequence.Count);
        Assert.Equal(1, sequence[0].GlobalIndex);
        Assert.Equal("S1", sequence[0].Surface.SurfaceId);
        Assert.Equal(2, sequence[1].GlobalIndex);
        Assert.Equal(3, sequence[2].GlobalIndex);
        Assert.Equal("S2", sequence[2].Surface.SurfaceId);
    }
}
