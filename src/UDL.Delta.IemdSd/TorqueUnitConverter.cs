namespace UDL.Delta.IemdSd;

/// <summary>扭矩单位换算（移植自厂商 Demo TorqUnitcoef）。</summary>
public static class TorqueUnitConverter
{
    /// <summary>Demo unit-1 系数：1 N·m ≈ 10.197 kgf·cm。</summary>
    public const double NmPerKgfCmFactor = 10.197;

    public static double ToNm(double rawThousandths, int torqueUnitMode) =>
        rawThousandths / 1000.0 * GetCoefficient(torqueUnitMode) / GetCoefficient(1001);

    public static double ConvertNmToDisplay(double nm, int displayUnitMode) =>
        nm * GetCoefficient(displayUnitMode) / GetCoefficient(1001);

    public static double ConvertDisplayToNm(double display, int displayUnitMode) =>
        display * GetCoefficient(1001) / GetCoefficient(displayUnitMode);

    public static double MilliNmToKgfCm(int milliNm) => milliNm / 1000.0 * NmPerKgfCmFactor;

    public static int KgfCmToMilliNm(double kgfCm) =>
        (int)Math.Round(kgfCm / NmPerKgfCmFactor * 1000.0);

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
