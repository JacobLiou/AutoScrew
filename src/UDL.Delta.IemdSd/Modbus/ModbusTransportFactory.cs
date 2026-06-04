using Microsoft.Extensions.Logging;
using System.IO.Ports;
using UDL.Delta.IemdSd;

namespace UDL.Delta.IemdSd.Modbus;

internal static class ModbusTransportFactory
{
    public static IModbusTransport Create(IemdSdClientOptions options, ILogger logger) =>
        options.Transport switch
        {
            ControllerTransportType.ModbusRtu => new ModbusRtuTransport(options, logger),
            _ => new ModbusTransport(options, logger),
        };

    internal static Parity ParseParity(string value) =>
        value.ToUpperInvariant() switch
        {
            "ODD" => Parity.Odd,
            "EVEN" => Parity.Even,
            "MARK" => Parity.Mark,
            "SPACE" => Parity.Space,
            _ => Parity.None,
        };

    internal static StopBits ParseStopBits(string value) =>
        value.ToUpperInvariant() switch
        {
            "TWO" or "2" => StopBits.Two,
            "ONEPOINTFIVE" or "1.5" => StopBits.OnePointFive,
            _ => StopBits.One,
        };
}
