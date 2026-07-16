using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd;

/// <summary>扭矩单位换算（移植自厂商 Demo TorqUnitcoef）。</summary>
public static class TorqueUnitConverter
{
    /// <summary>Demo unit-1 系数：1 N·m ≈ 10.197 kgf·cm。</summary>
    public const double NmPerKgfCmFactor = 10.197;

    /// <summary>#555 索引 → Demo mode（1000 + index；1000=N·m 参考）。</summary>
    public static int ToDemoMode(DefaultTorqueUnit unit) => 1000 + (int)unit;

    public static double ToNm(double rawThousandths, int torqueUnitMode) =>
        rawThousandths / 1000.0 * GetCoefficient(torqueUnitMode) / GetCoefficient(1001);

    public static double ConvertNmToDisplay(double nm, int displayUnitMode) =>
        nm * GetCoefficient(displayUnitMode) / GetCoefficient(1001);

    public static double ConvertDisplayToNm(double display, int displayUnitMode) =>
        display * GetCoefficient(1001) / GetCoefficient(displayUnitMode);

    /// <summary>显示侧小数位数（与控制器面板常见 2 位对齐，如 2.50 / 0.62）。</summary>
    public const int DisplayDecimalPlaces = 2;

    /// <summary>协议 mN·m → 控制器默认显示单位（#555），四舍五入到 <see cref="DisplayDecimalPlaces"/>。</summary>
    public static double MilliNmToDisplay(int milliNm, DefaultTorqueUnit unit) =>
        Math.Round(
            milliNm / 1000.0 * GetCoefficient(ToDemoMode(unit)) / GetCoefficient(1000),
            DisplayDecimalPlaces);

    /// <summary>控制器默认显示单位 → 协议 mN·m。</summary>
    /// <param name="currentMilliNm">
    /// 当前寄存器值。若其显示结果与 <paramref name="display"/>（按 <see cref="DisplayDecimalPlaces"/> 舍入后）相同，
    /// 则原样返回，避免 282→2.50→283 这类显示量化漂移。
    /// </param>
    public static int DisplayToMilliNm(double display, DefaultTorqueUnit unit, int? currentMilliNm = null)
    {
        var rounded = Math.Round(display, DisplayDecimalPlaces);
        if (currentMilliNm is int current && MilliNmToDisplay(current, unit) == rounded)
            return current;

        return (int)Math.Round(
            rounded * GetCoefficient(1000) / GetCoefficient(ToDemoMode(unit)) * 1000.0);
    }

    public static double MilliNmToKgfCm(int milliNm) =>
        MilliNmToDisplay(milliNm, DefaultTorqueUnit.KgfCm);

    public static int KgfCmToMilliNm(double kgfCm) =>
        DisplayToMilliNm(kgfCm, DefaultTorqueUnit.KgfCm);

    public static string GetUnitSymbol(DefaultTorqueUnit unit) => unit switch
    {
        DefaultTorqueUnit.NewtonMeter => "N.m",
        DefaultTorqueUnit.KgfCm => "kgf.cm",
        DefaultTorqueUnit.LbfFt => "lbf.ft",
        DefaultTorqueUnit.LbfIn => "lbf.in",
        _ => "kgf.cm",
    };

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
