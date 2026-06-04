namespace UDL.Delta.IemdSd.Protocol;

public static class TighteningParameterErrorCodes
{
    public static string Describe(int commandCode, int deviceErrorCode) =>
        commandCode switch
        {
            Modbus.ModbusFunctionCodes.WriteParameter => DescribeWrite(deviceErrorCode),
            Modbus.ModbusFunctionCodes.ReadParameter => DescribeRead(deviceErrorCode),
            _ => $"Device error code {deviceErrorCode}.",
        };

    public static string DescribeWrite(int code) => code switch
    {
        1 => "Startup: max torque < min torque.",
        2 => "Startup: max angle < min angle.",
        3 => "Run-in: max torque < min torque.",
        4 => "Run-in: max angle < min angle.",
        5 => "Preload: max torque < min torque.",
        6 => "Preload: max angle < min angle.",
        7 => "Tightening: max torque < min torque.",
        8 => "Tightening: max angle < min angle.",
        9 => "Invalid stage order configuration.",
        10 => "Stage has no tightening parameters.",
        100 => "Parameter ID out of range (1-500).",
        101 => "Tool index out of range (0-1).",
        102 => "Parameter name is empty.",
        103 => "Parameter name duplicated.",
        _ => $"Write parameter validation failed (code {code}).",
    };

    public static string DescribeRead(int code) => code switch
    {
        1 => "Parameter ID out of range (1-500).",
        2 => "Tool index out of range (0-1).",
        3 => "Parameter not configured on controller.",
        _ => $"Read parameter failed (code {code}).",
    };
}
