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
            Advanced = ExtractAdvanced(raw),
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
        ApplyAdvanced(raw, core.Advanced ?? TighteningSourceAdvancedCore.CreateDefaults());
    }

    private static TighteningSourceAdvancedCore ExtractAdvanced(int[] raw)
    {
        var flags = (ushort)raw[TighteningSequenceRegisterMap.SourceAdvancedFlagsLow];
        var torqueRaw = raw[TighteningSequenceRegisterMap.SourceTorqueUnit];
        var torque = Enum.IsDefined(typeof(DefaultTorqueUnit), (ushort)torqueRaw)
            ? (DefaultTorqueUnit)(ushort)torqueRaw
            : DefaultTorqueUnit.LbfIn;

        return new TighteningSourceAdvancedCore
        {
            ProhibitLoosenAfterTightenOk = IsBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitLoosenAfterTightenOk),
            ProhibitLoosenAfterTightenNg = IsBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitLoosenAfterTightenNg),
            LimitMaxTightenNgPerScrew = IsBit(flags, TighteningSequenceRegisterMap.AdvBitLimitMaxTightenNg),
            LimitMaxLoosenNgPerScrew = IsBit(flags, TighteningSequenceRegisterMap.AdvBitLimitMaxLoosenNg),
            AutoNextOnTightenNg = IsBit(flags, TighteningSequenceRegisterMap.AdvBitAutoNextOnTightenNg),
            GoBackOnLoosenOk = IsBit(flags, TighteningSequenceRegisterMap.AdvBitGoBackOnLoosenOk),
            ProhibitStartWhenBarcodeEmpty = IsBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitStartWhenBarcodeEmpty),
            ClearBarcodeWhenScrewCountComplete = IsBit(flags, TighteningSequenceRegisterMap.AdvBitClearBarcodeWhenScrewComplete),
            ProhibitScanWhenScrewCountIncomplete = IsBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitScanWhenScrewIncomplete),
            LimitMaxRunTime = IsBit(flags, TighteningSequenceRegisterMap.AdvBitLimitMaxRunTime),
            ResetCountWhenScrewCountComplete = IsBit(flags, TighteningSequenceRegisterMap.AdvBitResetCountWhenScrewComplete),
            PromptWhenTightenSignalDisappearsEarly = IsBit(flags, TighteningSequenceRegisterMap.AdvBitPromptTightenSignalEarly),
            ProhibitStartWhenBarcodeLengthMismatch = IsBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitStartWhenBarcodeLengthMismatch),
            MaxTightenNgPerScrew = ReadDword(raw, TighteningSequenceRegisterMap.SourceMaxTightenNgLow),
            MaxLoosenNgPerScrew = ReadDword(raw, TighteningSequenceRegisterMap.SourceMaxLoosenNgLow),
            MaxRunTimeSeconds = ReadDword(raw, TighteningSequenceRegisterMap.SourceMaxRunTimeLow),
            DualToolParamSelect = raw[TighteningSequenceRegisterMap.SourceDualToolParamSelect],
            TorqueUnit = torque,
            StartConditionTool1 = ToStartCondition(raw[TighteningSequenceRegisterMap.SourceStartConditionTool1]),
            StartConditionTool2 = ToStartCondition(raw[TighteningSequenceRegisterMap.SourceStartConditionTool2]),
        };
    }

    private static void ApplyAdvanced(int[] raw, TighteningSourceAdvancedCore adv)
    {
        var flags = 0;
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitLoosenAfterTightenOk, adv.ProhibitLoosenAfterTightenOk);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitLoosenAfterTightenNg, adv.ProhibitLoosenAfterTightenNg);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitLimitMaxTightenNg, adv.LimitMaxTightenNgPerScrew);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitLimitMaxLoosenNg, adv.LimitMaxLoosenNgPerScrew);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitAutoNextOnTightenNg, adv.AutoNextOnTightenNg);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitGoBackOnLoosenOk, adv.GoBackOnLoosenOk);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitStartWhenBarcodeEmpty, adv.ProhibitStartWhenBarcodeEmpty);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitClearBarcodeWhenScrewComplete, adv.ClearBarcodeWhenScrewCountComplete);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitScanWhenScrewIncomplete, adv.ProhibitScanWhenScrewCountIncomplete);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitLimitMaxRunTime, adv.LimitMaxRunTime);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitResetCountWhenScrewComplete, adv.ResetCountWhenScrewCountComplete);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitPromptTightenSignalEarly, adv.PromptWhenTightenSignalDisappearsEarly);
        flags = SetBit(flags, TighteningSequenceRegisterMap.AdvBitProhibitStartWhenBarcodeLengthMismatch, adv.ProhibitStartWhenBarcodeLengthMismatch);

        raw[TighteningSequenceRegisterMap.SourceAdvancedFlagsLow] = flags;
        raw[TighteningSequenceRegisterMap.SourceAdvancedFlagsHigh] = 0;
        WriteDword(raw, TighteningSequenceRegisterMap.SourceMaxTightenNgLow, adv.MaxTightenNgPerScrew);
        WriteDword(raw, TighteningSequenceRegisterMap.SourceMaxLoosenNgLow, adv.MaxLoosenNgPerScrew);
        WriteDword(raw, TighteningSequenceRegisterMap.SourceMaxRunTimeLow, adv.MaxRunTimeSeconds);
        raw[TighteningSequenceRegisterMap.SourceDualToolParamSelect] = adv.DualToolParamSelect;
        raw[TighteningSequenceRegisterMap.SourceTorqueUnit] = (int)adv.TorqueUnit;
        raw[TighteningSequenceRegisterMap.SourceStartConditionTool1] = (int)adv.StartConditionTool1;
        raw[TighteningSequenceRegisterMap.SourceStartConditionTool2] = (int)adv.StartConditionTool2;
    }

    private static bool IsBit(ushort flags, int bit) => (flags & (1 << bit)) != 0;

    private static int SetBit(int flags, int bit, bool on) =>
        on ? flags | (1 << bit) : flags & ~(1 << bit);

    private static int ReadDword(int[] raw, int lowOffset) =>
        (raw[lowOffset] & 0xFFFF) | ((raw[lowOffset + 1] & 0xFFFF) << 16);

    private static void WriteDword(int[] raw, int lowOffset, int value)
    {
        raw[lowOffset] = value & 0xFFFF;
        raw[lowOffset + 1] = (value >> 16) & 0xFFFF;
    }

    private static ToolStartCondition ToStartCondition(int raw) =>
        Enum.IsDefined(typeof(ToolStartCondition), raw)
            ? (ToolStartCondition)raw
            : ToolStartCondition.PushStart;

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
