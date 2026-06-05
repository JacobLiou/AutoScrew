using System.Threading.Channels;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Audit;

public sealed class UserAuditService : IUserAuditService, IDisposable
{
    private readonly IOptions<AutoScrewAppOptions> _options;

    public UserAuditService(IOptions<AutoScrewAppOptions> options)
    {
        _options = options;
        AuditChannel = System.Threading.Channels.Channel.CreateUnbounded<UserAuditEntry>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
            });
    }

    internal Channel<UserAuditEntry> AuditChannel { get; }

    public void Log(UserAuditEntry entry)
    {
        if (!_options.Value.AuditLogEnabled)
            return;

        if (!AuditChannel.Writer.TryWrite(entry))
            _ = AuditChannel.Writer.WriteAsync(entry);
    }

    public void Dispose() => AuditChannel.Writer.TryComplete();
}
