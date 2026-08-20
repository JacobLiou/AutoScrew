using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Application.Templates;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Infrastructure.Hardware;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class MissingScrewValidationTests
{
    [Fact]
    public async Task OverTorqueProfile_CausesNgLocked()
    {
        await using var env = await MissingScrewEnv.CreateAsync(new SimulationOptions
        {
            TighteningProfile = SimulatedTighteningProfile.OverTorque,
        });

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-MISS-01");
        await env.Controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.NgLocked, env.Controller.Phase);
        Assert.Equal("OVER_TORQUE_001", env.Controller.LastErrorCode);
    }

    [Fact]
    public async Task ParkFromNg_SavesRunningPending_RescanRestoresWithoutNgLock()
    {
        await using var env = await MissingScrewEnv.CreateAsync(new SimulationOptions
        {
            TighteningProfile = SimulatedTighteningProfile.OverTorque,
        });

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-PARK-01");
        await env.Controller.RunCurrentScrewCycleAsync();
        Assert.Equal(JobSessionPhase.NgLocked, env.Controller.Phase);
        Assert.Equal(StationScrewState.Ng, env.Controller.ScrewStates[0]);

        await env.Controller.ParkJobAsync();

        Assert.Equal(JobSessionPhase.Idle, env.Controller.Phase);
        Assert.NotNull(env.Store.Last);
        Assert.Equal(JobSessionPhase.Running, env.Store.Last!.Phase);
        Assert.Equal(StationScrewState.Pending, env.Store.Last.Surfaces[0].ScrewStates[0]);

        env.Controller.RequestScanDialog();
        var accept = await env.Controller.AcceptSerialNumberAsync("SN-PARK-01");
        Assert.True(accept.Accepted);
        await env.Controller.ContinueRestoreAfterSerialAcceptedAsync("SN-PARK-01");

        Assert.Equal(JobSessionPhase.Running, env.Controller.Phase);
        Assert.Equal(StationScrewState.Pending, env.Controller.ScrewStates[0]);
        Assert.Null(env.Controller.LastErrorCode);
    }

    [Fact]
    public async Task ConfirmFlip_WithIncompleteSurface_Throws()
    {
        await using var env = await MissingScrewEnv.CreateAsync(new SimulationOptions());
        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-FLIP");

        env.Store.Last = env.Store.Last! with
        {
            Phase = JobSessionPhase.AwaitFlip,
            ActiveSurfaceOrdinal = 0,
            Surfaces =
            [
                new SurfaceCheckpointSurface("S1", SurfaceProgressState.Complete, [StationScrewState.Ok, StationScrewState.Pending]),
                new SurfaceCheckpointSurface("S2", SurfaceProgressState.Locked, [StationScrewState.Pending]),
                new SurfaceCheckpointSurface("S3", SurfaceProgressState.Locked, [StationScrewState.Pending, StationScrewState.Pending, StationScrewState.Pending]),
            ],
        };

        Assert.True(await env.Controller.RestoreFromCheckpointAsync());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Task.Run(() => env.Controller.ConfirmAdvanceToNextSurface()));

        Assert.Contains("not completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class MissingScrewEnv(OperatorSessionController controller, MemoryCheckpointStore store, string tempDir) : IAsyncDisposable
    {
        public OperatorSessionController Controller { get; } = controller;

        public MemoryCheckpointStore Store { get; } = store;

        private readonly string _tempDir = tempDir;

        public static async Task<MissingScrewEnv> CreateAsync(SimulationOptions simulation)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "autoscrew-miss-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test-multisurface.product-template.json");
            var templateFileName = "test-multisurface.product-template.json";
            File.Copy(source, Path.Combine(tempDir, templateFileName), overwrite: true);
            var store = new MemoryCheckpointStore();
            var controller = Build(tempDir, templateFileName, store, simulation);
            await Task.CompletedTask;
            return new MissingScrewEnv(controller, store, tempDir);
        }

        private static OperatorSessionController Build(
            string tempDir,
            string templateFileName,
            MemoryCheckpointStore store,
            SimulationOptions simulation) =>
            new(
                new SimpleMesClient(templateFileName),
                new StubRecipeProvisioningService(Path.Combine(tempDir, templateFileName)),
                new NoOpControllerTraceService(),
                new TemplateLayoutJsonLoader(NullLogger<TemplateLayoutJsonLoader>.Instance),
                new SimulatedLockStationHardware(Options.Create(simulation)),
                new NoOpCurveArchive(),
                store,
                new NoOpOutbox(),
                new SimpleUser(),
                Options.Create(new AutoScrewAppOptions { TemplateDirectory = tempDir, StationId = "T-01" }),
                Options.Create(simulation),
                new NoOpUserAuditService(),
                new StubHostIdentity(),
                NullLogger<OperatorSessionController>.Instance);

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class MemoryCheckpointStore : ILockSessionRepository
    {
        public SessionCheckpointData? Last { get; set; }

        public Task SaveJobMemoryAsync(SessionCheckpointData data, SnJobMemoryStatus status, CancellationToken cancellationToken = default)
        {
            Last = data;
            return Task.CompletedTask;
        }

        public Task<SessionCheckpointData?> LoadJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(Last);

        public Task<SnJobMemoryStatus?> GetJobMemoryStatusAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(Last is null ? (SnJobMemoryStatus?)null : SnJobMemoryStatus.InProgress);

        public Task<SessionCheckpointData?> LoadLatestRestorableAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Last);

        public Task MarkJobCompletedAsync(string serialNumber, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RemoveJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default)
        {
            Last = null;
            return Task.CompletedTask;
        }

        public Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default) =>
            SaveJobMemoryAsync(data, SnJobMemoryStatus.InProgress, cancellationToken);

        public Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default) =>
            LoadLatestRestorableAsync(cancellationToken);

        public Task ClearCheckpointAsync(CancellationToken cancellationToken = default)
        {
            Last = null;
            return Task.CompletedTask;
        }

        public Task<long> SaveLockRecordAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
            Task.FromResult(1L);
    }

    private sealed class SimpleMesClient(string templateFileName) : IMesClient
    {
        public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SnValidationResult(true, "PN-1", null));

        public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new RecipeBundle(partNumber, templateFileName, null, Array.Empty<ScrewRecipeDto>()));

        public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
            Task.FromResult(new MesUploadResult(true, null, payload.SerialNumber));
    }

    private sealed class NoOpCurveArchive : ICurveArchive
    {
        public Task<string> SaveCurveCsvAsync(string serialNumber, int positionIndex, IReadOnlyList<Domain.Curves.TorqueAngleSample> samples, CancellationToken cancellationToken = default) =>
            Task.FromResult("curve.csv");

        public Task SaveLockLogJsonAsync(string serialNumber, string json, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class NoOpOutbox : IOutboundMesQueue
    {
        public Task EnqueueAsync(LockJobResultPayload payload, string? failureReason, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class SimpleUser : ICurrentUser
    {
        public string UserId => "tech";
        public string DisplayName => "Tech";
        public UserRole Role => UserRole.Technician;
        public bool CanAdjustParameters => true;
        public bool CanUnlockNg => true;
        public int? MimsPersonId => null;
        public int? MimsRoleId => null;
        public int? MimsRoleType => null;
    }

    private sealed class StubHostIdentity : IHostIdentity
    {
        public string? IpAddress => "192.168.1.10";
        public string? MacAddress => "AA-BB-CC-DD-EE-FF";
        public string MacFolderName => "AA-BB-CC-DD-EE-FF";
    }

    private sealed class NoOpUserAuditService : IUserAuditService
    {
        public void Log(UserAuditEntry entry)
        {
        }
    }

    private sealed class NoOpControllerTraceService : IControllerTraceService
    {
        public Task WriteSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
