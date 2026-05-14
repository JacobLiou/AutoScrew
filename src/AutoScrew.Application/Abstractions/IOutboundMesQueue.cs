namespace AutoScrew.Application.Abstractions;

public interface IOutboundMesQueue
{
    Task EnqueueAsync(LockJobResultPayload payload, string? failureReason, CancellationToken cancellationToken = default);
}
