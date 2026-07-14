using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class ControllerSourceConfigService : IControllerSourceConfigService
{
    private readonly LocalJsonControllerSourceConfigStore _store;
    private readonly IStationDeviceService _devices;
    private readonly ILogger<ControllerSourceConfigService> _logger;

    public ControllerSourceConfigService(
        LocalJsonControllerSourceConfigStore store,
        IStationDeviceService devices,
        ILogger<ControllerSourceConfigService> logger)
    {
        _store = store;
        _devices = devices;
        _logger = logger;
    }

    public bool IsDeviceAvailable => _devices.IsRuntimeDeviceAvailable;

    public async Task<ProductionTighteningMode> LoadProductionControlModeAsync(CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        return doc.ProductionControlMode;
    }

    public async Task SaveProductionControlModeAsync(ProductionTighteningMode mode, CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        doc.ProductionControlMode = mode;
        await _store.SaveAsync(doc, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TighteningSourceModeCore> LoadLocalModeAsync(CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        return doc.Mode;
    }

    public async Task SaveLocalModeAsync(TighteningSourceModeCore mode, CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        doc.Mode = mode;
        await _store.SaveAsync(doc, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TighteningSourceContentCore> LoadLocalContentAsync(CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        return doc.Content;
    }

    public async Task SaveLocalContentAsync(TighteningSourceContentCore content, CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        doc.Content = content;
        await _store.SaveAsync(doc, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ControllerSourceBindingEntry>> LoadBindingsAsync(CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        EnsureBindingsMigrated(doc);
        return doc.Bindings;
    }

    public async Task SaveBindingsAsync(
        IReadOnlyList<ControllerSourceBindingEntry> bindings,
        TighteningSourceModeCore mode,
        CancellationToken cancellationToken = default)
    {
        var doc = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        doc.Mode = mode;
        doc.Bindings = bindings.ToList();
        doc.Content = ControllerSourceConfigProjection.ToPrimaryContent(mode.OperatingMode, doc.Bindings, doc.Content);
        await _store.SaveAsync(doc, cancellationToken).ConfigureAwait(false);
    }

    private static void EnsureBindingsMigrated(ControllerSourceConfigDocument doc)
    {
        if (doc.Bindings.Count > 0)
            return;

        doc.Bindings = ControllerSourceConfigProjection.FromLegacyContent(doc.Content);
    }

    public async Task<(TighteningSourceModeCore Mode, TighteningSourceContentCore Content)> ReadFromDeviceAsync(
        CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        var modeSnap = await client.ReadSourceModeAsync(cancellationToken).ConfigureAwait(false);
        var contentSnap = await client.ReadSourceContentAsync(1, cancellationToken).ConfigureAwait(false);
        return (modeSnap.ToModeCore(), contentSnap.ToContentCore());
    }

    public async Task WriteToDeviceAsync(
        TighteningSourceModeCore mode,
        TighteningSourceContentCore content,
        CancellationToken cancellationToken = default)
    {
        var client = await RequireClientAsync(cancellationToken).ConfigureAwait(false);
        await client.WriteSourceModeAsync(
            mode.ToolIndex,
            (int)mode.OperatingMode,
            (int)mode.SwitchingMethod,
            cancellationToken).ConfigureAwait(false);

        if (content.BindingType == TighteningSourceBindingType.Sequence)
        {
            await client.WriteSourceContentAsync(
                content.SwitchingMethodId,
                0,
                content.TargetId,
                content.ScrewCount,
                cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await client.WriteSourceContentAsync(
                content.SwitchingMethodId,
                content.TargetId,
                0,
                content.ScrewCount,
                cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation(
            "Wrote source mode/content to IEMD-SD binding={Binding} target={Target}",
            content.BindingType,
            content.TargetId);
    }

    private Task<IIemdSdClient> RequireClientAsync(CancellationToken cancellationToken) =>
        StationDeviceClientGuard.RequireIdleClientAsync(_devices, cancellationToken);
}
