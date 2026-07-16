using System.Text;

namespace UDL.Delta.IemdSd.Protocol;

public static class TighteningSequenceCodec
{
    public static TighteningSequenceCore ExtractCoreFromRaw(int[] raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        if (raw.Length != TighteningSequenceRegisterMap.BlockWordCount)
            throw new ArgumentException($"Expected {TighteningSequenceRegisterMap.BlockWordCount} words.", nameof(raw));

        var steps = new List<TighteningSequenceStepCore>(TighteningSequenceRegisterMap.MaxSteps);
        for (var i = 0; i < TighteningSequenceRegisterMap.MaxSteps; i++)
        {
            var paramId = raw[TighteningSequenceRegisterMap.ParameterIdStart + i];
            if (paramId <= 0 && i > 0)
                break;
            var qtyLow = raw[TighteningSequenceRegisterMap.QuantityStart + i * 2];
            var qtyHigh = raw[TighteningSequenceRegisterMap.QuantityStart + i * 2 + 1];
            var quantity = qtyLow | (qtyHigh << 16);
            steps.Add(new TighteningSequenceStepCore
            {
                ToolId = raw[TighteningSequenceRegisterMap.ToolIdStart + i],
                ParameterId = paramId > 0 ? paramId : 1,
                // 手册有效范围 1–999999；设备/空槽可能回 0，与写侧钳位一致。
                Quantity = quantity <= 0 ? 1 : quantity,
                BitId = raw[TighteningSequenceRegisterMap.BitIdStart + i],
            });
        }

        if (steps.Count == 0)
            steps.Add(new TighteningSequenceStepCore());

        return new TighteningSequenceCore
        {
            Name = ReadName(raw),
            NavigatorMode = (TighteningSequenceNavigatorMode)raw[TighteningSequenceRegisterMap.NavigatorMode],
            PositioningArmEnabled = raw[TighteningSequenceRegisterMap.PositioningArmEnabled] != 0,
            Steps = steps,
        };
    }

    public static void ApplyCoreToRaw(int[] raw, TighteningSequenceCore core)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(core);
        if (raw.Length != TighteningSequenceRegisterMap.BlockWordCount)
            throw new ArgumentException($"Expected {TighteningSequenceRegisterMap.BlockWordCount} words.", nameof(raw));

        Array.Clear(raw, 0, raw.Length);
        WriteName(raw, core.Name);
        raw[TighteningSequenceRegisterMap.NavigatorMode] = (int)core.NavigatorMode;
        raw[TighteningSequenceRegisterMap.PositioningArmEnabled] = core.PositioningArmEnabled ? 1 : 0;

