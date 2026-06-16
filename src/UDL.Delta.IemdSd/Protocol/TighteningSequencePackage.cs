namespace UDL.Delta.IemdSd.Protocol;

/// <summary>Full sequence package: main block + navigator / arm payloads (#200–#253).</summary>
public sealed class TighteningSequencePackage
{
    public int SequenceId { get; set; } = 1;

    public TighteningSequenceCore Core { get; set; } = new();

    public NavigatorCoordinateCore NavigatorCoordinates { get; set; } = new();

    public NavigatorImageCodeCore NavigatorImageCodes { get; set; } = new();

    public PositioningArmCoordinateCore PositioningArmCoordinates { get; set; } = new();

    public int[] MainRawBlock { get; set; } = CreateMainRawBlock();

    public int[] NavigatorRawBlock { get; set; } = CreateNavigatorRawBlock();

    public int[] NavigatorImageRawBlock { get; set; } = CreateNavigatorImageRawBlock();

    public int[] PositioningArmRawBlock { get; set; } = CreatePositioningArmRawBlock();

    public static int[] CreateMainRawBlock() => new int[TighteningSequenceRegisterMap.BlockWordCount];

    public static int[] CreateNavigatorRawBlock() => new int[TighteningSequenceRegisterMap.NavigatorCoordinateWordCount];

    public static int[] CreateNavigatorImageRawBlock() => new int[TighteningSequenceRegisterMap.NavigatorImageCodeWordCount];

    public static int[] CreatePositioningArmRawBlock() => new int[TighteningSequenceRegisterMap.PositioningArmWordCount];

    public void ApplyCoreToRaw()
    {
        TighteningSequenceCodec.ApplyCoreToRaw(MainRawBlock, Core);
        TighteningSequenceCodec.ApplyNavigatorCoordinates(NavigatorRawBlock, NavigatorCoordinates);
        TighteningSequenceCodec.ApplyNavigatorImageCodes(NavigatorImageRawBlock, NavigatorImageCodes);
        TighteningSequenceCodec.ApplyPositioningArm(PositioningArmRawBlock, PositioningArmCoordinates);
    }

    public void ExtractCoreFromRaw()
    {
        Core = TighteningSequenceCodec.ExtractCoreFromRaw(MainRawBlock);
        NavigatorCoordinates = TighteningSequenceCodec.ExtractNavigatorCoordinates(NavigatorRawBlock);
        NavigatorImageCodes = TighteningSequenceCodec.ExtractNavigatorImageCodes(NavigatorImageRawBlock);
        PositioningArmCoordinates = TighteningSequenceCodec.ExtractPositioningArm(PositioningArmRawBlock);
    }

    public TighteningSequenceTemplate ToTemplate() =>
        new()
        {
            SequenceId = SequenceId,
            RawBlock = MainRawBlock,
            Core = Core,
        };

    public static TighteningSequencePackage FromTemplate(TighteningSequenceTemplate template)
    {
        var pkg = new TighteningSequencePackage { SequenceId = template.SequenceId };
        if (template.RawBlock.Length == TighteningSequenceRegisterMap.BlockWordCount)
            Array.Copy(template.RawBlock, pkg.MainRawBlock, template.RawBlock.Length);
        if (template.Core is not null)
            pkg.Core = template.Core;
        else
            pkg.ExtractCoreFromRaw();
        return pkg;
    }
}
