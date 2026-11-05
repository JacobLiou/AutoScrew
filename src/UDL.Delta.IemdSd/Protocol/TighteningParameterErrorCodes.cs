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

    /// <summary>
    /// #100 写入异常码（SD3_101 A-25/A-26 并集；与 4.3 表对齐）。
    /// </summary>
    public static string DescribeWrite(int code) => code switch
    {
        1 => "启动阶段：最大扭矩小于最小扭矩。",
        2 => "启动阶段：最大角度小于最小角度。",
        3 => "旋入阶段：最大扭矩小于最小扭矩。",
        4 => "旋入阶段：最大角度小于最小角度。",
        5 => "预紧阶段：最大扭矩小于最小扭矩。",
        6 => "预紧阶段：最大角度小于最小角度。",
        7 => "拧紧阶段：最大扭矩小于最小扭矩。",
        8 => "拧紧阶段：最大角度小于最小角度。",
        9 => "确认拧紧设定配置之阶段顺序是否正确。",
        10 => "阶段未设定任何拧紧参数。",
        11 => "旋入阶段：设定扭矩小于最小扭矩。",
        12 => "旋入阶段：设定扭矩大于最大扭矩。",
        13 => "预紧阶段：设定扭矩小于最小扭矩。",
        14 => "预紧阶段：设定扭矩大于最大扭矩。",
        15 => "拧紧阶段：设定扭矩小于最小扭矩。",
        16 => "拧紧阶段：设定扭矩大于最大扭矩。",
        17 => "设定角度大于 32767。",
        18 => "设定扭矩超过工具规格扭矩，请降低目标扭矩或更换工具。",
        19 => "设定扭矩大于工具最大扭矩。",
        20 => "设定速度大于工具最大转速。",
        21 => "旋入阶段：扭矩不能为 0。",
        22 => "预紧阶段：扭矩不能为 0。",
        23 => "拧紧阶段：扭矩不能为 0。",
        24 => "设定旋入扭矩大于设定预紧扭矩。",
        25 => "设定旋入扭矩大于设定拧紧扭矩。",
        26 => "设定预紧扭矩大于设定拧紧扭矩。",
        27 => "设定夹紧扭矩大于最大夹紧扭矩。",
        28 => "最大夹紧扭矩小于最小夹紧扭矩。",
        29 => "最大夹紧扭矩大于工具最大扭矩。",
        30 => "拧紧阶段：设定速度小于最小速度。",
        32 => "基本设定：设定拧紧/拧松超时大于 32767。",
        33 => "基本设定：设定拧紧/拧松最大总角度大于 32767。",
        34 => "启动阶段：最小运行时间大于最大运行时间。",
        35 => "旋入阶段：最小运行时间大于最大运行时间。",
        36 => "预紧阶段：最小运行时间大于最大运行时间。",
        37 => "拧紧阶段：最小运行时间大于最大运行时间。",
        38 => "基本设定：设定拧紧超时大于阶段最大运行时间。",
        39 => "链接补偿扭矩不能为 0。",
        40 => "不能设定多个扭矩率。",
        41 => "最大夹紧角度小于最小夹紧角度。",
        42 => "不能设定多个夹紧扭矩或角度。",
        43 => "最大切换扭矩小于最小切换扭矩。",
        44 => "最大切换扭矩大于工具最大扭矩。",
        47 => "拧紧速度大于预紧速度。",
        48 => "启动速度大于旋入速度。",
        49 => "预紧速度大于旋入速度。",
        50 => "拧紧速度大于旋入速度。",
        51 => "启动阶段：设定最大扭矩大于拧紧扭矩。",
        52 => "启动阶段：设定最小扭矩大于拧紧扭矩。",
        53 => "旋入阶段：设定最大扭矩大于拧紧扭矩。",
        54 => "旋入阶段：设定最小扭矩大于拧紧扭矩。",
        55 => "旋入阶段：设定最大扭矩大于拧紧扭矩。",
        56 => "旋入阶段：设定最小扭矩大于拧紧扭矩。",
        57 => "预紧阶段：设定最大扭矩大于拧紧扭矩。",
        58 => "预紧阶段：设定最小扭矩大于拧紧扭矩。",
        59 => "参数不适用于此机种。",
        60 => "不能设定多个屈服点。",
        61 => "设定扭矩小于最小扭矩。",
        62 => "设定扭矩大于最大扭矩。",
        63 => "最大扭矩小于最小扭矩。",
        64 => "最大角度小于最小角度。",
        65 => "最小运行时间大于最大运行时间。",
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
