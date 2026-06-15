using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.Mes;

internal sealed class MesHttpClientAdapter : IMesClient
{
    private readonly MesHttpClient _inner;

    public MesHttpClientAdapter(MesHttpClient inner) => _inner = inner;

    public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
        _inner.ValidateSnAsync(serialNumber, cancellationToken);

    public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
        _inner.GetRecipeAsync(serialNumber, partNumber, cancellationToken);

    public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
        _inner.UploadResultAsync(payload, cancellationToken);
}
