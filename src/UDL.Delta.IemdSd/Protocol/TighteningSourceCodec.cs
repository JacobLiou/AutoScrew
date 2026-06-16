using System.Text;

namespace UDL.Delta.IemdSd.Protocol;

public static class TighteningSourceCodec
{
    public static TighteningSourceContentCore ExtractContentFromRaw(int[] raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        if (raw.Length < TighteningSequenceRegisterMap.SourceContentWordCount)
            throw new ArgumentException($"Expected at least {TighteningSequenceRegisterMap.SourceContentWordCount} words.", nameof(raw));

        var screwLow = raw[TighteningSequenceRegisterMap.SourceScrewCountLow];
        var screwHigh = raw[TighteningSequenceRegisterMap.SourceScrewCountHigh];
        return new TighteningSourceContentCore
        {
            Barcode = ReadBarcode(raw),
            BindingType = (TighteningSourceBindingType)raw[TighteningSequenceRegisterMap.SourceType],
            TargetId = raw[TighteningSequenceRegisterMap.SourceTargetId],
            ScrewCount = screwLow | (screwHigh << 16),
            BitId = raw[TighteningSequenceRegisterMap.SourceBitId],
        };
    }

    public static void ApplyContentToRaw(int[] raw, TighteningSourceContentCore core)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(core);
        if (raw.Length < TighteningSequenceRegisterMap.SourceContentWordCount)
            throw new ArgumentException($"Expected at least {TighteningSequenceRegisterMap.SourceContentWordCount} words.", nameof(raw));

        Array.Clear(raw, 0, raw.Length);
        WriteBarcode(raw, core.Barcode);
        raw[TighteningSequenceRegisterMap.SourceType] = (int)core.BindingType;
        raw[TighteningSequenceRegisterMap.SourceTargetId] = core.TargetId;
        raw[TighteningSequenceRegisterMap.SourceScrewCountLow] = core.ScrewCount & 0xFFFF;
        raw[TighteningSequenceRegisterMap.SourceScrewCountHigh] = (core.ScrewCount >> 16) & 0xFFFF;
        raw[TighteningSequenceRegisterMap.SourceBitId] = core.BitId;
    }

    private static string ReadBarcode(int[] raw)
    {
        var bytes = new List<byte>(TighteningSequenceRegisterMap.SourceBarcodeWordCount * 2);
        for (var i = 0; i < TighteningSequenceRegisterMap.SourceBarcodeWordCount; i++)
        {
            var word = (ushort)raw[i];
            bytes.Add((byte)(word & 0xFF));
            bytes.Add((byte)(word >> 8));
        }

        var end = bytes.IndexOf(0);
        if (end >= 0)
            bytes.RemoveRange(end, bytes.Count - end);
        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    private static void WriteBarcode(int[] raw, string barcode)
    {
        var text = barcode ?? "";
        if (text.Length > TighteningSequenceRegisterMap.SourceBarcodeWordCount * 2 - 1)
            text = text[..(TighteningSequenceRegisterMap.SourceBarcodeWordCount * 2 - 1)];

        var bytes = Encoding.ASCII.GetBytes(text);
        for (var i = 0; i < TighteningSequenceRegisterMap.SourceBarcodeWordCount; i++)
        {
            var lo = i * 2 < bytes.Length ? bytes[i * 2] : (byte)0;
            var hi = i * 2 + 1 < bytes.Length ? bytes[i * 2 + 1] : (byte)0;
            raw[i] = (hi << 8) | lo;
        }
    }
}
