using System.Net;
using System.Text;
using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Mes;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AutoScrew.Tests;

public sealed class MesHttpClientTests
{
    [Fact]
    public async Task ValidateSnAsync_SendsStationIdAndApiKey_ReturnsParsedResult()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHttpHandler(_ =>
        {
            captured = _;
            return Json(HttpStatusCode.OK, """{"valid":true,"partNumber":"PN-001","message":null}""");
        });
        var client = CreateClient(handler, apiKey: "secret-key");

        var result = await client.ValidateSnAsync("SN-ABC");

        Assert.True(result.IsValid);
        Assert.Equal("PN-001", result.PartNumber);
        Assert.NotNull(captured);
        Assert.Contains("stationId=ST-01", captured!.RequestUri!.Query);
        Assert.Contains("sn=SN-ABC", captured.RequestUri.Query);
        Assert.Equal("secret-key", captured.Headers.GetValues("X-Api-Key").Single());
    }

    [Fact]
    public async Task GetRecipeAsync_ParsesScrewList()
    {
        var handler = new StubHttpHandler(_ => Json(HttpStatusCode.OK,
            """
            {
              "templateJsonPath": "demo.json",
              "templatePackageUrl": "api/templates/PN-1/package",
              "productImageUrl": "img.png",
              "screws": [
                { "index": 2, "partNo": "S2", "targetTorqueNm": 1.0, "torqueLowerNm": 0.9, "torqueUpperNm": 1.1, "angleLimitDeg": 180, "controllerParameterId": 3 }
              ]
            }
            """));
        var client = CreateClient(handler);

        var recipe = await client.GetRecipeAsync("SN-1", "PN-1");

        Assert.Equal("PN-1", recipe.PartNumber);
        Assert.Equal("demo.json", recipe.TemplateJsonPath);
        Assert.Equal("api/templates/PN-1/package", recipe.TemplatePackageUrl);
        Assert.Single(recipe.Screws);
        Assert.Equal(2, recipe.Screws[0].PositionIndex);
        Assert.Equal(3, recipe.Screws[0].ControllerParameterId);
    }

    [Fact]
    public async Task UploadResultAsync_ReturnsAcceptedOnSuccess()
    {
        var handler = new StubHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = CreateClient(handler);
        var payload = new LockJobResultPayload
        {
            SerialNumber = "SN-UP",
            PartNumber = "PN-UP",
            CompletedAt = DateTimeOffset.Parse("2026-06-11T10:00:00Z"),
        };

        var result = await client.UploadResultAsync(payload);

        Assert.True(result.Accepted);
        Assert.Equal("SN-UP:2026-06-11T10:00:00.0000000+00:00", result.IdempotencyKey);
    }

    [Fact]
    public async Task TestConnectionAsync_UsesHealthWhenAvailable()
    {
        var handler = new StubHttpHandler(req =>
        {
            if (req.RequestUri!.AbsolutePath.EndsWith("/api/health", StringComparison.Ordinal))
                return Json(HttpStatusCode.OK, """{"status":"ok"}""");
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        var client = CreateClient(handler);

        var result = await client.TestConnectionAsync();

        Assert.True(result.Success);
        Assert.Contains("Health OK", result.Message);
    }

    [Fact]
    public async Task TestConnectionAsync_FallsBackToValidateWhenHealth404()
    {
        var paths = new List<string>();
        var handler = new StubHttpHandler(req =>
        {
            paths.Add(req.RequestUri!.AbsolutePath);
            if (req.RequestUri.AbsolutePath.EndsWith("/api/health", StringComparison.Ordinal))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            return Json(HttpStatusCode.OK, """{"valid":false,"message":"ping"}""");
        });
        var client = CreateClient(handler);

        var result = await client.TestConnectionAsync();

        Assert.True(result.Success);
        Assert.Contains("Validate fallback", result.Message);
        Assert.Contains("/api/health", paths[0]);
        Assert.Contains("/api/sn/validate", paths[1]);
    }

    private static MesHttpClient CreateClient(HttpMessageHandler handler, string? apiKey = null)
    {
        var http = new HttpClient(handler);
        var settings = new MesRuntimeSettings
        {
            UseMockMes = false,
            BaseUrl = "http://mes.test/",
            ApiKey = apiKey,
            TimeoutSeconds = 15,
        };
        return new MesHttpClient(http, settings, "ST-01", NullLogger<MesHttpClient>.Instance);
    }

    private static HttpResponseMessage Json(HttpStatusCode code, string body) =>
        new(code) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class StubHttpHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public StubHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) => _respond = respond;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(_respond(request));
    }
}
