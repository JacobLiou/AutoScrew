using System.Text.Json.Serialization;

namespace AutoScrew.Infrastructure.Mes.ProductKey.Models;

/// <summary>Raw payload from GET /api/v2/container/query/getProductInfo.</summary>
public sealed class ProductInfoDto
{
    [JsonPropertyName("Container")]
    public string? Container { get; set; }

    [JsonPropertyName("Product")]
    public string? Product { get; set; }

    [JsonPropertyName("OplinkPN")]
    public string? OplinkPn { get; set; }

    [JsonPropertyName("topPN")]
    public string? TopPn { get; set; }

    [JsonPropertyName("Spec")]
    public string? Spec { get; set; }

    [JsonPropertyName("Operation")]
    public string? Operation { get; set; }

    [JsonPropertyName("MfgOrder")]
    public string? MfgOrder { get; set; }

    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    [JsonPropertyName("IsOnHold")]
    public string? IsOnHold { get; set; }

    [JsonPropertyName("Workflow")]
    public string? Workflow { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    [JsonPropertyName("ProductFamily")]
    public string? ProductFamily { get; set; }

    [JsonPropertyName("ProductSpecRev")]
    public string? ProductSpecRev { get; set; }

    [JsonPropertyName("OrderStatus")]
    public string? OrderStatus { get; set; }

    [JsonPropertyName("mlxBatchNumber")]
    public string? BatchNumber { get; set; }
}
