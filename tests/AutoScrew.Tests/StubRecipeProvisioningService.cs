using AutoScrew.Application.Abstractions;

namespace AutoScrew.Tests;

internal sealed class StubRecipeProvisioningService : IRecipeProvisioningService
{
    private readonly string _templatePath;
    private readonly string _partNumber;

    public StubRecipeProvisioningService(string templatePath, string partNumber = "PNDEMO")
    {
        _templatePath = templatePath;
        _partNumber = partNumber;
    }

    public Task<ProvisionedRecipe> GetProvisionedRecipeAsync(
        string serialNumber,
        string partNumber,
        CancellationToken cancellationToken = default)
    {
        var pn = string.IsNullOrWhiteSpace(partNumber) ? _partNumber : partNumber;
        var recipe = new RecipeBundle(pn, Path.GetFileName(_templatePath), null, Array.Empty<ScrewRecipeDto>());
        return Task.FromResult(new ProvisionedRecipe(recipe, _templatePath, RecipeTemplateSource.Local, null));
    }
}
