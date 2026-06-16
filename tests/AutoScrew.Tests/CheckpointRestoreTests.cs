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
        Assert.NotNull(store.Last);

        var saved = store.Last!;
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
                new NoOpUserAuditService(),
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
        public SessionCheckpointData? Last { get; private set; }

        public Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default)
        {
            Last = data;
            return Task.CompletedTask;
        }

        public Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Last);

        public Task ClearCheckpointAsync(CancellationToken cancellationToken = default)
        {
            Last = null;
            return Task.CompletedTask;
        }

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
