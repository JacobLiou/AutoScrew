using AutoScrew.Application.Abstractions;
using AutoScrew.Common.Host;

namespace AutoScrew.Infrastructure.Host;

public sealed class CachedHostIdentity : IHostIdentity
{
    private readonly HostIdentitySnapshot _snapshot = HostIdentity.Current;

    public string? IpAddress => _snapshot.IpAddress;

    public string? MacAddress => _snapshot.MacAddress;

    public string MacFolderName => _snapshot.MacFolderName;
}
