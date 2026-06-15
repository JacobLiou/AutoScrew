var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "MesMockServer" }));

app.MapGet("/api/sn/validate", (string sn, string stationId) =>
{
    if (string.Equals(sn, "__PING__", StringComparison.Ordinal))
        return Results.Ok(new { valid = false, partNumber = (string?)null, message = "ping ok" });

    return Results.Ok(new { valid = true, partNumber = "PN-DEMO", message = (string?)null });
});

app.MapGet("/api/recipe", (string sn, string pn, string stationId) =>
    Results.Ok(new
    {
        templateJsonPath = "demo.product-template.json",
        productImageUrl = (string?)null,
        screws = new[]
        {
            new
            {
                index = 1,
                partNo = "SCR-01",
                targetTorqueNm = 0.5,
                torqueLowerNm = 0.45,
                torqueUpperNm = 0.55,
                angleLimitDeg = 360.0,
                controllerParameterId = 1,
            },
        },
    }));

app.MapPost("/api/results", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    _ = await reader.ReadToEndAsync();
    return Results.Ok(new { accepted = true });
});

app.Run("http://localhost:5080");
