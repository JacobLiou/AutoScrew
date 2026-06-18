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

public sealed class SimulatedFeedFailureTests
{
    [Fact]
    public async Task FeedFailure_OnFirstPick_EntersNgLocked()
    {
        await using var fixture = await FeedFixture.CreateAsync(new SimulationOptions
        {
            FeedFailureMode = SimulatedFeedFailureMode.Empty,
            FeedFailureOnScrewIndex = 1,
        });

        fixture.Controller.RequestScanDialog();
        await fixture.Controller.SubmitSerialNumberAsync("SN-FEED-01");

        await fixture.Controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.NgLocked, fixture.Controller.Phase);
        Assert.Equal("FEED_EMPTY", fixture.Controller.LastErrorCode);
    }

    private sealed class FeedFixture(OperatorSessionController controller, string tempDir) : IAsyncDisposable
    {
        public OperatorSessionController Controller { get; } = controller;
        private readonly string _tempDir = tempDir;

        public static async Task<FeedFixture> CreateAsync(SimulationOptions simulation)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "autoscrew-feed-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test-multisurface.product-template.json");
            var templatePath = Path.Combine(tempDir, "demo.json");
            File.Copy(source, templatePath, overwrite: true);

            var mes = new SimpleMesClient(templatePath);
            var controller = new OperatorSessionController(
                mes,
                new StubRecipeProvisioningService(templatePath),
                new NoOpControllerTraceService(),
                new TemplateLayoutJsonLoader(NullLogger<TemplateLayoutJsonLoader>.Instance),
                new SimulatedLockStationHardware(Options.Create(simulation)),
                new NoOpCurveArchive(),
                new NoOpCheckpointStore(),
                new NoOpOutbox(),
                new SimpleUser(),
                Options.Create(new AutoScrewAppOptions { TemplateDirectory = tempDir, StationId = "T-01" }),
                Options.Create(new SimulationOptions()),
                new NoOpUserAuditService(),
                NullLogger<OperatorSessionController>.Instance);

            await Task.CompletedTask;
            return new FeedFixture(controller, tempDir);
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class SimpleMesClient(string templatePath) : IMesClient
    {
        public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SnValidationResult(true, "PN-DEMO", null));

        public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new RecipeBundle(partNumber, Path.GetFileName(templatePath), null, Array.Empty<ScrewRecipeDto>()));

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

    private sealed class NoOpCheckpointStore : ILockSessionRepository
    {
        public Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default) => Task.FromResult<SessionCheckpointData?>(null);
        public Task ClearCheckpointAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<long> SaveLockRecordAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) => Task.FromResult(1L);
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
