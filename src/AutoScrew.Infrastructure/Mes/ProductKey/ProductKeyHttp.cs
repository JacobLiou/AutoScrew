using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AutoScrew.Infrastructure.Mes.ProductKey;

public static class ProductKeyHttp
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static HttpClient CreateClient(ProductKeyMesOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var handler = new SocketsHttpHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        };

        if (options.AcceptAnyServerCertificate)
        {
            handler.SslOptions.RemoteCertificateValidationCallback = static (_, _, _, _) => true;
        }

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(TrimSlash(options.ContainerApiBaseUrl) + "/"),
            Timeout = options.Timeout,
        };
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private static string TrimSlash(string url) => url.TrimEnd('/');
}
