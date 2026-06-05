using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Hmi.Services;

/// <summary>供对话框等无法 DI 的场景访问审计服务。</summary>
public static class AuditContext
{
    private static IUserAuditService? _audit;
    private static IOptions<AutoScrewAppOptions>? _options;
    private static ICurrentUser? _user;

    public static void Initialize(
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> options,
        ICurrentUser user)
    {
        _audit = audit;
        _options = options;
        _user = user;
    }

    public static IUserAuditService Audit =>
        _audit ?? throw new InvalidOperationException("AuditContext not initialized.");

    public static IOptions<AutoScrewAppOptions> Options =>
        _options ?? throw new InvalidOperationException("AuditContext not initialized.");

    public static ICurrentUser User =>
        _user ?? throw new InvalidOperationException("AuditContext not initialized.");

    public static bool IsInitialized => _audit is not null && _options is not null && _user is not null;
}
