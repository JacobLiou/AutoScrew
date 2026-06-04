namespace UDL.Delta.IemdSd.Protocol;

/// <summary>拧紧阶段控制模式（手册 A.3.1 FA 等）。</summary>
public enum TighteningControlMode : ushort
{
    Angle = 0,
    Torque = 1,
    TorqueRate = 2,
    ClampTorque = 3,
    ClampAngle = 4,
}
