using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.Mes;

public sealed class MesSettingsService : IMesSettingsService
{
    private readonly LocalJsonMesSettingsStore _store;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private MesRuntimeSettings _snapshot;

    public MesSettingsService(LocalJsonMesSettingsStore store)
    {
        _store = store;
        _snapshot = store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public MesRuntimeSettings GetSnapshot() => _snapshot.Clone();

    public async Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _snapshot = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
            return _snapshot.Clone();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _snapshot = settings.Clone();
            await _store.SaveAsync(_snapshot, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public void ApplySnapshot(MesRuntimeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _snapshot = settings.Clone();
    }
}
