namespace AutoScrew.Application.Abstractions;

/// <summary>智能电批控制器返回的最近一次拧紧结果摘要（产线 OK/NG 以此为准）。</summary>
public sealed record LockHardwareOutcome(
    bool DeviceOk,
    double FinalTorqueNm,
    double FinalAngleDeg,
    ushort? DeviceErrorCode,
    uint ReportId);
