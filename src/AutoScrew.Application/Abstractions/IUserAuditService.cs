namespace AutoScrew.Application.Abstractions;

public enum AuditCategory
{
    Auth,
    Navigation,
    UiAction,
    Dialog,
    Setting,
    Operation,
    Configuration,
    System,
}

public sealed record UserAuditEntry(
    DateTimeOffset Timestamp,
    string StationId,
    string UserId,
    string DisplayName,
    UserRole Role,
    AuditCategory Category,
    string Action,
    string? Target = null,
    string? Detail = null,
    bool Success = true,
    string? SerialNumber = null);

/// <summary>仅追加的用户操作审计；同步入队，后台持久化。</summary>
public interface IUserAuditService
{
    void Log(UserAuditEntry entry);
}
