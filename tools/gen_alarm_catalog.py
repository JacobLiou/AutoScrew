"""Generate DeviceAlarmCodeCatalog.g.cs from Delta SD3 EN manual CH12 list."""
from __future__ import annotations

import re
from pathlib import Path

from pypdf import PdfReader

ROOT = Path(__file__).resolve().parents[1]
PDF = ROOT / "doc" / "DELTA_IA-DCSS_SD3_UM_EN_20260310.pdf"
OUT = ROOT / "src" / "UDL.Delta.IemdSd" / "Protocol" / "DeviceAlarmCodeCatalog.g.cs"

REPLS = [
    ("Rundown stage (torque rate)", "旋入阶段(扭矩率)"),
    ("Pre-tightening stage (torque rate)", "预紧阶段(扭矩率)"),
    ("Tightening stage (torque rate)", "拧紧阶段(扭矩率)"),
    ("Start stage", "启动阶段"),
    ("Rundown stage", "旋入阶段"),
    ("Pre-tightening stage", "预紧阶段"),
    ("Tightening stage", "拧紧阶段"),
    ("Loosening stage", "拧松阶段"),
    # Full sentences before generic Tightening/Loosening tokens
    ("Tightening signal ends too early", "拧紧信号提早消失"),
    ("Tightening: parameter exceeded the tool specification. Check the torque or", "拧紧：参数超出工具规格，请检查扭矩"),
    ("Tightening: parameter does not match the tool. Recreate a tightening", "拧紧：参数与工具不匹配，请重建参数"),
    ("Tightening operation prohibited by SDSoft", "SDSoft禁止拧紧操作"),
    ("Loosening prohibited after tightening OK", "拧紧OK后禁止拧松"),
    ("Loosening prohibited after tightening NOK", "拧紧NG后禁止拧松"),
    ("Cannot perform tightening and loosening at the same time", "不可同时拧紧与拧松"),
    ("Incorrect parameter setting. Tightening prohibited.", "参数设定不正确，禁止拧紧"),
    ("Incorrect parameter setting. Loosening prohibited.", "参数设定不正确，禁止拧松"),
    ("Tightening prohibited by remote communication or DI", "远程通讯或DI禁止拧紧"),
    ("Loosening prohibited by remote communication or DI", "远程通讯或DI禁止拧松"),
    ("Exceeded the max. count for NOK loosening", "超出最大NG拧松次数"),
    ("Screw quantity reached. Tightening prohibited.", "螺丝数量已达成，禁止拧紧"),
    ("The scanner string is null or the string length is incorrect. Tightening prohibited.", "扫码字符串为空或长度不正确，禁止拧紧"),
    ("Loosening", "拧松"),
    ("Tightening", "拧紧"),
    ("Stage 1", "阶段1"),
    ("Stage 2", "阶段2"),
    ("Stage 3", "阶段3"),
    ("Stage 4", "阶段4"),
    ("Stage 5", "阶段5"),
    ("Stage 6", "阶段6"),
    ("exceeded the tool torque protection range", "超出工具扭矩防护"),
    ("exceeded the tightening torque protection range", "超出拧紧扭矩防护"),
    ("exceeded the max. operation time", "运行时间过长"),
    ("lower than the min. operation time", "运行时间过短"),
    ("exceeded the switching torque", "超出切换扭矩"),
    ("lower than the switching torque", "低于切换扭矩"),
    ("exceeded tool max. current", "超出工具电流上限"),
    ("lower than tool min. current", "低于工具电流下限"),
    ("exceeded the max. clamp torque", "超出最大夹紧扭矩"),
    ("lower than the min. clamp torque", "低于最小夹紧扭矩"),
    ("exceeded the max. clamp angle", "超出最大夹紧角度"),
    ("lower than the min. clamp angle", "低于最小夹紧角度"),
    ("exceeded the max. angle", "超出最大角度值"),
    ("lower than the min. angle", "小于最小角度值"),
    ("exceeded the max. torque", "超出最大扭矩值"),
    ("lower than the min. torque", "低于最小扭矩值"),
    ("exceeded the max. rotation angle", "超出最大总角度"),
    ("lower than the min. rotation angle", "低于最小总角度"),
    ("error occurs during parameter setting", "配置参数时发生异常"),
    ("operation error", "运行错误"),
    ("timeout", "过程超时"),
    ("did not reach the set torque value", "未到达设定扭矩值"),
    ("Unknown parameter", "未知参数内容"),
    ("Bit camming-out", "批头滑牙/脱出"),
    ("Overcurrent", "过电流"),
    ("Tool combination error", "工具匹配异常"),
    ("Overload", "过负载"),
    ("Excessive control error of Speed command", "速度控制误差过大"),
    ("Tool communication error", "工具通讯异常"),
    ("Emergency stop", "紧急停止"),
    ("MOSFET overheating", "MOSFET过热"),
    ("No response from tool encoder", "工具编码器无反应"),
    ("Tool encoder communication error", "工具编码器通讯异常"),
    ("Tool encoder alarm", "工具编码器警报"),
    ("Tool encoder warning", "工具编码器警告"),
    ("Tool control board did not complete the read / write procedure", "工具控制板读写操作未完成"),
    ("Controller outputs excessive current", "控制器输出电流过大"),
    ("Tool torque exceeded the sensor spec.", "工具扭矩超出感测器规格"),
    ("Tool torque sensor error", "工具扭矩感测器异常"),
    ("No response from tool torque sensor", "工具扭矩感测器无反应"),
    ("Tool torque sensor communication error", "工具扭矩感测器通讯异常"),
    ("Tool torque sensor alarm", "工具扭矩感测器警报"),
    ("Tool torque sensor initialization failed", "工具扭矩感测器初始化失败"),
    ("Tool temperature error when the power is on", "工具上电时温度异常"),
    ("Tool temperature error", "工具温度异常"),
    ("Cam-out detection error", "批头脱出检测异常"),
    ("Cam-out or tool perpendicularity detection sensor is in error", "批头脱出或垂直度感测器异常"),
    ("Tool parameter write-in failed", "工具参数写入失败"),
    ("Key parameter reading error", "关键参数读取错误"),
    ("Tool temperature is too high", "工具温度过高"),
    ("Tool temperature is abnormal", "工具温度异常"),
    ("EEPROM not reset after firmware update", "固件升级后未重置EEPROM"),
    ("Tool data error when the power is on (partial)", "上电时工具数据错误(部分)"),
    ("Tool data error when the power is on (1030)", "上电时工具数据错误(1030)"),
    ("Tool data error when the power is on (1078)", "上电时工具数据错误(1078)"),
    ("Tool data error when the power is on (1060)", "上电时工具数据错误(1060)"),
    ("Tool data error when the power is on (1038)", "上电时工具数据错误(1038)"),
    ("Quantity not reached. String scanning prohibited.", "数量未达成，禁止扫码"),
    ("Send the tool back for service", "请送修工具"),
    ("Parameters not set", "参数未设定"),
    ("Unknown tool model and specification. Update the firmware.", "未知工具型号规格，请更新固件"),
    ("Parameter setting prohibited when tool is in operation", "工具运行中禁止设定参数"),
    ("Tool final current is lower than the range. Check the tightening parameter.", "工具最终电流偏低，请检查拧紧参数"),
    ("Positioning arm is not configured", "定位臂未配置"),
    ("Exceeded the max. count for NOK tightening", "超出最大NG拧紧次数"),
    ("The scanner string is null or the string length is incorrect. Tightening prohibited.", "扫码字符串为空或长度不正确，禁止拧紧"),
    ("Screw quantity reached. Tightening prohibited.", "螺丝数量已达成，禁止拧紧"),
    ("Exceeded max. operation time", "超出最大运行时间"),
    ("Incorrect parameter setting. Tightening prohibited.", "参数设定不正确，禁止拧紧"),
    ("Tightening prohibited by remote communication or DI", "远程通讯或DI禁止拧紧"),
]


