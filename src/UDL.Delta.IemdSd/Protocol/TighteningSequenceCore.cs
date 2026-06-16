namespace UDL.Delta.IemdSd.Protocol;

public enum TighteningSequenceNavigatorMode
{
    General = 0,
    Navigator = 1,
}

public sealed class TighteningSequenceStepCore
{
    public int ToolId { get; set; }

    public int ParameterId { get; set; } = 1;
}

public sealed class TighteningSequenceCore
{
    public string Name { get; set; } = "";

    public TighteningSequenceNavigatorMode NavigatorMode { get; set; } = TighteningSequenceNavigatorMode.General;

    public bool PositioningArmEnabled { get; set; }

    public List<TighteningSequenceStepCore> Steps { get; set; } = [new()];
}

public sealed class NavigatorCoordinateCore
{
    public List<NavigatorScrewCoordinate> Screws { get; set; } = [];
}

public sealed class NavigatorScrewCoordinate
{
    public int X { get; set; }

    public int Y { get; set; }
}

public sealed class NavigatorImageCodeCore
{
    public List<int> ImageCodes { get; set; } = [];
}

public sealed class PositioningArmCoordinateCore
{
    public List<PositioningArmScrewCoordinate> Screws { get; set; } = [];
}

public sealed class PositioningArmScrewCoordinate
{
    public double Xmm { get; set; }

    public double Ymm { get; set; }

    public double Zmm { get; set; }
}
