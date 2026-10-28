namespace AutoScrew.Application.Abstractions;

/// <summary>最近一次拧紧周期的 #751 曲线读回诊断（不影响 OK/NG）。</summary>
public sealed record TighteningCurveDiagnostic(
    uint ReportId,
    int PointCount,
    string? EmptyReason);
