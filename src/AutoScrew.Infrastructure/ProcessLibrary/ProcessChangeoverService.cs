using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.ProcessLibrary;

public sealed class ProcessChangeoverService : IProcessChangeoverService
{
    private readonly IProcessLibraryService _library;
    private readonly IStationProcessStateStore _stationState;
    private readonly ILogger<ProcessChangeoverService> _logger;

    public ProcessChangeoverService(
        IProcessLibraryService library,
        IStationProcessStateStore stationState,
        ILogger<ProcessChangeoverService> logger)
    {
        _library = library;
        _stationState = stationState;
        _logger = logger;
    }

    public StationProcessState? GetStationState() => _stationState.Load();

    public async Task<ChangeoverDecision> EvaluateAsync(
        string productPn,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productPn))
            throw new ArgumentException("产品 PN 不能为空。", nameof(productPn));

        var pn = productPn.Trim();
        var current = _stationState.Load();
        ProcessLibraryProductSummary? product = null;
        try
        {
            product = await _library.GetProductAsync(pn, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Evaluate changeover: failed to load product {ProductPn}", pn);
        }

        if (product is null)
        {
            return new ChangeoverDecision(
                ChangeoverReason.ProductMissing,
                pn,
                current?.ProductPn,
                null);
        }

        if (current is null || string.IsNullOrWhiteSpace(current.ProductPn))
        {
            return new ChangeoverDecision(
                ChangeoverReason.FirstDeploy,
                pn,
                null,
                product.UpdatedUtc);
        }

        if (!string.Equals(current.ProductPn, pn, StringComparison.OrdinalIgnoreCase))
        {
            return new ChangeoverDecision(
                ChangeoverReason.ProductPnChanged,
                pn,
                current.ProductPn,
                product.UpdatedUtc);
        }

        if (!Nullable.Equals(current.UpdatedUtc, product.UpdatedUtc))
        {
            return new ChangeoverDecision(
                ChangeoverReason.ProcessVersionChanged,
                pn,
                current.ProductPn,
                product.UpdatedUtc);
        }

        return new ChangeoverDecision(
            ChangeoverReason.SameSkip,
            pn,
            current.ProductPn,
            product.UpdatedUtc);
    }

    public async Task DeployAndCommitAsync(string productPn, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productPn))
            throw new ArgumentException("产品 PN 不能为空。", nameof(productPn));

        var pn = productPn.Trim();
        var product = await _library.GetProductAsync(pn, cancellationToken).ConfigureAwait(false)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{pn}");

        if (product.Slots.Count == 0)
            throw new InvalidOperationException($"产品 {pn} 下没有工艺卡，无法换产下发。");

        if (product.Sequences.Count == 0)
            throw new InvalidOperationException($"产品 {pn} 下没有拧紧顺序，无法换产下发。");

        var paramResult = await _library.DeployProductToDeviceAsync(pn, cancellationToken).ConfigureAwait(false);
        if (paramResult.Failures.Count > 0)
        {
            var fail = paramResult.Failures[0];
            throw new InvalidOperationException(
                $"换产下发参数失败（槽位 {fail.SlotId}）：{fail.Message}");
        }

        var seqResult = await _library.DeployProductSequencesToDeviceAsync(pn, cancellationToken)
            .ConfigureAwait(false);
        if (seqResult.Failures.Count > 0)
        {
            var fail = seqResult.Failures[0];
            throw new InvalidOperationException(
                $"换产下发顺序失败（顺序 {fail.SequenceId}）：{fail.Message}");
        }

        _stationState.Save(new StationProcessState(
            pn,
            product.UpdatedUtc,
            DateTimeOffset.UtcNow));

        _logger.LogInformation(
            "Changeover committed product={ProductPn} updatedUtc={UpdatedUtc} params={ParamCount} sequences={SeqCount}",
            pn,
            product.UpdatedUtc,
            paramResult.WrittenSlotIds.Count,
            seqResult.WrittenSequenceIds.Count);
    }
}
