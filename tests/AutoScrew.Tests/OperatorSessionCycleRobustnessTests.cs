using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Application.Templates;
using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class OperatorSessionCycleRobustnessTests
{
    [Fact]
    public async Task TightenException_RestoresPendingAndClearsGate()
    {
        await using var env = await RobustnessEnv.CreateAsync(throwOnTighten: true);

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-ROB-EX-01");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => env.Controller.RunCurrentScrewCycleAsync());

        Assert.Contains("simulated tighten fault", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(env.Controller.IsCycleInProgress);
        Assert.Equal(StationScrewState.Pending, env.Controller.ScrewStates[0]);
        Assert.Equal(JobSessionPhase.Running, env.Controller.Phase);
    }

    [Fact]
    public async Task StopActiveCycle_WhileHung_ForceClearsGateAndRestoresPending()
    {
        await using var env = await RobustnessEnv.CreateAsync(hangOnTighten: true);

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-ROB-CANCEL-01");

        var cycleTask = env.Controller.RunCurrentScrewCycleAsync();
        await env.Hardware.EnteredTighten.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.True(env.Controller.IsCycleInProgress);
        Assert.Equal(StationScrewState.InProgress, env.Controller.ScrewStates[0]);

        await env.Controller.StopActiveScrewCycleAsync();

        Assert.False(env.Controller.IsCycleInProgress);
        Assert.Equal(StationScrewState.Pending, env.Controller.ScrewStates[0]);

        // Cancel path swallows OCE — cycle task should complete without fault.
        await cycleTask.WaitAsync(TimeSpan.FromSeconds(5));
    }

    private sealed class RobustnessEnv : IAsyncDisposable
    {
        public OperatorSessionController Controller { get; }
        public ControllableHardware Hardware { get; }

        private readonly string _tempDir;

        private RobustnessEnv(OperatorSessionController controller, ControllableHardware hardware, string tempDir)
        {
            Controller = controller;
            Hardware = hardware;
            _tempDir = tempDir;
        }

        public static async Task<RobustnessEnv> CreateAsync(bool throwOnTighten = false, bool hangOnTighten = false)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "autoscrew-rob-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test-multisurface.product-template.json");
            var templateFileName = "test-multisurface.product-template.json";
            File.Copy(source, Path.Combine(tempDir, templateFileName), overwrite: true);

            var hardware = new ControllableHardware(throwOnTighten, hangOnTighten);
            var simulation = Options.Create(new SimulationOptions());
            var controller = new OperatorSessionController(
                new SimpleMesClient(templateFileName),
                new StubRecipeProvisioningService(Path.Combine(tempDir, templateFileName)),
                new NoOpControllerTraceService(),
                new TemplateLayoutJsonLoader(NullLogger<TemplateLayoutJsonLoader>.Instance),
                hardware,
                new NoOpCurveArchive(),
                new MemoryCheckpointStore(),
                new NoOpOutbox(),
                new SimpleUser(),
                Options.Create(new AutoScrewAppOptions { TemplateDirectory = tempDir, StationId = "T-01" }),
                simulation,
                new NoOpUserAuditService(),
                new StubHostIdentity(),
                NullLogger<OperatorSessionController>.Instance);

            await Task.CompletedTask;
            return new RobustnessEnv(controller, hardware, tempDir);
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class ControllableHardware : ILockStationHardware
    {
        private readonly bool _throwOnTighten;
        private readonly bool _hangOnTighten;

        public ControllableHardware(bool throwOnTighten, bool hangOnTighten)
        {
            _throwOnTighten = throwOnTighten;
            _hangOnTighten = hangOnTighten;
        }

        public TaskCompletionSource EnteredTighten { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public LockHardwareOutcome? LastOutcome { get; private set; }

        public TighteningCurveDiagnostic? LastCurveDiagnostic { get; private set; }

        public Task PickScrewAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task PrepareForJobAsync(CancellationToken cancellationToken = default, int? sequenceId = null) =>
            Task.CompletedTask;

        public Task ClearErrorsAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public async IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
            TighteningContext context,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnteredTighten.TrySetResult();

            if (_throwOnTighten)
                throw new InvalidOperationException("simulated tighten fault");

            if (_hangOnTighten)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
                yield break;
            }

            LastOutcome = new LockHardwareOutcome(true, 0.25, 360, null, 1);
            LastCurveDiagnostic = new TighteningCurveDiagnostic(1, 1, null);
            yield return new TorqueAngleSample(0, 0.1, 10, 0, null);
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
        public Task<string> SaveCurveCsvAsync(string serialNumber, int positionIndex, IReadOnlyList<TorqueAngleSample> samples, CancellationToken cancellationToken = default) =>
            Task.FromResult("curve.csv");

        public Task SaveLockLogJsonAsync(string serialNumber, string json, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class NoOpOutbox : IOutboundMesQueue
    {
        public Task EnqueueAsync(LockJobResultPayload payload, string? failureReason, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
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
        public Task WriteSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
