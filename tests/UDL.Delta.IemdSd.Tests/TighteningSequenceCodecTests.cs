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
                new TighteningSequenceStepCore { ToolId = 0, ParameterId = 3, Quantity = 6, BitId = 2 },
                new TighteningSequenceStepCore { ToolId = 1, ParameterId = 7, Quantity = 1, BitId = 0 },
            ],
        };

        TighteningSequenceCodec.ApplyCoreToRaw(raw, core);
        var decoded = TighteningSequenceCodec.ExtractCoreFromRaw(raw);

        Assert.Equal("Line-A", decoded.Name);
        Assert.Equal(TighteningSequenceNavigatorMode.Navigator, decoded.NavigatorMode);
        Assert.True(decoded.PositioningArmEnabled);
        Assert.Equal(2, decoded.Steps.Count);
        Assert.Equal(3, decoded.Steps[0].ParameterId);
        Assert.Equal(6, decoded.Steps[0].Quantity);
        Assert.Equal(2, decoded.Steps[0].BitId);
        Assert.Equal(7, decoded.Steps[1].ParameterId);
        Assert.Equal(1, decoded.Steps[1].Quantity);
        Assert.Equal(0, decoded.Steps[1].BitId);
    }

    [Fact]
    public void Sequence_quantity_uses_dword_at_0x1B8()
    {
        var raw = TighteningSequencePackage.CreateMainRawBlock();
        // Absolute 0x1B8 / 0x1B9 relative to block base 0xD2
        var qtyIndex = TighteningSequenceRegisterMap.QuantityStart;
        raw[TighteningSequenceRegisterMap.ParameterIdStart] = 12;
        raw[qtyIndex] = 0x86A0; // 100000 & 0xFFFF
        raw[qtyIndex + 1] = 0x1; // 100000 >> 16
        raw[TighteningSequenceRegisterMap.BitIdStart] = 7;

        var decoded = TighteningSequenceCodec.ExtractCoreFromRaw(raw);
        Assert.Equal(100_000, decoded.Steps[0].Quantity);
        Assert.Equal(7, decoded.Steps[0].BitId);
    }

    [Fact]
    public void Sequence_apply_clamps_zero_quantity_to_one()
    {
        var raw = TighteningSequencePackage.CreateMainRawBlock();
        var core = new TighteningSequenceCore
        {
            Name = "Q",
            Steps = [new TighteningSequenceStepCore { ParameterId = 1, Quantity = 0 }],
        };

        TighteningSequenceCodec.ApplyCoreToRaw(raw, core);
        Assert.Equal(1, raw[TighteningSequenceRegisterMap.QuantityStart]);
        Assert.Equal(0, raw[TighteningSequenceRegisterMap.QuantityStart + 1]);
    }

    [Fact]
    public void Sequence_block_word_count_matches_manual()
    {
        Assert.Equal(530, TighteningSequenceRegisterMap.BlockWordCount);
        Assert.Equal(530, TighteningSequenceTemplate.SequenceBlockWordCount);
        Assert.Equal(0x1B8 - 0xD2, TighteningSequenceRegisterMap.QuantityStart);
        Assert.Equal(0x280 - 0xD2, TighteningSequenceRegisterMap.BitIdStart);
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
            Advanced = new TighteningSourceAdvancedCore
            {
                ProhibitLoosenAfterTightenOk = true,
                LimitMaxTightenNgPerScrew = true,
                MaxTightenNgPerScrew = 7,
                LimitMaxLoosenNgPerScrew = false,
                MaxLoosenNgPerScrew = 3,
                AutoNextOnTightenNg = true,
                ResetCountWhenScrewCountComplete = true,
                LimitMaxRunTime = true,
                MaxRunTimeSeconds = 3600,
                ProhibitStartWhenBarcodeLengthMismatch = true,
                TorqueUnit = DefaultTorqueUnit.LbfIn,
                StartConditionTool1 = ToolStartCondition.DiOrPush,
                StartConditionTool2 = ToolStartCondition.DigitalDi,
            },
        };

        TighteningSourceCodec.ApplyContentToRaw(raw, core);
        var decoded = TighteningSourceCodec.ExtractContentFromRaw(raw);

        Assert.Equal("SN123", decoded.Barcode);
        Assert.Equal(TighteningSourceBindingType.Sequence, decoded.BindingType);
        Assert.Equal(5, decoded.TargetId);
        Assert.Equal(12, decoded.ScrewCount);
        Assert.Equal(2, decoded.BitId);
        Assert.True(decoded.Advanced.ProhibitLoosenAfterTightenOk);
        Assert.True(decoded.Advanced.LimitMaxTightenNgPerScrew);
        Assert.Equal(7, decoded.Advanced.MaxTightenNgPerScrew);
        Assert.False(decoded.Advanced.LimitMaxLoosenNgPerScrew);
        Assert.Equal(3, decoded.Advanced.MaxLoosenNgPerScrew);
        Assert.True(decoded.Advanced.AutoNextOnTightenNg);
        Assert.True(decoded.Advanced.ResetCountWhenScrewCountComplete);
        Assert.Equal(3600, decoded.Advanced.MaxRunTimeSeconds);
        Assert.True(decoded.Advanced.ProhibitStartWhenBarcodeLengthMismatch);
        Assert.Equal(DefaultTorqueUnit.LbfIn, decoded.Advanced.TorqueUnit);
        Assert.Equal(ToolStartCondition.DiOrPush, decoded.Advanced.StartConditionTool1);
        Assert.Equal(ToolStartCondition.DigitalDi, decoded.Advanced.StartConditionTool2);
        Assert.Equal(1 << TighteningSequenceRegisterMap.AdvBitProhibitLoosenAfterTightenOk
                     | 1 << TighteningSequenceRegisterMap.AdvBitLimitMaxTightenNg
                     | 1 << TighteningSequenceRegisterMap.AdvBitAutoNextOnTightenNg
                     | 1 << TighteningSequenceRegisterMap.AdvBitLimitMaxRunTime
                     | 1 << TighteningSequenceRegisterMap.AdvBitResetCountWhenScrewComplete
                     | 1 << TighteningSequenceRegisterMap.AdvBitProhibitStartWhenBarcodeLengthMismatch,
            raw[TighteningSequenceRegisterMap.SourceAdvancedFlagsLow]);
    }
}
