namespace UDL.Delta.IemdSd.Protocol;

/// <summary>控制器默认扭矩单位（#555 / #509，手册 0–3）。</summary>
public enum DefaultTorqueUnit : ushort
{
    NewtonMeter = 0,
    KgfCm = 1,
    LbfFt = 2,
    LbfIn = 3,
}
