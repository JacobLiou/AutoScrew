namespace UDL.Delta.IemdSd.Session;

/// <summary>
/// Exclusive device ownership: one mailbox transaction or tightening cycle at a time.
/// Re-entrant for the same async flow (cycle may call nested mailbox sends).
/// </summary>
internal sealed class DeviceSession : IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly AsyncLocal<int> _reentrancy = new();
    private int _busyHolders;

    public bool IsBusy => Volatile.Read(ref _busyHolders) > 0;

    public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);
        var nested = _reentrancy.Value > 0;
        if (!nested)
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            Interlocked.Increment(ref _busyHolders);
        }

        _reentrancy.Value++;
        try
        {
            return await action(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _reentrancy.Value--;
            if (!nested)
            {
                Interlocked.Decrement(ref _busyHolders);
                _gate.Release();
            }
        }
    }

    public Task RunAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken) =>
        RunAsync(async ct =>
        {
            await action(ct).ConfigureAwait(false);
            return 0;
        }, cancellationToken);

    public async ValueTask DisposeAsync()
    {
        _gate.Dispose();
        await ValueTask.CompletedTask;
    }
}
