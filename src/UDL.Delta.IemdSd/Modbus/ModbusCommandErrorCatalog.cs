using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Modbus;

public static class ModbusCommandErrorCatalog
{
    public static string Describe(int commandCode, int deviceErrorCode) =>
        TighteningParameterErrorCodes.Describe(commandCode, deviceErrorCode);
}
