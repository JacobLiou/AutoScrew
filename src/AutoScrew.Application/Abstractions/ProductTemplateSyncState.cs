namespace AutoScrew.Application.Abstractions;

public enum ProductTemplateSyncState
{
    LocalOnly = 0,
    DownloadedFromMes = 1,
    PendingUpload = 2,
    Synced = 3,
    Failed = 4,
}

public enum RecipeTemplateSource
{
    Mes = 0,
    Local = 1,
}

public sealed record ProductTemplateSyncRecord(
    string PartNumber,
    string LocalRelativePath,
    ProductTemplateSyncState SyncState,
    string? LocalFileHash,
    DateTimeOffset? LocalModifiedUtc,
    DateTimeOffset? LastMesPullUtc,
    DateTimeOffset? LastMesPushUtc,
    string? MesRevision,
    string? LastError);

public sealed record ProvisionedRecipe(
    RecipeBundle Recipe,
    string ResolvedTemplatePath,
    RecipeTemplateSource TemplateSource,
    string? InfoMessage);
