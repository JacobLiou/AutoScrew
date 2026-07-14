namespace UDL.Delta.IemdSd.Protocol;

public static class TighteningParameterErrorCodes
{
    public static string Describe(int commandCode, int deviceErrorCode) =>
        commandCode switch
        {
            Modbus.ModbusFunctionCodes.WriteParameter => DescribeWrite(deviceErrorCode),
            Modbus.ModbusFunctionCodes.ReadParameter => DescribeRead(deviceErrorCode),
            Modbus.ModbusFunctionCodes.SwitchParameter => DescribeSwitchParameter(deviceErrorCode),
            (int)Modbus.ModbusFunctionCode.Read_created_sets_tightening_parameters => DescribeListParameters(deviceErrorCode),
            (int)Modbus.ModbusFunctionCode.Write_sequence => DescribeSequenceWrite(deviceErrorCode),
            (int)Modbus.ModbusFunctionCode.Read_sequence => DescribeSequenceRead(deviceErrorCode),
            (int)Modbus.ModbusFunctionCode.Write_switch_sequence_under_manual_setting => DescribeSwitchSequence(deviceErrorCode),
            (int)Modbus.ModbusFunctionCode.Write_operating_mode_switching_method_source => DescribeSourceMode(deviceErrorCode),
            (int)Modbus.ModbusFunctionCode.Write_contents_single_source => DescribeSourceContent(deviceErrorCode),
            _ => $"设备错误码 {deviceErrorCode}（命令 #{commandCode}）。",
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
        2 => "工具索引超出范围 (0–1)。请到设备连接页核对「工具号」。",
        3 => "控制器上未配置该参数 ID（或工具号不匹配）。当前读写使用的工具号下没有此 ID：请到「设备连接」确认工具号 0/1 后「应用」，再「刷新设备列表」用不在空列表里的 ID 读取。",
        _ => $"读取参数失败 (code {code})。",
    };

    public static string DescribeSwitchParameter(int code) => code switch
    {
        1 => "切换方式须为「手动设定」( #300 CC=0 ) 才能使用 #302。",
        3 => "参数 ID 未配置（或工具号不匹配）。",
        11 => "无法切换：参数未配置或工具号/来源状态不允许（请核对工具号与 #300 手动）。",
        _ => $"切换参数失败 (code {code})。",
    };

    public static string DescribeListParameters(int code) => code switch
    {
        1 => "工具索引超出范围 (0–1)。请核对设备连接页「工具号」。",
        _ => $"读取参数列表 #160 失败 (code {code})。",
    };

    public static string DescribeSequenceWrite(int code) => code switch
    {
        1 => "顺序 ID 超出范围。",
        2 => "顺序内容校验失败。",
        _ => $"写入顺序 #200 失败 (code {code})。",
    };

    public static string DescribeSequenceRead(int code) => code switch
    {
        1 => "顺序 ID 超出范围。",
        3 => "控制器上未配置该顺序 ID。请先刷新顺序列表。",
        _ => $"读取顺序 #250 失败 (code {code})。",
    };

    public static string DescribeSwitchSequence(int code) => code switch
    {
        1 => "切换方式须为「手动设定」( #300 CC=0 ) 才能使用 #303。",
        3 => "顺序 ID 未配置。",
        _ => $"切换顺序 #303 失败 (code {code})。",
    };

    public static string DescribeSourceMode(int code) =>
        $"写入来源模式 #300 失败 (code {code})。";

    public static string DescribeSourceContent(int code) =>
        $"写入来源内容 #301 失败 (code {code})。";
}
