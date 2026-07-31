namespace AutoScrew.Infrastructure.Mes.ProductKey;

/// <summary>Opcenter container API（getProductInfo）运行时选项。</summary>
public sealed class ProductKeyMesOptions
{
    public string ContainerApiBaseUrl { get; init; } = "https://zuhaip.molex.com:9607";

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(100);

    /// <summary>现场常见非公有 CA；默认 true，对齐 ConsoleApp1 / 旧 CheckValidationResult。</summary>
    public bool AcceptAnyServerCertificate { get; init; } = true;
}
