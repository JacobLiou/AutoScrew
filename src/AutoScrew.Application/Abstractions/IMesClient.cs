namespace AutoScrew.Application.Abstractions;

public interface IMesClient
{
    Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default);

    Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default);

    Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default);
}

public sealed record SnValidationResult(bool IsValid, string? PartNumber, string? Message);

public sealed record RecipeBundle(
    string PartNumber,
    string? TemplateJsonPath,
    string? ProductImageUrl,
    IReadOnlyList<ScrewRecipeDto> Screws,
    string? TemplatePackageUrl = null);

public sealed record ScrewRecipeDto(
    int PositionIndex,
    string? PartNo,
    double TargetTorqueNm,
    double TorqueLowerNm,
    double TorqueUpperNm,
    double AngleLimitDeg,
    int? ControllerParameterId = null);

public sealed record MesUploadResult(bool Accepted, string? Message, string? IdempotencyKey);

public sealed class LockJobResultPayload
{
    public string SerialNumber { get; set; } = "";

    public string PartNumber { get; set; } = "";

    public string StationId { get; set; } = "";

    /// <summary>本机首选 IPv4；可空。</summary>
    public string? HostIp { get; set; }

    /// <summary>本机 MAC，规范化 AA-BB-CC-DD-EE-FF；可空。</summary>
    public string? HostMac { get; set; }

    public string OperatorId { get; set; } = "";

    public bool IsRework { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset CompletedAt { get; set; }

    public string OverallResult { get; set; } = "";

    public List<ScrewResultDto> Screws { get; set; } = new();

    public string? LockLogJson { get; set; }
}

public sealed class ScrewResultDto
{
    public int PositionIndex { get; set; }

    public string Result { get; set; } = "";

    public string? ErrorCode { get; set; }

    public double? FinalTorqueNm { get; set; }

    public double? FinalAngleDeg { get; set; }

    public string? CurveRelativePath { get; set; }
}
