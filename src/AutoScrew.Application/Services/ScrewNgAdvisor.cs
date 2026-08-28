namespace AutoScrew.Application.Services;

/// <summary>PRD §3.2.2：NG 弹窗中的处理建议（按错误码映射）。</summary>
public static class ScrewNgAdvisor
{
    public static string GetAdvice(string? errorCode, ushort? deviceErrorCode = null)
    {
        if (deviceErrorCode is > 0)
            return DeviceNgDisplayFormat.BuildDeviceAdvice(deviceErrorCode.Value);

        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            if (errorCode.StartsWith("DEVICE_", StringComparison.Ordinal)
                && ushort.TryParse(errorCode["DEVICE_".Length..], out var parsed)
                && parsed > 0)
            {
                return DeviceNgDisplayFormat.BuildDeviceAdvice(parsed);
            }

            return errorCode switch
            {
                "FEED_TIMEOUT" => "供料超时：检查供料器是否响应、传感器与程序号；清障后由技术员解锁。",
                "FEED_EMPTY" => "供料缺料：补充料仓或更换料卷后由技术员解锁。",
                "FEED_JAM" => "供料卡料：检查钉道/真空与异物，排除后由技术员解锁。",
                "MISSING_SCREW_001" => "存在未完成的螺钉位，请补打全部 Pending 位后再继续或翻面。",
                "FLOAT_001" => "未采集到曲线，检查 Modbus 曲线读取或设备履历。",
                "FLOAT_002" => "检查浮锁：确认扭矩下限、贴合点与螺钉是否到位。",
                "OVER_TORQUE_001" => "过扭保护触发，检查设定扭矩与螺钉规格是否匹配。",
                "SKEW_003" => "轴线歪斜超限，重新对正电批与螺钉后再试。",
                "STRIP_001" => "疑似滑牙，检查螺纹、转速与角度上限。",
                "JAM_001" => "疑似卡钉，检查供钉与螺钉是否倾斜或堵塞。",
                "DEVICE_NG" => "控制器判定 NG。请「退出作业」挂起，在设备上清错并确认后，再扫同一 SN 恢复。",
                _ when errorCode.StartsWith("FEED_", StringComparison.Ordinal) =>
                    "供料异常，请检查供料器与料仓后由技术员解锁。",
                _ when errorCode.StartsWith("DEVICE_", StringComparison.Ordinal) =>
                    "控制器判定 NG。请「退出作业」挂起，在设备上清错后，再扫同一 SN 恢复。",
                _ => "请联系技术员检查曲线与设备报告后解锁。"
            };
        }

        return "请联系技术员解锁后继续。";
    }

    public static bool IsFeedError(string? errorCode) =>
        !string.IsNullOrWhiteSpace(errorCode)
        && errorCode.StartsWith("FEED_", StringComparison.Ordinal);

    public static bool IsDeviceError(string? errorCode) =>
        !string.IsNullOrWhiteSpace(errorCode)
        && (string.Equals(errorCode, "DEVICE_NG", StringComparison.Ordinal)
            || errorCode.StartsWith("DEVICE_", StringComparison.Ordinal));
}
