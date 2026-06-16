using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class TighteningSequenceCodecTests
{
    [Fact]
    public void Sequence_roundtrip_preserves_core_fields()
    {
        var raw = TighteningSequencePackage.CreateMainRawBlock();
        var core = new TighteningSequenceCore
        {
            Name = "Line-A",
            NavigatorMode = TighteningSequenceNavigatorMode.Navigator,
            PositioningArmEnabled = true,
            Steps =
            [
                new TighteningSequenceStepCore { ToolId = 0, ParameterId = 3 },
                new TighteningSequenceStepCore { ToolId = 1, ParameterId = 7 },
            ],
        };

        TighteningSequenceCodec.ApplyCoreToRaw(raw, core);
        var decoded = TighteningSequenceCodec.ExtractCoreFromRaw(raw);

        Assert.Equal("Line-A", decoded.Name);
        Assert.Equal(TighteningSequenceNavigatorMode.Navigator, decoded.NavigatorMode);
        Assert.True(decoded.PositioningArmEnabled);
        Assert.Equal(2, decoded.Steps.Count);
        Assert.Equal(3, decoded.Steps[0].ParameterId);
        Assert.Equal(7, decoded.Steps[1].ParameterId);
    }

    [Fact]
    public void Sequence_block_word_count_matches_manual()
    {
        Assert.Equal(530, TighteningSequenceRegisterMap.BlockWordCount);
        Assert.Equal(530, TighteningSequenceTemplate.SequenceBlockWordCount);
    }

    [Fact]
    public void Navigator_coordinates_roundtrip()
    {
        var raw = TighteningSequencePackage.CreateNavigatorRawBlock();
        var core = new NavigatorCoordinateCore
        {
            Screws =
            [
                new NavigatorScrewCoordinate { X = 100, Y = 200 },
                new NavigatorScrewCoordinate { X = 300, Y = 400 },
            ],
        };

        TighteningSequenceCodec.ApplyNavigatorCoordinates(raw, core);
        var decoded = TighteningSequenceCodec.ExtractNavigatorCoordinates(raw);
        Assert.Equal(2, decoded.Screws.Count);
        Assert.Equal(100, decoded.Screws[0].X);
        Assert.Equal(400, decoded.Screws[1].Y);
    }

    [Fact]
    public void Source_content_roundtrip()
    {
        var raw = new int[TighteningSequenceRegisterMap.SourceContentWordCount];
        var core = new TighteningSourceContentCore
        {
            Barcode = "SN123",
            BindingType = TighteningSourceBindingType.Sequence,
            TargetId = 5,
            ScrewCount = 12,
            BitId = 2,
        };

        TighteningSourceCodec.ApplyContentToRaw(raw, core);
        var decoded = TighteningSourceCodec.ExtractContentFromRaw(raw);

        Assert.Equal("SN123", decoded.Barcode);
        Assert.Equal(TighteningSourceBindingType.Sequence, decoded.BindingType);
        Assert.Equal(5, decoded.TargetId);
        Assert.Equal(12, decoded.ScrewCount);
        Assert.Equal(2, decoded.BitId);
    }
}
