using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;
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

    public async Task WriteToDeviceAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.WriteParameterAsync(template, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Wrote parameter {ParamId} to IEMD-SD", template.ParameterId);
    }

    public async Task ActivateOnDeviceAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.SwitchParameterAsync(parameterId, screwCount, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Activated parameter {ParamId} on IEMD-SD screwCount={Count}", parameterId, screwCount);
    }

    private async Task<IIemdSdClient> RequireClientAsync(CancellationToken cancellationToken)
    {
        if (!_devices.IsRuntimeDeviceAvailable)
            throw new InvalidOperationException("IEMD-SD device is not available in the current configuration.");

        await _devices.EnsureActiveClientAsync(cancellationToken).ConfigureAwait(false);
        var client = _devices.GetActiveClient()
                     ?? throw new InvalidOperationException("Active IEMD-SD device is not connected. Configure it on the Device Connection page.");
        return client;
    }
}
