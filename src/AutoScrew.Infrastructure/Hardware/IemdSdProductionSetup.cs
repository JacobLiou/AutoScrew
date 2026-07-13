using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// 产线联机：拧紧来源 #300 须为「手动设定」，#302 换参才有效（见 doc/driverAnaC.md §5.6）。
/// </summary>
public static class IemdSdProductionSetup
{
    /// <summary>手册 #300 CB：0 = 单轴独立。</summary>
    public const int SingleToolOperatingMode = 0;

    /// <summary>手册 #300 CC：0 = 手动设定；#302/#303 须此模式。</summary>
    public const int SwitchingMethodManual = 0;

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
            if (current.OperatingMode == SingleToolOperatingMode
                && current.SwitchingMethod == SwitchingMethodManual)
            {
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "ReadSourceMode before #300 write failed; proceeding with write.");
        }

        await client
            .WriteSourceModeAsync(0, SingleToolOperatingMode, SwitchingMethodManual, cancellationToken)
            .ConfigureAwait(false);
        logger.LogInformation(
            "IEMD-SD #300 manual source applied (operatingMode={Mode}, switchingMethod={Switch}).",
            SingleToolOperatingMode,
            SwitchingMethodManual);
    }
}
