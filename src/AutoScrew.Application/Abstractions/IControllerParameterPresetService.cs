using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

public sealed record ControllerParameterPresetSummary(int ParameterId, string Name, int ToolIndex);

public interface IControllerParameterPresetService
{
    bool IsDeviceAvailable { get; }

    Task<IReadOnlyList<ControllerParameterPresetSummary>> ListLocalPresetsAsync(CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> LoadLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default);

    Task SaveLocalPresetAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    Task DeleteLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default);

    Task ExportToFileAsync(TighteningParameterTemplate template, string filePath, CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ReadFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ListDeviceParameterIdsAsync(CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ImportFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default);

    Task WriteToDeviceAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    Task ActivateOnDeviceAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default);
}
