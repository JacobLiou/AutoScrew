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
    /// <summary>
    /// #100 mailbox 0xCC：Sd3_101 上 0=自动识别、1=固定自创；Sd3_43 手册为保留，恒写 0。
    /// </summary>
    public static int ToMailboxWord4(
        TighteningStrategy strategy,
        IemdSdControllerProfile profile = IemdSdControllerProfile.Sd3_101)
    {
        if (profile == IemdSdControllerProfile.Sd3_43)
            return 0;
        return strategy == TighteningStrategy.SelfDefined ? 1 : 0;
    }

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
    /// 写设备前对非活动槽清零（就地修改）。
    /// 自创：清空未配置槽（避免 off/空段以 Torque@0 下发）；已配置槽保留。
    /// </summary>
    public static void ApplyStrategyMask(IList<TighteningStageCore> stages, TighteningStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(stages);
        EnsureSixSlots(stages);

        if (strategy == TighteningStrategy.SelfDefined)
        {
            for (var i = 0; i < 6; i++)
            {
                if (!IsStageConfigured(stages[i]))
                    stages[i] = new TighteningStageCore();
            }

            return;
        }

        var active = new HashSet<int>(GetActiveStageIndices(strategy));
        for (var i = 0; i < 6; i++)
        {
            if (!active.Contains(i))
                stages[i] = new TighteningStageCore();
        }
    }

    /// <summary>
    /// 从阶段内容推断策略（#150 回读无 Strategy 字段时）。
    /// 以转速&gt;0 判定活动段；控制模式组合对齐 SDSoft <c>StrategyDetectType</c> 的标准/预定位模板。
    /// <b>加强仅认绝对槽「仅槽 3 有转速」</b>——#150 为设备布局，不得套用 SDSoft UI
    /// 「加强拧紧段重映射到 Item1」规则，否则「自创仅启动」会被误判为加强。
    /// 未命中模板 → 自创。
    /// </summary>
    /// <param name="linkedCompensationParamId">
    /// 对应 SDSoft <c>ThePrevailTorqueToBeLinked</c>；&gt;0 视为自创。
    /// </param>
    public static TighteningStrategy InferFromStages(
        IList<TighteningStageCore> stages,
        int linkedCompensationParamId = 0)
    {
        ArgumentNullException.ThrowIfNull(stages);

        // WAType[i]：有转速则为控制模式数值，否则 99（与 SDSoft 一致）。
        var waType = new int[6];
        var hasSpeed = new bool[6];
        var prevail = linkedCompensationParamId > 0;
        TighteningDirection? firstActiveDirection = null;

        for (var i = 0; i < 6; i++)
        {
            var stage = i < stages.Count ? stages[i] : null;
            var speed = stage?.SpeedRpm ?? 0;
            hasSpeed[i] = speed > 0;
            waType[i] = hasSpeed[i] ? (int)(stage!.ControlMode) : 99;

            if (stage is { CompTorqueEnabled: true })
                prevail = true;

            if (i > 3 && hasSpeed[i])
                return TighteningStrategy.SelfDefined;

            if (hasSpeed[i])
            {
                if (firstActiveDirection is null)
                    firstActiveDirection = stage!.Direction;
                else if (stage!.Direction != firstActiveDirection.Value)
                    return TighteningStrategy.SelfDefined;
            }
        }

        if (prevail)
            return TighteningStrategy.SelfDefined;

        // 标准：启动角 + 旋入角/扭/率 + 预紧扭/率 + 拧紧角或扭/夹紧，且无第 5 段转速
        if (waType[0] == 0
            && waType[1] is 0 or 1 or 2
            && waType[2] is 1 or 2
            && (waType[3] == 0 || ((waType[3] is 1 or 3 or 4) && waType[4] == 99)))
            return TighteningStrategy.Standard;

        // 加强：#150 绝对布局下仅拧紧槽 3 有转速（写加强时 ApplyStrategyMask 亦只保留槽 3）
        if (!hasSpeed[0] && !hasSpeed[1] && !hasSpeed[2] && hasSpeed[3] && !hasSpeed[4] && !hasSpeed[5])
            return TighteningStrategy.Enhanced;

        // 预定位：启动角 + 旋入角/扭/率，且槽 2–5 无转速（与写前掩码一致）
        if (waType[0] == 0
            && waType[1] is 0 or 1 or 2
            && !hasSpeed[2] && !hasSpeed[3] && !hasSpeed[4] && !hasSpeed[5])
            return TighteningStrategy.PrePosition;

        if (!hasSpeed[0] && !hasSpeed[1] && !hasSpeed[2] && !hasSpeed[3] && !hasSpeed[4] && !hasSpeed[5])
            return TighteningStrategy.Standard;

        // 未匹配标准/加强/预定位模板 → 自创（含「仅启动」等自创子集）
        return TighteningStrategy.SelfDefined;
    }

    public static TighteningStrategy InferFromCore(TighteningParameterCore core)
    {
        ArgumentNullException.ThrowIfNull(core);
        return InferFromStages(core.Stages, core.LinkedCompensationParamId);
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
