namespace AutoScrew.Application.Abstractions;

public interface IControllerTraceService
{
    Task WriteSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
}
