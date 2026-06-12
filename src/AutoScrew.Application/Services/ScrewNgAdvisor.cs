namespace AutoScrew.Application.Services;

/// <summary>PRD §3.2.2：NG 弹窗中的处理建议（按错误码映射）。</summary>
public static class ScrewNgAdvisor
{
    public static string GetAdvice(string? errorCode, ushort? deviceErrorCode = null)
    {
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            return errorCode switch
            {
                "FLOAT_001" => "未采集到曲线，检查 Modbus 曲线读取或设备履历。",
                "FLOAT_002" => "检查浮锁：确认扭矩下限、贴合点与螺钉是否到位。",
                "OVER_TORQUE_001" => "过扭保护触发，检查设定扭矩与螺钉规格是否匹配。",
                "SKEW_003" => "轴线歪斜超限，重新对正电批与螺钉后再试。",
                "STRIP_001" => "疑似滑牙，检查螺纹、转速与角度上限。",
                "JAM_001" => "疑似卡钉，检查供钉与螺钉是否倾斜或堵塞。",
                "DEVICE_NG" => "控制器判定 NG，请在设备 HMI 查看详情后清错再试。",
                _ when errorCode.StartsWith("DEVICE_", StringComparison.Ordinal) =>
                    $"控制器错误码 {errorCode["DEVICE_".Length..]}，请技术员清错或返修。",
                _ => "请联系技术员检查曲线与设备报告后解锁。"
            };
        }

        if (deviceErrorCode is > 0)
            return $"控制器错误码 {deviceErrorCode}，请技术员在设备侧清错后解锁重试。";

        return "请联系技术员解锁后继续。";
    }
}
