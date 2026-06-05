using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Audit;
using AutoScrew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public class UserAuditServiceTests
{
    [Fact]
    public void ResolveAuditDirectory_UsesDataDirectoryWhenAuditDirectoryEmpty()
    {
        var opts = new AutoScrewAppOptions
        {
            DataDirectory = @"C:\data\autoscrew",
            AuditDirectory = "",
        };

        var path = UserAuditBackgroundService.ResolveAuditDirectory(opts);
        Assert.Equal(Path.Combine(@"C:\data\autoscrew", "audit"), path);
    }

    [Fact]
    public void JsonlUserAuditStore_AppendsSingleLine()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-audit-test", Guid.NewGuid().ToString("N"));
        var store = new JsonlUserAuditStore(NullLogger<JsonlUserAuditStore>.Instance);
        var entry = new UserAuditEntry(
            DateTimeOffset.Parse("2026-05-21T10:00:00+08:00"),
            "STATION-01",
            "op1",
            "Operator",
            UserRole.Operator,
            AuditCategory.Operation,
            "Operation.SubmitSn",
            "Submit SN",
            "sn=ABC123",
            true,
            "ABC123");

        store.Append(dir, entry);

        var files = Directory.GetFiles(dir, "user-audit-*.jsonl");
        Assert.Single(files);
        var text = File.ReadAllText(files[0]);
        Assert.Contains("Operation.SubmitSn", text);
        Assert.Contains("ABC123", text);

        Directory.Delete(dir, recursive: true);
    }

    [Fact]
    public void UserAuditService_Log_EnqueuesWhenEnabled()
    {
        var options = Options.Create(new AutoScrewAppOptions { AuditLogEnabled = true });
        using var service = new UserAuditService(options);
        service.Log(new UserAuditEntry(
            DateTimeOffset.UtcNow,
            "STATION-01",
            "tech1",
            "Tech",
            UserRole.Technician,
            AuditCategory.Configuration,
            "Configuration.DeviceSave"));

        Assert.True(service.AuditChannel.Reader.TryRead(out var read));
        Assert.Equal("Configuration.DeviceSave", read.Action);
    }

    [Fact]
    public void UserAuditService_Log_SkipsWhenDisabled()
    {
        var options = Options.Create(new AutoScrewAppOptions { AuditLogEnabled = false });
        using var service = new UserAuditService(options);
        service.Log(new UserAuditEntry(
            DateTimeOffset.UtcNow,
            "STATION-01",
            "tech1",
            "Tech",
            UserRole.Technician,
            AuditCategory.Configuration,
            "Configuration.DeviceSave"));

        Assert.False(service.AuditChannel.Reader.TryRead(out _));
    }
}
