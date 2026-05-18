namespace UDL.Delta.IemdSd;

/// <summary>扭矩单位换算（移植自厂商 Demo TorqUnitcoef）。</summary>
public static class TorqueUnitConverter
{
    public static double ToNm(double rawThousandths, int torqueUnitMode) =>
        rawThousandths / 1000.0 * GetCoefficient(torqueUnitMode) / GetCoefficient(1001);

    public static double ConvertNmToDisplay(double nm, int displayUnitMode) =>
        nm * GetCoefficient(displayUnitMode) / GetCoefficient(1001);

    private static double GetCoefficient(int mode)
    {
        var unit = mode switch
        {
            0 or 1 or 2 or 3 => 1,
            1000 => 99,
            1001 => 1,
            1002 => 2,
            1003 => 3,
            1004 => 4,
            1005 => 5,
            1006 => 6,
            1050 => 50,
            _ => 99,
        };

        return unit switch
        {
            1 => 10.197,
            2 => 0.737,
            3 => 8.849,
            4 => 11.801,
            5 => 141.612,
            6 => 100.0,
            50 => 50.0,
            _ => 1.0,
        };
    }
}
