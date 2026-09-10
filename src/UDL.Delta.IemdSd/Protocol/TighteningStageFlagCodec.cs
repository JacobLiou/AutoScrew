namespace UDL.Delta.IemdSd.Protocol;

/// <summary>
/// 阶段控制字 / 进阶字位域编解码（对齐 SDSoft TransferCSV / GlobalVar）。
/// 兼容旧版「word0=枚举、word1=方向」的简化落盘。
/// </summary>
internal static class TighteningStageFlagCodec
{
    public const int BitDirection = 6;          // word0 bit6
    public const int BitClampMode = 11;         // word0 bit11
    public const int BitTwoStage = 13;          // word0 bit13 = 0x2000
    public const int MaskStageTarget = 0x30;    // word0 bits4–5
    public const int BitErgoStopInAdvanced = 2;  // AdvancedSetting bit2 → word1

    /// <summary>阶段内 limit/enable 镜像字（SDSoft 读两阶段时检查 +46 bit13）。</summary>
    public const int StageEnableFlagsOffset = 46;

    public static void DecodeControlWord(
        ushort word0,
        ushort word1,
        out TighteningControlMode mode,
        out TighteningDirection direction,
        out bool twoStage,
        out bool ergoStop)
    {
        if (IsLegacyPlainControlWord(word0))
        {
            mode = (TighteningControlMode)word0;
            direction = word1 <= 1
                ? (TighteningDirection)word1
                : DecodeDirectionFromPacked(word0, word1);
            twoStage = false;
            ergoStop = DecodeErgoStop(word1);
            return;
        }

        mode = DecodeControlMode(word0);
        direction = ((word0 >> BitDirection) & 1) != 0
            ? TighteningDirection.CounterClockwise
            : TighteningDirection.Clockwise;
        twoStage = (word0 & (1 << BitTwoStage)) != 0;
        ergoStop = DecodeErgoStop(word1);
    }

    public static void EncodeControlWords(
        TighteningControlMode mode,
        TighteningDirection direction,
        bool twoStage,
        bool ergoStop,
        out ushort word0,
        out ushort word1)
    {
        word0 = EncodeControlModeBits(mode);
        if (direction == TighteningDirection.CounterClockwise)
            word0 |= (ushort)(1 << BitDirection);
        if (twoStage)
            word0 |= (ushort)(1 << BitTwoStage);

        // AdvancedSetting_L：bit2 = Ergo Stop；写入 word1 的 SDSoft 映射
        ushort advanced = 0;
        if (ergoStop)
            advanced |= (ushort)(1 << BitErgoStopInAdvanced);
        word1 = EncodeAdvancedSetting(advanced);
    }

    public static void ApplyTwoStageEnableMirror(int[] raw, int stageBase, bool twoStage)
    {
        var idx = stageBase + StageEnableFlagsOffset;
        if (idx < 0 || idx >= raw.Length)
            return;

        if (twoStage)
            raw[idx] = (ushort)(raw[idx] | (1 << BitTwoStage));
        else
            raw[idx] = (ushort)(raw[idx] & ~(1 << BitTwoStage));
    }

    public static bool ReadTwoStageEnableMirror(int[] raw, int stageBase)
    {
        var idx = stageBase + StageEnableFlagsOffset;
        if (idx < 0 || idx >= raw.Length)
            return false;
        return (raw[idx] & (1 << BitTwoStage)) != 0;
    }

    private static bool IsLegacyPlainControlWord(ushort word0) =>
        word0 <= (ushort)TighteningControlMode.ClampAngle
        && (word0 & ~(ushort)0x7) == 0
        && (word0 & MaskStageTarget) == 0
        && (word0 & (1 << BitClampMode)) == 0
        && (word0 & (1 << BitTwoStage)) == 0
        && (word0 & (1 << BitDirection)) == 0;

    private static TighteningControlMode DecodeControlMode(ushort word0)
    {
        var stageTarget = (word0 & MaskStageTarget) >> 4;
        var clamp = (word0 & (1 << BitClampMode)) != 0;
        return (stageTarget, clamp) switch
        {
            (0, false) => TighteningControlMode.Angle,
            (1, false) => TighteningControlMode.Torque,
            (2, false) => TighteningControlMode.TorqueRate,
            (1, true) => TighteningControlMode.ClampTorque,
            (0, true) => TighteningControlMode.ClampAngle,
            _ => TighteningControlMode.Angle,
        };
    }

    private static ushort EncodeControlModeBits(TighteningControlMode mode) =>
        mode switch
        {
            TighteningControlMode.Torque => 0x10,
            TighteningControlMode.TorqueRate => 0x20,
            TighteningControlMode.ClampTorque => (ushort)(0x10 | (1 << BitClampMode)),
            TighteningControlMode.ClampAngle => (ushort)(1 << BitClampMode),
            _ => 0,
        };

    private static TighteningDirection DecodeDirectionFromPacked(ushort word0, ushort word1)
    {
        if ((word0 & (1 << BitDirection)) != 0)
            return TighteningDirection.CounterClockwise;
        // 旧数据方向曾整字落在 word1
        return word1 == 1 ? TighteningDirection.CounterClockwise : TighteningDirection.Clockwise;
    }

    private static bool DecodeErgoStop(ushort word1)
    {
        var advanced = DecodeAdvancedSetting(word1);
        return (advanced & (1 << BitErgoStopInAdvanced)) != 0;
    }

    /// <summary>SDSoft：AdvancedSetting ← word1 位重排。</summary>
    private static ushort DecodeAdvancedSetting(ushort word1)
    {
        ushort advanced = (ushort)(word1 & 0x7);
        advanced |= (ushort)((word1 & 0x30) >> 1);
        advanced |= (ushort)((word1 & 0x100) >> 3);
        return advanced;
    }

    /// <summary>SDSoft：word1 ← AdvancedSetting 位重排。</summary>
    private static ushort EncodeAdvancedSetting(ushort advanced)
    {
        ushort word1 = (ushort)(advanced & 0x7);
        word1 |= (ushort)((advanced & 0x18) << 1);
        word1 |= (ushort)((advanced & 0x20) << 3);
        return word1;
    }
}
