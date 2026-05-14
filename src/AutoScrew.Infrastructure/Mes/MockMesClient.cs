using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.Mes;

public sealed class MockMesClient : IMesClient
{
    public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        if (serialNumber.Trim().Length >= 3)
            return Task.FromResult(new SnValidationResult(true, "PN-DEMO", null));

        return Task.FromResult(new SnValidationResult(false, null, "SN too short."));
    }

    public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default)
    {
        var bundle = new RecipeBundle(
            partNumber,
            "demo-template.json",
            ProductImageUrl: null,
            Screws: Array.Empty<ScrewRecipeDto>());
        return Task.FromResult(bundle);
    }

    public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MesUploadResult(true, null, $"{payload.SerialNumber}:{payload.CompletedAt:O}"));
    }
}
