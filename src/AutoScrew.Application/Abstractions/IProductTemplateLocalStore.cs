namespace AutoScrew.Application.Abstractions;

public interface IProductTemplateLocalStore
{
    string GetTemplateDirectory();

    string GetProductFolder(string partNumber);

    string GetDefaultTemplatePath(string partNumber);

    void EnsureProductFolder(string partNumber);

    IReadOnlyList<string> ListLocalPartNumbers();

    string? TryResolveLocalTemplate(string partNumber);

    string? TryResolveTemplatePath(string? templateJsonPath);

    string ToRelativePath(string absolutePath);

    void SeedFromSamplesIfEmpty();
}
