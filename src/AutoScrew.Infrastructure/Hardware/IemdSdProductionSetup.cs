using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// 产线联机：拧紧来源 #300 须为「手动设定」，#302 换参才有效（见 doc/driverAnaC.md §5.6）。
/// </summary>
public static class IemdSdProductionSetup
{
    /// <summary>手册：拧紧来源 = 手动设定（单来源）。</summary>
    public const int OperatingModeManualSet = 0;

    /// <summary>手册：切换方式 = 手动设定；非 1 时 #302 会拒收。</summary>
    public const int SwitchingMethodManualSet = 1;

    public static async Task EnsureManualSourceAsync(
        IIemdSdClient client,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(logger);

        try
        {
            var current = await client.ReadSourceModeAsync(cancellationToken).ConfigureAwait(false);
            if (current.OperatingMode == OperatingModeManualSet
                && current.SwitchingMethod == SwitchingMethodManualSet)
            {
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "ReadSourceMode before #300 write failed; proceeding with write.");
        }

        await client
            .WriteSourceModeAsync(OperatingModeManualSet, SwitchingMethodManualSet, cancellationToken)
            .ConfigureAwait(false);
        logger.LogInformation(
            "IEMD-SD #300 manual source applied (operatingMode={Mode}, switchingMethod={Switch}).",
            OperatingModeManualSet,
            SwitchingMethodManualSet);
    }
}
