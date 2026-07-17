using AutoScrew.Application.Configuration;using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

public interface IControllerSourceConfigService
{
    bool IsDeviceAvailable { get; }

    Task<ProductionTighteningMode> LoadProductionControlModeAsync(CancellationToken cancellationToken = default);

    Task SaveProductionControlModeAsync(ProductionTighteningMode mode, CancellationToken cancellationToken = default);

    Task<TighteningSourceModeCore> LoadLocalModeAsync(CancellationToken cancellationToken = default);

    Task SaveLocalModeAsync(TighteningSourceModeCore mode, CancellationToken cancellationToken = default);

    Task<TighteningSourceContentCore> LoadLocalContentAsync(CancellationToken cancellationToken = default);

    Task SaveLocalContentAsync(TighteningSourceContentCore content, CancellationToken cancellationToken = default);

    Task<(TighteningSourceModeCore Mode, TighteningSourceContentCore Content)> ReadFromDeviceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>#351 按切换方式 ID（来源槽）读取单条来源内容。</summary>
    Task<TighteningSourceContentCore> ReadDeviceContentBySwitchingIdAsync(
        int switchingMethodId,
        CancellationToken cancellationToken = default);

    Task WriteToDeviceAsync(
        TighteningSourceModeCore mode,
        TighteningSourceContentCore content,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ControllerSourceBindingEntry>> LoadBindingsAsync(CancellationToken cancellationToken = default);

    Task SaveBindingsAsync(
        IReadOnlyList<ControllerSourceBindingEntry> bindings,
        TighteningSourceModeCore mode,
        CancellationToken cancellationToken = default);
}
