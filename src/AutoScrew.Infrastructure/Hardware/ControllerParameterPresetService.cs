using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Exceptions;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class ControllerParameterPresetService : IControllerParameterPresetService
{
    private readonly LocalJsonControllerParameterPresetStore _store;
    private readonly IStationDeviceService _devices;
    private readonly ILogger<ControllerParameterPresetService> _logger;

    public ControllerParameterPresetService(
        LocalJsonControllerParameterPresetStore store,
        IStationDeviceService devices,
        ILogger<ControllerParameterPresetService> logger)
    {
        _store = store;
        _devices = devices;
        _logger = logger;
    }

    public bool IsDeviceAvailable => _devices.IsRuntimeDeviceAvailable;

    public async Task<IReadOnlyList<ControllerParameterPresetSummary>> ListLocalPresetsAsync(
        CancellationToken cancellationToken = default)
    {
        var docs = await _store.ListAsync(cancellationToken).ConfigureAwait(false);
        return docs
            .Select(d =>
            {
                var template = d.ToTemplate();
                return new ControllerParameterPresetSummary(
                    template.ParameterId,
                    template.Core.Name,
                    template.ToolIndex);
            })
            .ToList();
    }

    public Task<TighteningParameterTemplate> LoadLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default) =>
        _store.LoadAsync(parameterId, cancellationToken);

    public Task SaveLocalPresetAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default) =>
        _store.SaveAsync(template, cancellationToken);

    public Task DeleteLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default) =>
        _store.DeleteAsync(parameterId, cancellationToken);

    public Task<TighteningParameterTemplate> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default) =>
        _store.ImportFromFileAsync(filePath, cancellationToken);

    public Task ExportToFileAsync(TighteningParameterTemplate template, string filePath, CancellationToken cancellationToken = default) =>
        _store.ExportToFileAsync(template, filePath, cancellationToken);

    public async Task<TighteningParameterTemplate> ReadFromDeviceAsync(
        int parameterId,
        CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var template = await client.ReadParameterAsync(parameterId, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Read parameter {ParamId} from IEMD-SD", parameterId);
        return template;
    }

    public async Task<DefaultTorqueUnit> ReadDefaultTorqueUnitAsync(CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var unit = await client.ReadDefaultTorqueUnitAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Read default torque unit from IEMD-SD: {Unit}", unit);
        return unit;
    }

    public async Task<IReadOnlyList<int>> ListDeviceParameterIdsAsync(CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var configuredTool = client.Options.ToolIndex;

        var snapshot = await client
            .ListParametersAsync(ParameterListSnapshot.MaxParameterSlots, cancellationToken)
            .ConfigureAwait(false);
        var ids = snapshot.GetConfiguredIds();
        if (ids.Count > 0)
            return ids;

        // Probe the other tool axis. Do NOT treat mailbox word2=0 as "no tool" while staying on tool 1 for #150/#100 —
        // that previously returned phantom IDs that then failed #150 with deviceError=3.
        var otherTool = configuredTool == 0 ? 1 : 0;
        var other = await client
            .ListParametersForToolAsync(otherTool, ParameterListSnapshot.MaxParameterSlots, cancellationToken)
            .ConfigureAwait(false);
        var otherIds = other.GetConfiguredIds();
        if (otherIds.Count > 0)
        {
            var sample = string.Join(", ", otherIds.Take(12));
            throw new InvalidOperationException(
                $"当前设备连接「工具号」为 {configuredTool}，该工具下 #160 无参数；工具 {otherTool} 上检测到参数 ID：{sample}。" +
                $"请到「设备连接」将工具号改为 {otherTool} 后重新「应用」，再刷新/读写拧紧参数。");
        }

        return [];
    }

    public async Task<ControllerParameterBulkImportResult> ImportAllFromDeviceAsync(
        CancellationToken cancellationToken = default)
    {
        var ids = await ListDeviceParameterIdsAsync(cancellationToken).ConfigureAwait(false);
        if (ids.Count == 0)
        {
            throw new InvalidOperationException(
                "Controller returned no configured parameter IDs from #160. Refresh the device list or read by parameter ID.");
        }

        var imported = new List<int>();
        var failures = new List<ControllerParameterImportFailure>();
        foreach (var id in ids)
        {
            try
            {
                await ImportFromDeviceAsync(id, cancellationToken).ConfigureAwait(false);
                imported.Add(id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Bulk import skipped parameter {ParamId}", id);
                failures.Add(new ControllerParameterImportFailure(id, ex.Message));
            }
        }

        _logger.LogInformation(
            "Bulk imported {Imported} parameter(s) from IEMD-SD ({Failed} failed)",
            imported.Count,
            failures.Count);
        return new ControllerParameterBulkImportResult(imported, failures);
    }

    public async Task<TighteningParameterTemplate> ImportFromDeviceAsync(
        int parameterId,
        CancellationToken cancellationToken = default)
    {
        var template = await ReadFromDeviceAsync(parameterId, cancellationToken).ConfigureAwait(false);
        await SaveLocalPresetAsync(template, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Imported parameter {ParamId} from IEMD-SD to local store", parameterId);
        return template;
    }

    public async Task WriteToDeviceAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        EnsureWritableStages(template);

        try
        {
            var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
            await client.WriteParameterAsync(template, cancellationToken).ConfigureAwait(false);
        }
        catch (IemdSdCommunicationException ex) when (IsTransportIoFailure(ex))
        {
            _logger.LogWarning(ex, "Write parameter {ParamId} hit transport failure; reconnecting once", template.ParameterId);

            var reapplied = await _devices.TestConnectionAsync(cancellationToken).ConfigureAwait(false);
            if (!reapplied.Success)
                throw new IemdSdCommunicationException($"下发失败且重连未成功：{reapplied.Message}", ex);

            var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
            await client.WriteParameterAsync(template, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation("Wrote parameter {ParamId} to IEMD-SD", template.ParameterId);
    }

    public async Task DeleteFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default)
    {
        if (parameterId is < 1 or > 500)
            throw new ArgumentOutOfRangeException(nameof(parameterId), parameterId, "Parameter ID must be 1–500.");

        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.DeleteParameterAsync(parameterId, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Deleted parameter {ParamId} from IEMD-SD (#110)", parameterId);
    }

    private static void EnsureWritableStages(TighteningParameterTemplate template)
    {
        var stages = template.Core.Stages;
        var hasConfiguredStage = stages.Any(s =>
            s.TargetTorqueMilliNm > 0 || s.TargetAngleDeg > 0 || s.SpeedRpm > 0);
        if (!hasConfiguredStage)
        {
            throw new InvalidOperationException(
                "下发前请至少配置一段有效拧紧参数（目标扭矩、目标角度或转速不能全为 0）。");
        }
    }

    private static bool IsTransportIoFailure(IemdSdCommunicationException ex) =>
        ex.InnerException is System.IO.IOException or System.Net.Sockets.SocketException
        || ex.Message.Contains("Write registers", StringComparison.OrdinalIgnoreCase)
        || ex.Message.Contains("Read registers", StringComparison.OrdinalIgnoreCase)
        || ex.Message.Contains("Modbus not connected", StringComparison.OrdinalIgnoreCase);

    public async Task ActivateOnDeviceAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.SwitchParameterAsync(parameterId, screwCount, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Activated parameter {ParamId} on IEMD-SD screwCount={Count}", parameterId, screwCount);
    }

    private Task<IIemdSdClient> RequireClientAsync(CancellationToken cancellationToken) =>
        StationDeviceClientGuard.RequireIdleClientAsync(_devices, cancellationToken);
}
