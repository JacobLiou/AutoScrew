using AutoScrew.Application.Templates;
using AutoScrew.Domain.Models;

namespace AutoScrew.Application.Abstractions;

public interface ITemplateLayoutLoader
{
    Task<TemplateLoadResult> LoadAsync(string jsonFilePath, CancellationToken cancellationToken = default);
}

public sealed record TemplateLoadResult(
    TemplateLayoutDto Raw,
    IReadOnlyList<ScrewPosition> Positions,
    string? ResolvedProductImagePath);