def to_int(s: str) -> int:
    return int(s, 16) if any(c in "ABCDEFabcdef" for c in s) else int(s, 10)


def clean_name(name: str) -> str:
    name = name.strip()
    # duplicated full string
    half = len(name) // 2
    if len(name) > 20 and name[:half].strip() == name[half:].strip():
        name = name[:half].strip()
    # "Start stage: A Start stage: B" → first clause
    m = re.match(
        r"^((?:Start|Rundown|Pre-tightening|Tightening|Loosening|Stage \d+)(?: stage)?(?: \(torque rate\))?: .+?)"
        r"(?=\s+(?:Start|Rundown|Pre-tightening|Tightening|Loosening|Stage \d+))",
        name,
    )
    if m:
        name = m.group(1).strip()
    return name


def to_zh(en: str) -> str:
    s = en
    for a, b in REPLS:
        s = s.replace(a, b)
    s = s.replace(": ", ":")
    return s


def parse_entries() -> dict[int, str]:
    reader = PdfReader(str(PDF))
    full = "\n".join((reader.pages[i].extract_text() or "") for i in range(160, 168))
    if "12.2" in full:
        full = full.split("12.2")[0]
    lines = [ln.strip() for ln in full.splitlines() if ln.strip()]
    entries: dict[int, str] = {}
    pending: list[int] = []
    for line in lines:
        m = re.match(
            r"^(AL|NG|WN)([0-9A-Fa-f]{4})\s*&\s*(?:AL|NG|WN)([0-9A-Fa-f]{4})\s*(.*)$",
            line,
        )
        if not m:
            continue
        c1, c2 = to_int(m.group(2)), to_int(m.group(3))
        name = m.group(4).strip()
        if not name:
            pending.extend([c1, c2])
            continue
        name = clean_name(name)
        for c in pending + [c1, c2]:
            if 0 <= c <= 65535:
                entries[c] = name
        pending = []
    return entries


def main() -> None:
    entries = parse_entries()
    lines = [
        "// <auto-generated />",
        "// From Delta SD3 CH12 alarm list (EN manual pages). Regenerate via tools/gen_alarm_catalog.py.",
        "namespace UDL.Delta.IemdSd.Protocol;",
        "",
        "internal static class DeviceAlarmCodeCatalogData",
        "{",
        "    internal static readonly Dictionary<ushort, string> EnglishNames = new()",
        "    {",
    ]
    for c in sorted(entries):
        en = entries[c].replace("\\", "\\\\").replace('"', '\\"')
        lines.append(f"        [{c}] = \"{en}\",")
    lines += [
        "    };",
        "",
        "    internal static readonly Dictionary<ushort, string> ChineseNames = new()",
        "    {",
    ]
    for c in sorted(entries):
        zh = to_zh(entries[c]).replace("\\", "\\\\").replace('"', '\\"')
        lines.append(f"        [{c}] = \"{zh}\",")
    lines += ["    };", "}", ""]
    OUT.write_text("\n".join(lines), encoding="utf-8")
    print(f"wrote {len(entries)} codes -> {OUT}")
    print("3224:", to_zh(entries[3224]))


if __name__ == "__main__":
    main()
