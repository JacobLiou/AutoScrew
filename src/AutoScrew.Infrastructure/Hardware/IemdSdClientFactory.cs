using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;

namespace AutoScrew.Infrastructure.Hardware;

public sealed class IemdSdClientFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public IemdSdClientFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public IIemdSdClient Create(StationDeviceEndpoint endpoint)
    {
        var options = MapToOptions(endpoint);
        return new IemdSdClient(options, _loggerFactory.CreateLogger<IemdSdClient>());
    }

    public static IemdSdClientOptions MapToOptions(StationDeviceEndpoint endpoint) => new()
    {
        Transport = endpoint.Transport == ControllerTransport.ModbusRtu
            ? ControllerTransportType.ModbusRtu
            : ControllerTransportType.ModbusTcp,
        Host = endpoint.Host,
        Port = endpoint.Port,
        SerialPortName = endpoint.SerialPortName,
        BaudRate = endpoint.BaudRate,
        DataBits = endpoint.DataBits,
        Parity = endpoint.Parity,
        StopBits = endpoint.StopBits,
        ToolIndex = endpoint.ToolIndex,
        TriggerMode = string.Equals(endpoint.TriggerMode, "Manual", StringComparison.OrdinalIgnoreCase)
            ? TighteningTriggerMode.Manual
            : TighteningTriggerMode.AutoDi,
        AutoLockOnInit = endpoint.AutoLockOnInit,
        SendUnlockAfterCycle = endpoint.SendUnlockAfterCycle,
        UseLegacyFinishRegister = endpoint.UseLegacyFinishRegister,
        CommandTimeoutMs = endpoint.CommandTimeoutMs,
    };
}