        var count = Math.Min(core.Steps.Count, TighteningSequenceRegisterMap.MaxSteps);
        for (var i = 0; i < count; i++)
        {
            var step = core.Steps[i];
            raw[TighteningSequenceRegisterMap.ToolIdStart + i] = step.ToolId;
            raw[TighteningSequenceRegisterMap.ParameterIdStart + i] = step.ParameterId;

            // 设备拒绝数量为 0（#200 异常码 2）。
            var quantity = step.Quantity <= 0 ? 1 : Math.Min(step.Quantity, 999_999);
            raw[TighteningSequenceRegisterMap.QuantityStart + i * 2] = quantity & 0xFFFF;
            raw[TighteningSequenceRegisterMap.QuantityStart + i * 2 + 1] = (quantity >> 16) & 0xFFFF;
            raw[TighteningSequenceRegisterMap.BitIdStart + i] = Math.Clamp(step.BitId, 0, 255);
        }
    }

    public static NavigatorCoordinateCore ExtractNavigatorCoordinates(int[] raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        var list = new List<NavigatorScrewCoordinate>();
        for (var i = 0; i < TighteningSequenceRegisterMap.MaxSteps; i++)
        {
            var x = raw[i * 2];
            var y = raw[i * 2 + 1];
            if (x == 0 && y == 0 && i > 0)
                break;
            list.Add(new NavigatorScrewCoordinate { X = x, Y = y });
        }

        return new NavigatorCoordinateCore { Screws = list };
    }

    public static void ApplyNavigatorCoordinates(int[] raw, NavigatorCoordinateCore core)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(core);
        if (raw.Length != TighteningSequenceRegisterMap.NavigatorCoordinateWordCount)
            throw new ArgumentException($"Expected {TighteningSequenceRegisterMap.NavigatorCoordinateWordCount} words.", nameof(raw));

        Array.Clear(raw, 0, raw.Length);
        var count = Math.Min(core.Screws.Count, TighteningSequenceRegisterMap.MaxSteps);
        for (var i = 0; i < count; i++)
        {
            raw[i * 2] = core.Screws[i].X;
            raw[i * 2 + 1] = core.Screws[i].Y;
        }
    }

    public static NavigatorImageCodeCore ExtractNavigatorImageCodes(int[] raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        var list = new List<int>();
        for (var i = 0; i < TighteningSequenceRegisterMap.MaxSteps; i++)
        {
            if (raw[i] == 0 && i > 0)
                break;
            list.Add(raw[i]);
        }

        return new NavigatorImageCodeCore { ImageCodes = list };
    }

    public static void ApplyNavigatorImageCodes(int[] raw, NavigatorImageCodeCore core)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(core);
        if (raw.Length != TighteningSequenceRegisterMap.NavigatorImageCodeWordCount)
            throw new ArgumentException($"Expected {TighteningSequenceRegisterMap.NavigatorImageCodeWordCount} words.", nameof(raw));

        Array.Clear(raw, 0, raw.Length);
        var count = Math.Min(core.ImageCodes.Count, TighteningSequenceRegisterMap.MaxSteps);
        for (var i = 0; i < count; i++)
            raw[i] = core.ImageCodes[i];
    }

    public static PositioningArmCoordinateCore ExtractPositioningArm(int[] raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        var list = new List<PositioningArmScrewCoordinate>();
        for (var i = 0; i < TighteningSequenceRegisterMap.MaxSteps; i++)
        {
            var baseIdx = i * 6;
            if (baseIdx + 5 >= raw.Length)
                break;
            var xl = raw[baseIdx];
            var xh = raw[baseIdx + 1];
            var yl = raw[baseIdx + 2];
            var yh = raw[baseIdx + 3];
            var zl = raw[baseIdx + 4];
            var zh = raw[baseIdx + 5];
            if (xl == 0 && xh == 0 && yl == 0 && yh == 0 && zl == 0 && zh == 0 && i > 0)
                break;
            list.Add(new PositioningArmScrewCoordinate
            {
                Xmm = CombineFixedPoint(xl, xh),
                Ymm = CombineFixedPoint(yl, yh),
                Zmm = CombineFixedPoint(zl, zh),
            });
        }

        return new PositioningArmCoordinateCore { Screws = list };
    }

    public static void ApplyPositioningArm(int[] raw, PositioningArmCoordinateCore core)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(core);
        if (raw.Length != TighteningSequenceRegisterMap.PositioningArmWordCount)
            throw new ArgumentException($"Expected {TighteningSequenceRegisterMap.PositioningArmWordCount} words.", nameof(raw));

        Array.Clear(raw, 0, raw.Length);
        var count = Math.Min(core.Screws.Count, TighteningSequenceRegisterMap.MaxSteps);
        for (var i = 0; i < count; i++)
        {
            var baseIdx = i * 6;
            SplitFixedPoint(core.Screws[i].Xmm, out var xl, out var xh);
            SplitFixedPoint(core.Screws[i].Ymm, out var yl, out var yh);
            SplitFixedPoint(core.Screws[i].Zmm, out var zl, out var zh);
            raw[baseIdx] = xl;
            raw[baseIdx + 1] = xh;
            raw[baseIdx + 2] = yl;
            raw[baseIdx + 3] = yh;
            raw[baseIdx + 4] = zl;
            raw[baseIdx + 5] = zh;
        }
    }

    private static string ReadName(int[] raw)
    {
        var bytes = new List<byte>(TighteningSequenceRegisterMap.NameWordCount * 2);
        for (var i = 0; i < TighteningSequenceRegisterMap.NameWordCount; i++)
        {
            var word = (ushort)raw[TighteningSequenceRegisterMap.NameStart + i];
            bytes.Add((byte)(word & 0xFF));
            bytes.Add((byte)(word >> 8));
        }

        var end = bytes.IndexOf(0);
        if (end >= 0)
            bytes.RemoveRange(end, bytes.Count - end);
        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    private static void WriteName(int[] raw, string name)
    {
        var text = string.IsNullOrWhiteSpace(name) ? "" : name.Trim();
        if (text.Length > TighteningSequenceRegisterMap.NameWordCount * 2 - 1)
            text = text[..(TighteningSequenceRegisterMap.NameWordCount * 2 - 1)];

        var bytes = Encoding.ASCII.GetBytes(text);
        for (var i = 0; i < TighteningSequenceRegisterMap.NameWordCount; i++)
        {
            var lo = i * 2 < bytes.Length ? bytes[i * 2] : (byte)0;
            var hi = i * 2 + 1 < bytes.Length ? bytes[i * 2 + 1] : (byte)0;
            raw[TighteningSequenceRegisterMap.NameStart + i] = (hi << 8) | lo;
        }
    }

    private static double CombineFixedPoint(int low, int high) => (high << 16) | (ushort)low;

    private static void SplitFixedPoint(double value, out int low, out int high)
    {
        var bits = (int)value;
        low = bits & 0xFFFF;
        high = bits >> 16;
    }
}
