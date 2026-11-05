---
name: COMM log readability
overview: "从协议 PDF 补全 #100 异常码释义（含 21），并改造 COMM 日志分层与 Reject 文案，让失败一眼可读、成功帧不再淹没排查。"
todos:
  - id: fill-write-errors
    content: 从 SD3_101 A-25/A-26 补全 DescribeWrite（含 21），并集覆盖 4.3 表
    status: completed
  - id: enrich-mailbox-log
    content: CommandMailbox Reject/异常消息加入 CommandName + Describe
    status: completed
  - id: demote-transport-inf
    content: LoggingModbusTransport 成功帧改为 Debug
    status: completed
  - id: tests-docs
    content: 单测 100/21 + driverDebug.md 补一行
    status: completed
isProject: false
---

# 通信日志可读性改善

## 现状问题

截图里 `DeviceErrorCode=21` **已经打出数字**，但难查是因为：

1. **[TighteningParameterErrorCodes.DescribeWrite](src/UDL.Delta.IemdSd/Protocol/TighteningParameterErrorCodes.cs)** 只覆盖 `1–10 / 18 / 100–103`，`21` 落到泛化「写入参数校验失败 (code 21)」。
2. Reject 行只有编号，没有命令中文名 / 手册释义。
3. [LoggingModbusTransport](src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs) 把每次 `ReadHolding` / `WriteMultiple` 成功都打 **INF**，一次 `#100` 会被大量帧日志淹没，难扫到真正的 Reject。

## 协议依据（已核对 PDF）

两份手册 **#100** 异常表一致（10.1 更全）：

- [doc/protocol/DELTA_IA-DCSS_SD3_101.pdf](doc/protocol/DELTA_IA-DCSS_SD3_101.pdf) A-24～A-26（约 p.235–237）
- [doc/protocol/DELTA_IA-DCSS_SD30X0_43.pdf](doc/protocol/DELTA_IA-DCSS_SD30X0_43.pdf) A-19～A-20（约 p.172–173）

**`21` 原文：旋入阶段：扭矩不能为 0**（旋入段目标扭矩为 0 时 `#100` 拒绝）。

完整表还包括 `11–20`、`22–42`、`47–65`、`100–103` 等；10.1 另有 `43/44/60`（4.3 未列）。实现时以 **10.1 并集** 写入代码，与现有中文风格对齐。

## 改动方案（定稿）

### 1. 补全 `#100` 异常码映射

更新 [TighteningParameterErrorCodes.cs](src/UDL.Delta.IemdSd/Protocol/TighteningParameterErrorCodes.cs) 的 `DescribeWrite`：按 PDF 表补齐全部已知码（含 **21**）。`ModbusCommandErrorCatalog` 已转发，无需改 API。

同步在 [ModbusCommandErrorCatalogTests.cs](tests/UDL.Delta.IemdSd.Tests/ModbusCommandErrorCatalogTests.cs) 增加 `100/21 → 旋入` 等关键断言。

### 2. 邮箱层日志带释义 + 命令名

改 [CommandMailbox.cs](src/UDL.Delta.IemdSd/Modbus/CommandMailbox.cs) Reject / Timeout（Success 可选带 Name）：

```text
COMM Command #100 Write the parameter Rejected DeviceErrorCode=21 Describe=旋入阶段：扭矩不能为 0。 ElapsedMs=…
```

- `CommandName` 来自已有 [ModbusCommandCatalog.g.cs](src/UDL.Delta.IemdSd/Modbus/ModbusCommandCatalog.g.cs)
- `Describe` 来自 `ModbusCommandErrorCatalog.Describe`
- 抛出的 `IemdSdCommunicationException` 消息改为含 `Describe`（HMI 已读 `DeviceErrorCode` 再 Describe，日志与弹窗一致）

### 3. 传输层降噪（默认可查）

改 [LoggingModbusTransport.cs](src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs)：

- **成功** 的 `Read*/Write*` → `LogDebug`（保留 Data/地址，需要抓包时开 Debug）
- **失败 / Cancel** 保持 Warning/Debug 不变

业务侧已有的高层 INF（如 `COMM ParameterWrite …`、`COMM Command #N Started/Success/Rejected`）仍为排查主路径。

### 4. 文档锚点（便于人眼查）

在 [doc/driverDebug.md](doc/driverDebug.md) `#100` 排错表补一行：`code 21 → 旋入扭矩为 0`，并注明完整表见协议 PDF A-25/A-26；不另造大段重复手册。

## 不做的范围

- 不为每条 Modbus 帧加 CorrelationId / AsyncLocal（成本高，本轮用「降噪 + Reject 释义」即可）。
- 不改 MES/HMI 其它日志体系；不生成 PDF 抽取脚本进 CI。

## 验收

- 复现 `#100` 旋入扭矩=0 → 日志/异常含「旋入阶段：扭矩不能为 0」。
- 默认 INF 以 `COMM Command #…` / `COMM ParameterWrite` 为主，不再被成功 ReadHolding 刷屏。
- 单测 `Describe(100, 21)` 通过。
