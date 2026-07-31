namespace AutoScrew.Infrastructure.Mes.ProductKey.Models;

/// <summary>Normalized SN key fields（兼容旧 GetProductKeyInfo 语义）。</summary>
public sealed class ProductKeyInfo
{
    public required string SerialNo { get; init; }
    public required string PartNumber { get; init; }
    public required string Spec { get; init; }
    public required string WorkOrder { get; init; }
    public required string CurrentProcess { get; init; }

    /// <summary>True when Status == "1" and not on hold.</summary>
    public bool IsAvailable { get; init; }

    public ProductInfoDto? Raw { get; init; }
}
