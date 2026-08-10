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

public sealed class CheckpointRestoreTests
{
    [Fact]
    public async Task RestoreFromCheckpoint_RestoresPhaseAndScrewStates()
    {
        await using var env = await RestoreEnv.CreateAsync();
        var controller = env.Controller;
        var store = env.CheckpointStore;

        controller.RequestScanDialog();
        await controller.SubmitSerialNumberAsync("SN-CP-001");
        await controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.Running, controller.Phase);
        Assert.NotNull(store.Get("SN-CP-001"));

        var saved = store.Get("SN-CP-001")!;
        var controller2 = env.CreateFreshController(store);

        var offer = await controller2.GetCheckpointRestoreOfferAsync();
        Assert.NotNull(offer);
        Assert.Equal("SN-CP-001", offer!.SerialNumber);

        var restored = await controller2.RestoreFromCheckpointAsync();
        Assert.True(restored);
        Assert.Equal(saved.Phase, controller2.Phase);
        Assert.Equal(saved.ActiveSurfaceOrdinal, controller2.ActiveSurfaceOrdinal);
        Assert.Equal("SN-CP-001", controller2.SerialNumber);
    }

    [Fact]
    public async Task Reset_ParksMemory_OtherSnDoesNotOverwrite_RescanRestores()
    {
        await using var env = await RestoreEnv.CreateAsync();
        var a = env.Controller;
        var store = env.CheckpointStore;

        a.RequestScanDialog();
        await a.SubmitSerialNumberAsync("SN-A");
        await a.RunCurrentScrewCycleAsync();
        Assert.Equal(JobSessionPhase.Running, a.Phase);
        Assert.NotNull(store.Get("SN-A"));

        await a.ResetToIdleAsync();
        Assert.Equal(JobSessionPhase.Idle, a.Phase);
        Assert.NotNull(store.Get("SN-A"));
        Assert.Equal(SnJobMemoryStatus.InProgress, store.GetStatus("SN-A"));

        a.RequestScanDialog();
        await a.SubmitSerialNumberAsync("SN-B");
        Assert.Equal(JobSessionPhase.Running, a.Phase);
        Assert.NotNull(store.Get("SN-B"));
        Assert.NotNull(store.Get("SN-A"));

        await a.ResetToIdleAsync();
        a.RequestScanDialog();
        var accept = await a.AcceptSerialNumberAsync("SN-A");
        Assert.True(accept.Accepted);
        var offer = await a.TryGetRestorableMemoryAsync("SN-A");
        Assert.NotNull(offer);
        Assert.True(offer!.CompletedScrewCount >= 1);

        await a.ContinueRestoreAfterSerialAcceptedAsync("SN-A");
        Assert.True(a.IsActiveJobPhase || a.Phase == JobSessionPhase.Running);
        Assert.Equal("SN-A", a.SerialNumber);
    }

    [Fact]
    public async Task ActiveJob_RejectsDifferentSn()
    {
        await using var env = await RestoreEnv.CreateAsync();
        var controller = env.Controller;
        controller.RequestScanDialog();
        await controller.SubmitSerialNumberAsync("SN-A");
        Assert.True(controller.IsActiveJobPhase);

        var blocked = await controller.AcceptSerialNumberAsync("SN-B");
        Assert.False(blocked.Accepted);
        Assert.Equal("ActiveJobMustReset", blocked.ErrorMessage);
        Assert.Equal("SN-A", controller.SerialNumber);
    }

    [Fact]
    public async Task Completed_IsNotRestorable()
    {
        await using var env = await RestoreEnv.CreateAsync();
        var store = env.CheckpointStore;
        store.SetCompleted("SN-DONE", new SessionCheckpointData(
            JobSessionPhase.Completed,
            "SN-DONE",
            "PN-CP",
            0,
            0,
            [new SurfaceCheckpointSurface("S1", SurfaceProgressState.Complete, [StationScrewState.Ok])],
            DateTimeOffset.UtcNow));

        var controller = env.CreateFreshController(store);
        var offer = await controller.TryGetRestorableMemoryAsync("SN-DONE");
        Assert.Null(offer);
        Assert.Null(await controller.GetCheckpointRestoreOfferAsync());
    }

    private sealed class RestoreEnv : IAsyncDisposable
    {
        private readonly string _tempDir;

        private RestoreEnv(OperatorSessionController controller, MemoryCheckpointStore checkpointStore, string tempDir, string templateFileName)
        {
            Controller = controller;
            CheckpointStore = checkpointStore;
            _tempDir = tempDir;
            TemplateFileName = templateFileName;
        }

        public OperatorSessionController Controller { get; }

        public MemoryCheckpointStore CheckpointStore { get; }

        public string TemplateFileName { get; }

        public static async Task<RestoreEnv> CreateAsync()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "autoscrew-cp-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test-multisurface.product-template.json");
            var templateFileName = "test-multisurface.product-template.json";
            File.Copy(source, Path.Combine(tempDir, templateFileName), overwrite: true);

            var store = new MemoryCheckpointStore();
            var controller = BuildController(tempDir, templateFileName, store, new SimulatedLockStationHardware(Options.Create(new SimulationOptions())));
            await Task.CompletedTask;
            return new RestoreEnv(controller, store, tempDir, templateFileName);
        }

        public OperatorSessionController CreateFreshController(MemoryCheckpointStore store) =>
            BuildController(_tempDir, TemplateFileName, store, new SimulatedLockStationHardware(Options.Create(new SimulationOptions())));

        private static OperatorSessionController BuildController(
            string tempDir,
            string templateFileName,
            MemoryCheckpointStore store,
            SimulatedLockStationHardware hardware) =>
            new(
                new RestoreMesClient(templateFileName),
                new StubRecipeProvisioningService(Path.Combine(tempDir, templateFileName)),
                new NoOpControllerTraceService(),
                new TemplateLayoutJsonLoader(NullLogger<TemplateLayoutJsonLoader>.Instance),
                hardware,
                new NoOpCurveArchive(),
                store,
                new NoOpOutbox(),
                new SimpleUser(),
                Options.Create(new AutoScrewAppOptions { TemplateDirectory = tempDir, StationId = "T-01" }),
                Options.Create(new SimulationOptions()),
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
        private readonly Dictionary<string, (SessionCheckpointData Data, SnJobMemoryStatus Status)> _bySn = new(StringComparer.OrdinalIgnoreCase);

        public SessionCheckpointData? Get(string sn) =>
            _bySn.TryGetValue(sn, out var row) ? row.Data : null;

        public SnJobMemoryStatus? GetStatus(string sn) =>
            _bySn.TryGetValue(sn, out var row) ? row.Status : null;

        public void SetCompleted(string sn, SessionCheckpointData data) =>
            _bySn[sn] = (data, SnJobMemoryStatus.Completed);

        public Task SaveJobMemoryAsync(
            SessionCheckpointData data,
            SnJobMemoryStatus status,
            CancellationToken cancellationToken = default)
        {
            _bySn[data.SerialNumber] = (data, status);
            return Task.CompletedTask;
        }

        public Task<SessionCheckpointData?> LoadJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(Get(serialNumber));

        public Task<SnJobMemoryStatus?> GetJobMemoryStatusAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(GetStatus(serialNumber));

        public Task<SessionCheckpointData?> LoadLatestRestorableAsync(CancellationToken cancellationToken = default)
        {
            SessionCheckpointData? best = null;
            DateTimeOffset bestAt = default;
            foreach (var (_, row) in _bySn)
            {
                if (row.Status == SnJobMemoryStatus.Completed)
                    continue;
                if (row.Data.Phase is not (JobSessionPhase.Running or JobSessionPhase.AwaitFlip or JobSessionPhase.NgLocked))
                    continue;
                if (best is null || row.Data.UpdatedAt >= bestAt)
                {
                    best = row.Data;
                    bestAt = row.Data.UpdatedAt;
                }
            }

            return Task.FromResult(best);
        }

        public Task MarkJobCompletedAsync(string serialNumber, CancellationToken cancellationToken = default)
        {
            if (_bySn.TryGetValue(serialNumber, out var row))
                _bySn[serialNumber] = (row.Data, SnJobMemoryStatus.Completed);
            return Task.CompletedTask;
        }

        public Task RemoveJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default)
        {
            _bySn.Remove(serialNumber);
            return Task.CompletedTask;
        }

        public Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default)
        {
            var status = data.Phase == JobSessionPhase.NgLocked
                ? SnJobMemoryStatus.NgPaused
                : SnJobMemoryStatus.InProgress;
            return SaveJobMemoryAsync(data, status, cancellationToken);
        }

        public Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default) =>
            LoadLatestRestorableAsync(cancellationToken);

        public Task ClearCheckpointAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<long> SaveLockRecordAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
            Task.FromResult(1L);
    }

    private sealed class RestoreMesClient(string templateFileName) : IMesClient
    {
        public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SnValidationResult(true, "PN-CP", null));

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
        public string UserId => "test";
        public string DisplayName => "Test";
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
