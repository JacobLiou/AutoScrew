namespace AutoScrew.Application.Abstractions;

public interface IRecipeProvisioningService
{
    Task<ProvisionedRecipe> GetProvisionedRecipeAsync(
        string serialNumber,
        string partNumber,
        CancellationToken cancellationToken = default);
}
