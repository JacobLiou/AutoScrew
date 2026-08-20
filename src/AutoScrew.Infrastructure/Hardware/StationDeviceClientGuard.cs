using AutoScrew.Application.Abstractions;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Exceptions;

namespace AutoScrew.Infrastructure.Hardware;

internal static class StationDeviceClientGuard
{
    public static async Task<IIemdSdClient> RequireIdleClientAsync(
        IStationDeviceService devices,
        CancellationToken cancellationToken)
    {
        if (!devices.IsRuntimeDeviceAvailable)
            throw new InvalidOperationException("IEMD-SD device is not available in the current configuration.");

        if (devices.IsDeviceBusy)
            throw new IemdSdDeviceBusyException(
                "设备正忙（作业拧紧周期进行中），请等待完成后再操作。");

        await devices.EnsureClientAsync(cancellationToken).ConfigureAwait(false);
        return devices.GetClient()
               ?? throw new InvalidOperationException("IEMD-SD device is not connected. Configure it on the Device Connection page.");
    }
}
