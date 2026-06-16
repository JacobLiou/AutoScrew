using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Application.Templates;
using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Infrastructure.Hardware;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class OperatorSessionMultiSurfaceTests
{
    [Fact]
    public async Task LoadMultiSurfaceTemplate_StartsOnFirstSurfaceOrdinalZero()
    {
        await using var fixture = await MultiSurfaceFixture.CreateAsync();
        var controller = fixture.Controller;

        controller.RequestScanDialog();
        await controller.SubmitSerialNumberAsync("SN-TEST-001");

        Assert.Equal(JobSessionPhase.Running, controller.Phase);
        Assert.Equal(3, controller.TemplateSurfaceCount);
        Assert.Equal(0, controller.ActiveSurfaceOrdinal);
        Assert.Equal("S1", controller.ActiveSurfaceId);
        Assert.Equal("顶面", controller.ActiveSurfaceName);
        Assert.Equal(2, controller.Positions.Count);
        Assert.Equal(0, controller.CurrentScrewIndex);
    }

    [Fact]
    public async Task CompleteFirstSurface_EntersAwaitFlip_ThenAdvancesToSecondSurface()
    {
        await using var fixture = await MultiSurfaceFixture.CreateAsync();
        var controller = fixture.Controller;

        controller.RequestScanDialog();
        await controller.SubmitSerialNumberAsync("SN-TEST-002");

        await controller.RunCurrentScrewCycleAsync();
        await controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.AwaitFlip, controller.Phase);
        var (_, nextName) = controller.GetPendingFlipTarget();
        Assert.Equal("底面", nextName);

        controller.ConfirmAdvanceToNextSurface();

        Assert.Equal(JobSessionPhase.Running, controller.Phase);
        Assert.Equal(1, controller.ActiveSurfaceOrdinal);
        Assert.Equal("S2", controller.ActiveSurfaceId);
        Assert.Single(controller.Positions);
        Assert.Equal(0, controller.CurrentScrewIndex);
    }

    [Fact]
    public async Task CompleteAllSurfaces_CompletesSession()
    {
        await using var fixture = await MultiSurfaceFixture.CreateAsync();
        var controller = fixture.Controller;

        controller.RequestScanDialog();
        await controller.SubmitSerialNumberAsync("SN-TEST-003");

        await controller.RunCurrentScrewCycleAsync();
        await controller.RunCurrentScrewCycleAsync();
        controller.ConfirmAdvanceToNextSurface();
        await controller.RunCurrentScrewCycleAsync();
        controller.ConfirmAdvanceToNextSurface();
        await controller.RunCurrentScrewCycleAsync();
        await controller.RunCurrentScrewCycleAsync();
        await controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.Completed, controller.Phase);
        Assert.Equal(6, fixture.LastUpload?.Screws.Count);
    }

    private sealed class MultiSurfaceFixture : IAsyncDisposable
    {
        public OperatorSessionController Controller { get; }
        public LockJobResultPayload? LastUpload { get; private set; }

        private readonly string _tempDir;

        private MultiSurfaceFixture(OperatorSessionController controller, string tempDir)
        {
            Controller = controller;
            _tempDir = tempDir;
        }

        public static async Task<MultiSurfaceFixture> CreateAsync()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "autoscrew-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test-multisurface.product-template.json");
            var templatePath = Path.Combine(tempDir, "test-multisurface.product-template.json");
            File.Copy(source, templatePath, overwrite: true);

            var mes = new TestMesClient(templatePath);
            var loader = new TemplateLayoutJsonLoader(NullLogger<TemplateLayoutJsonLoader>.Instance);
            var hardware = new SimulatedLockStationHardware(Options.Create(new SimulationOptions()));
            var archive = new TestCurveArchive();
            var checkpoint = new TestCheckpointStore();
            var outbox = new TestOutbox();
            var user = new TestCurrentUser();
            var options = Options.Create(new AutoScrewAppOptions { TemplateDirectory = tempDir, StationId = "T-01" });
            var controller = new OperatorSessionController(
                mes,
                new StubRecipeProvisioningService(templatePath),
                new NoOpControllerTraceService(),
                loader,
                hardware,
                archive,
                checkpoint,
                outbox,
                user,
                options,
                new NoOpUserAuditService(),
                NullLogger<OperatorSessionController>.Instance);

            var fixture = new MultiSurfaceFixture(controller, tempDir);
            mes.OnUpload = p => fixture.LastUpload = p;
            await Task.CompletedTask;
            return fixture;
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class TestMesClient(string templatePath) : IMesClient
    {
        public Action<LockJobResultPayload>? OnUpload { get; set; }

        public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SnValidationResult(true, "DEMO-PN-MULTI", null));

        public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new RecipeBundle(partNumber, Path.GetFileName(templatePath), null, Array.Empty<ScrewRecipeDto>()));

        public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default)
        {
            OnUpload?.Invoke(payload);
            return Task.FromResult(new MesUploadResult(true, null, payload.SerialNumber));
        }
    }

    private sealed class TestCurveArchive : ICurveArchive
    {
        public Task<string> SaveCurveCsvAsync(
            string serialNumber,
            int positionIndex,
            IReadOnlyList<TorqueAngleSample> samples,
            CancellationToken cancellationToken = default) =>
            Task.FromResult($"curve_{positionIndex}.csv");

        public Task SaveLockLogJsonAsync(string serialNumber, string json, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class TestCheckpointStore : ILockSessionRepository
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

    private sealed class TestOutbox : IOutboundMesQueue
    {
        public Task EnqueueAsync(LockJobResultPayload payload, string? failureReason, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class TestCurrentUser : ICurrentUser
    {
        public string UserId => "test-op";
        public string DisplayName => "Test";
        public UserRole Role => UserRole.Operator;
        public bool CanAdjustParameters => false;
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
        public Task WriteSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
