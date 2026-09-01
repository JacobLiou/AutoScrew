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

public sealed class OperatorSessionDeviceAuthorityTests
{
    /// <summary>设备 OK + 曲线峰值会触发 OVER_TORQUE advisory → 作业继续，不 NgLocked。</summary>
    [Fact]
    public async Task DeviceOk_OverTorqueCurveAdvisory_DoesNotNgLock()
    {
        await using var env = await DeviceAuthorityEnv.CreateAsync(new SimulationOptions
        {
            TighteningProfile = SimulatedTighteningProfile.OverTorque,
        });

        // 覆盖仿真为设备 OK，但曲线仍为高扭矩（OverTorque profile 峰值 0.50 N·m）
        env.Hardware.OverrideLastOutcome(new LockHardwareOutcome(true, 0.50, 520, null, 1));

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-DEV-OK-01");
        await env.Controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.Running, env.Controller.Phase);
        Assert.Equal(StationScrewState.Ok, env.Controller.ScrewStates[0]);
        Assert.Null(env.Controller.LastErrorCode);
    }

    [Fact]
    public async Task DeviceNg_OverTorqueProfile_NgLockedWithDeviceCode()
    {
        await using var env = await DeviceAuthorityEnv.CreateAsync(new SimulationOptions
        {
            TighteningProfile = SimulatedTighteningProfile.OverTorque,
        });

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-DEV-NG-01");
        await env.Controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.NgLocked, env.Controller.Phase);
        Assert.Equal("DEVICE_1", env.Controller.LastErrorCode);
    }

    [Fact]
    public async Task DeviceNg_FloatLockProfile_NgLockedWithDeviceCode()
    {
        await using var env = await DeviceAuthorityEnv.CreateAsync(new SimulationOptions
        {
            TighteningProfile = SimulatedTighteningProfile.FloatLock,
        });

        env.Controller.RequestScanDialog();
        await env.Controller.SubmitSerialNumberAsync("SN-DEV-FLOAT-01");
        await env.Controller.RunCurrentScrewCycleAsync();

        Assert.Equal(JobSessionPhase.NgLocked, env.Controller.Phase);
        Assert.Equal("DEVICE_1", env.Controller.LastErrorCode);
    }

    private sealed class DeviceAuthorityEnv : IAsyncDisposable
    {
        public OperatorSessionController Controller { get; }
        public OverridableSimHardware Hardware { get; }

        private readonly string _tempDir;

        private DeviceAuthorityEnv(OperatorSessionController controller, OverridableSimHardware hardware, string tempDir)
        {
            Controller = controller;
            Hardware = hardware;
            _tempDir = tempDir;
        }

        public static async Task<DeviceAuthorityEnv> CreateAsync(SimulationOptions simulation)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "autoscrew-dev-auth-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test-multisurface.product-template.json");
            var templateFileName = "test-multisurface.product-template.json";
            File.Copy(source, Path.Combine(tempDir, templateFileName), overwrite: true);

            var hardware = new OverridableSimHardware(Options.Create(simulation));
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
                Options.Create(simulation),
                new NoOpUserAuditService(),
                new StubHostIdentity(),
                NullLogger<OperatorSessionController>.Instance);

            await Task.CompletedTask;
            return new DeviceAuthorityEnv(controller, hardware, tempDir);
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>仿真硬件，RunTightening 后可注入 LastOutcome 覆盖（测 device OK + 高扭矩曲线）。</summary>
    private sealed class OverridableSimHardware : ILockStationHardware
    {
        private readonly SimulatedLockStationHardware _inner;
        private LockHardwareOutcome? _override;

        public OverridableSimHardware(IOptions<SimulationOptions> simulation) =>
            _inner = new SimulatedLockStationHardware(simulation);

        public LockHardwareOutcome? LastOutcome => _override ?? _inner.LastOutcome;

        public void OverrideLastOutcome(LockHardwareOutcome outcome) => _override = outcome;

        public Task PickScrewAsync(CancellationToken cancellationToken = default) =>
            _inner.PickScrewAsync(cancellationToken);

        public Task PrepareForJobAsync(CancellationToken cancellationToken = default, int? sequenceId = null) =>
            _inner.PrepareForJobAsync(cancellationToken, sequenceId);

        public Task ClearErrorsAsync(CancellationToken cancellationToken = default) =>
            _inner.ClearErrorsAsync(cancellationToken);

        public IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
            TighteningContext context,
            CancellationToken cancellationToken = default) =>
            _inner.RunTighteningAsync(context, cancellationToken);
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
