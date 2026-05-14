namespace AutoScrew.Domain.Curves;

public readonly record struct LockEvaluationResult(bool IsOk, NgReason Reason, string? ErrorCode, string? Message)
{
    public static LockEvaluationResult Ok() => new(true, NgReason.None, null, null);

    public static LockEvaluationResult Ng(NgReason reason, string errorCode, string? message = null) =>
        new(false, reason, errorCode, message);
}
