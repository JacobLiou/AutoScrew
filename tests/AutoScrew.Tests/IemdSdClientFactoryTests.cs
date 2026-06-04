using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Hardware;
using UDL.Delta.IemdSd;
using Xunit;

namespace AutoScrew.Tests;

public class IemdSdClientFactoryTests
{
    [Fact]
    public void MapToOptions_SerialEndpoint_UsesRtuTransport()
    {
        var endpoint = new StationDeviceEndpoint
        {
            SlotIndex = 1,
            Transport = ControllerTransport.ModbusRtu,
            SerialPortName = "COM3",
            BaudRate = 9600,
            Parity = "Even",
            StopBits = "Two",
            ToolIndex = 1,
            TriggerMode = "Manual",
        };

        var options = IemdSdClientFactory.MapToOptions(endpoint);

        Assert.Equal(ControllerTransportType.ModbusRtu, options.Transport);
        Assert.Equal("COM3", options.SerialPortName);
        Assert.Equal(9600, options.BaudRate);
        Assert.Equal(TighteningTriggerMode.Manual, options.TriggerMode);
        Assert.Equal(1, options.ToolIndex);
    }

    [Fact]
    public void MapToOptions_TcpEndpoint_UsesTcpTransport()
    {
        var endpoint = new StationDeviceEndpoint
        {
            Transport = ControllerTransport.ModbusTcp,
            Host = "10.0.0.5",
            Port = 502,
        };

        var options = IemdSdClientFactory.MapToOptions(endpoint);

        Assert.Equal(ControllerTransportType.ModbusTcp, options.Transport);
        Assert.Equal("10.0.0.5", options.Host);
        Assert.Equal(502, options.Port);
    }
}
