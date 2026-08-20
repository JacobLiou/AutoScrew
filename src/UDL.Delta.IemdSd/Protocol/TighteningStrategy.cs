namespace UDL.Delta.IemdSd.Protocol;

/// <summary>
/// 设备 CH05 锁附策略。不写入 349-word 参数块；写设备时通过 #100 mailbox 0xCC 控制识别方式。
/// </summary>
public enum TighteningStrategy : ushort
{
    /// <summary>标准：启动 / 旋入 / 预紧 / 拧紧（槽 0–3）。</summary>
    Standard = 0,

    /// <summary>加强：仅拧紧阶段（槽 3）。</summary>
    Enhanced = 1,

    /// <summary>预定位：启动 + 旋入（槽 0–1）。</summary>
    PrePosition = 2,

    /// <summary>自创：最多 6 阶段；写设备时 0xCC=1 强制自创。</summary>
    SelfDefined = 3,
}

/// <summary>策略 ↔ 阶段槽位映射、写前掩码、设备回读推断。</summary>
public static class TighteningStrategyHelper
{
    /// <summary>#100 mailbox 0xCC：0=自动识别策略，1=固定自创。</summary>
    public static int ToMailboxWord4(TighteningStrategy strategy) =>
        strategy == TighteningStrategy.SelfDefined ? 1 : 0;

    public static IReadOnlyList<int> GetActiveStageIndices(TighteningStrategy strategy) =>
        strategy switch
        {
            TighteningStrategy.Standard => [0, 1, 2, 3],
            TighteningStrategy.Enhanced => [3],
            TighteningStrategy.PrePosition => [0, 1],
            TighteningStrategy.SelfDefined => [0, 1, 2, 3, 4, 5],
            _ => [0, 1, 2, 3],
        };

    public static bool IsStageActive(TighteningStrategy strategy, int stageIndex)
    {
        if (stageIndex is < 0 or > 5)
            return false;
        return strategy switch
        {
            TighteningStrategy.Standard => stageIndex <= 3,
            TighteningStrategy.Enhanced => stageIndex == 3,
            TighteningStrategy.PrePosition => stageIndex <= 1,
            TighteningStrategy.SelfDefined => true,
            _ => stageIndex <= 3,
        };
    }

    /// <summary>
    /// 写设备前对非活动槽清零（就地修改）。自创仅清未配置的尾部槽以外的空槽策略：
    /// 自创保留全部已有内容，不强制清零（0xCC=1 固定自创）。
    /// </summary>
    public static void ApplyStrategyMask(IList<TighteningStageCore> stages, TighteningStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(stages);
        EnsureSixSlots(stages);

        if (strategy == TighteningStrategy.SelfDefined)
            return;

        var active = new HashSet<int>(GetActiveStageIndices(strategy));
        for (var i = 0; i < 6; i++)
        {
            if (!active.Contains(i))
                stages[i] = new TighteningStageCore();
        }
    }

    /// <summary>从阶段内容推断策略（#150 回读无 Strategy 字段时）。</summary>
    public static TighteningStrategy InferFromStages(IList<TighteningStageCore> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        var configured = new bool[6];
        for (var i = 0; i < 6 && i < stages.Count; i++)
            configured[i] = IsStageConfigured(stages[i]);

        if (configured[4] || configured[5])
            return TighteningStrategy.SelfDefined;

        var c0 = configured[0];
        var c1 = configured[1];
        var c2 = configured[2];
        var c3 = configured[3];

        if (!c0 && !c1 && !c2 && !c3)
            return TighteningStrategy.Standard;

        // 仅拧紧槽 → 加强
        if (!c0 && !c1 && !c2 && c3)
            return TighteningStrategy.Enhanced;

        // 仅启动+旋入 → 预定位
        if (c0 && c1 && !c2 && !c3)
            return TighteningStrategy.PrePosition;

        // 槽 0–3 有内容且无 4/5 → 标准（含部分标准槽）
        if (c0 || c1 || c2 || c3)
            return TighteningStrategy.Standard;

        return TighteningStrategy.SelfDefined;
    }

    public static bool IsStageConfigured(TighteningStageCore? stage)
    {
        if (stage is null)
            return false;
        return stage.SpeedRpm > 0
            || stage.TargetTorqueMilliNm > 0
            || stage.TargetAngleDeg > 0
            || stage.TargetTorqueRate > 0
            || stage.MaxClampTorqueMilliNm > 0
            || stage.MaxClampAngleDeg > 0;
    }

    private static void EnsureSixSlots(IList<TighteningStageCore> stages)
    {
        while (stages.Count < 6)
            stages.Add(new TighteningStageCore());
    }
}
