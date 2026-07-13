namespace UDL.Delta.IemdSd.Protocol;

public static class TighteningParameterErrorCodes
{
    public static string Describe(int commandCode, int deviceErrorCode) =>
        commandCode switch
        {
            Modbus.ModbusFunctionCodes.WriteParameter => DescribeWrite(deviceErrorCode),
            Modbus.ModbusFunctionCodes.ReadParameter => DescribeRead(deviceErrorCode),
            Modbus.ModbusFunctionCodes.SwitchParameter => DescribeSwitchParameter(deviceErrorCode),
            _ => $"设备错误码 {deviceErrorCode}。",
        };

    public static string DescribeWrite(int code) => code switch
    {
        1 => "启动段：最大扭矩小于最小扭矩。",
        2 => "启动段：最大角度小于最小角度。",
        3 => "磨合段：最大扭矩小于最小扭矩。",
        4 => "磨合段：最大角度小于最小角度。",
        5 => "预压段：最大扭矩小于最小扭矩。",
        6 => "预压段：最大角度小于最小角度。",
        7 => "拧紧段：最大扭矩小于最小扭矩。",
        8 => "拧紧段：最大角度小于最小角度。",
        9 => "阶段顺序配置无效。",
        10 => "阶段未配置拧紧参数。",
        18 => "设定扭矩超过工具规格扭矩，请降低目标扭矩或更换工具。",
        100 => "参数 ID 超出范围 (1–500)。",
        101 => "工具索引超出范围 (0–1)。",
        102 => "参数名称为空。",
        103 => "参数名称重复。",
        _ => $"写入参数校验失败 (code {code})。",
    };

    public static string DescribeRead(int code) => code switch
    {
        1 => "参数 ID 超出范围 (1–500)。",
        2 => "工具索引超出范围 (0–1)。",
        3 => "控制器上未配置该参数 ID，请先用 #160 列表确认或从设备导入。",
        _ => $"读取参数失败 (code {code})。",
    };

    public static string DescribeSwitchParameter(int code) => code switch
    {
        1 => "切换方式须为「手动设定」( #300 CC=0 ) 才能使用 #302。",
        _ => $"切换参数失败 (code {code})。",
    };
}
