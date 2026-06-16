namespace AutoScrew.Application.Abstractions;

public interface IProductTemplateSyncRepository
{
    Task<ProductTemplateSyncRecord?> GetAsync(string partNumber, CancellationToken cancellationToken = default);

    Task UpsertAsync(ProductTemplateSyncRecord record, CancellationToken cancellationToken = default);
}
