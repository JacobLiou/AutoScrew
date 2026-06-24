using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Hardware;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ControllerSourceConfigProjectionTests
{
    [Fact]
    public void ToPrimaryContent_SingleTool_PrefersTool0Binding()
    {
        var bindings = new List<ControllerSourceBindingEntry>
        {
            new() { ToolIndex = 0, BindingType = 1, TargetId = 3, ScrewCount = 8 },
            new() { ToolIndex = 1, BindingType = 1, TargetId = 5, ScrewCount = 4 },
        };

        var content = ControllerSourceConfigProjection.ToPrimaryContent(TighteningOperatingMode.SingleTool, bindings);

        Assert.Equal(3, content.TargetId);
        Assert.Equal(8, content.ScrewCount);
        Assert.Equal(TighteningSourceBindingType.Sequence, content.BindingType);
    }

    [Fact]
    public void FromLegacyContent_RoundTripsPrimaryFields()
    {
        var legacy = new TighteningSourceContentCore
        {
            ToolIndex = 0,
            BindingType = TighteningSourceBindingType.Sequence,
            TargetId = 12,
            ScrewCount = 6,
            BitId = 2,
        };

        var bindings = ControllerSourceConfigProjection.FromLegacyContent(legacy);
        var projected = ControllerSourceConfigProjection.ToPrimaryContent(
            TighteningOperatingMode.DualToolAlternation,
            bindings,
            legacy);

        Assert.Equal(12, projected.TargetId);
        Assert.Equal(6, projected.ScrewCount);
        Assert.Equal(2, projected.BitId);
    }

    [Fact]
    public void DeserializeLegacySourceJson_WithoutBindings_StillLoads()
    {
        var json = """
                   {
                     "ProductionControlMode": 0,
                     "Mode": { "ToolIndex": 0, "OperatingMode": 0, "SwitchingMethod": 0 },
                     "Content": { "BindingType": 1, "TargetId": 7, "ScrewCount": 3, "BitId": 0, "Barcode": "" }
                   }
                   """;

        var doc = System.Text.Json.JsonSerializer.Deserialize<ControllerSourceConfigDocument>(json);

        Assert.NotNull(doc);
        Assert.Equal(7, doc!.Content.TargetId);
        Assert.Empty(doc.Bindings);
    }
}
