namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#751 命令 word4（Mode）分块读取曲线。</summary>
public enum CurveReadMode
{
    Scale = 10,
    AngleBlock0 = 1,
    AngleBlock1 = 21,
    AngleBlock2 = 31,
    AngleBlock3 = 41,
    TorqueBlock0 = 4,
    TorqueBlock1 = 5,
    TorqueBlock2 = 24,
    TorqueBlock3 = 25,
    TorqueBlock4 = 34,
    TorqueBlock5 = 35,
    TorqueBlock6 = 44,
    TorqueBlock7 = 45,
    Parameter = 11,
}
