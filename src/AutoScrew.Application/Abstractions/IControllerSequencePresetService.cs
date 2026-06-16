using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

public sealed record ControllerSequencePresetSummary(int SequenceId, string Name, int StepCount);

public interface IControllerSequencePresetService
{
    bool IsDeviceAvailable { get; }

    Task<IReadOnlyList<ControllerSequencePresetSummary>> ListLocalPresetsAsync(CancellationToken cancellationToken = default);

    Task<TighteningSequencePackage> LoadLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task SaveLocalPresetAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default);

    Task DeleteLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task<TighteningSequencePackage> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default);

    Task ExportToFileAsync(TighteningSequencePackage package, string filePath, CancellationToken cancellationToken = default);

    Task<TighteningSequencePackage> ReadFromDeviceAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task WriteToDeviceAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default);

    Task ActivateOnDeviceAsync(int sequenceId, CancellationToken cancellationToken = default);
}
