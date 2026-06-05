using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Hmi.Services;

public static class AuditHelper
{
    public static void Log(
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> options,
        ICurrentUser user,
        AuditCategory category,
        string action,
        string? target = null,
        string? detail = null,
        bool success = true,
        string? serialNumber = null)
    {
        audit.Log(new UserAuditEntry(
            DateTimeOffset.Now,
            options.Value.StationId,
            user.UserId,
            user.DisplayName,
            user.Role,
            category,
            action,
            target,
            detail,
            success,
            serialNumber));
    }

    public static void LogAuth(
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> options,
        string userId,
        string displayName,
        UserRole role,
        string action,
        string? detail = null,
        bool success = true) =>
        audit.Log(new UserAuditEntry(
            DateTimeOffset.Now,
            options.Value.StationId,
            userId,
            displayName,
            role,
            AuditCategory.Auth,
            action,
            null,
            detail,
            success));
}
