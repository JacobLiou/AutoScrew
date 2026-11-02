---
name: Comm Log Full Dump
overview: 根因是 LoggingModbusTransport 把寄存器摘要硬截断为最多 16 个 word；拧紧参数约 349 word、顺序约 530 word，诊断时几乎只看到开头。计划改为完整可检索的十六进制转储，并补齐失败路径与文档/测试。
todos:
  - id: formatter-full-dump
    content: LoggingModbusTransport：完整 hex + 偏移标记 + SHA256，去掉 16-word 截断
    status: completed
  - id: failure-payload
    content: 写失败 Warning 尽量附带 Data/Sha256
    status: completed
  - id: tests-update
    content: 更新/新增 LoggingModbusTransportTests 断言完整载荷
    status: completed
  - id: readme-comm
    content: readme 通讯日志小节与截断说明对齐新行为
    status: completed
isProject: false
---

# 通讯日志完整可诊断化

## 根因

当前实现在 [`LoggingModbusTransport.Summarize`](src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs) 中硬编码：

```96:103:src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs
    private static string Summarize(int[]? values)
    {
        ...
        var summary = string.Join(" ", values.Take(16).Select(...));
        return values.Length > 16 ? $"[{summary} ...] (... truncated)" : $"[{summary}]";
    }
```

[`readme.md`](readme.md) 也写明「最多展示 16 个寄存器」。拧紧参数块 [`ParameterBlockWordCount`](src/UDL.Delta.IemdSd/Modbus/ModbusRegisterMap.cs) = `0x22E - 0xD2 + 1` = **349**；顺序块 [`BlockWordCount`](src/UDL.Delta.IemdSd/Protocol/TighteningSequenceRegisterMap.cs) ≈ **530**。写 `#100` / 读 `#150` / 顺序读写时，Information 日志里 `Data=` 只留前 16 个 word，无法对照工艺字段。

逻辑层一次 `WriteMultiple`/`ReadHolding` 已覆盖整块；TCP 物理分块在 [`ModbusTransport.WriteMultipleCoreAsync`](src/UDL.Delta.IemdSd/Modbus/ModbusTransport.cs) 内部完成，外层日志本就只记逻辑一次——问题不在分块，而在摘要截断。

```mermaid
flowchart LR
  cmd["Command #100/#150/#200"] --> mailbox[CommandMailbox]
  mailbox --> write["WriteMultiple full block"]
  write --> logWrap[LoggingModbusTransport]
  logWrap -->|"Data=前16word truncated"| file["Logs/Comm/comm-*.log"]
```

## 目标行为

- **Information**：每次成功的读写在 `Data=` 中输出**全部**寄存器（`XXXX` 空格分隔），带 `Words=` / 结束地址，便于用编辑器搜索、复制、与读回比对。
- **可读性**：按 16 word 一组插入偏移标记（如 `@0010:`），**不省略任何 word**。
- **快速比对**：附带 `DataSha256=`（对全部 word 的小端 ushort 字节序列做 SHA-256），方便「写出 vs 读回」一眼是否一致。
- **失败**：写失败时尽量带上待写 `Data`/`DataSha256`（现失败路径无载荷）。
- **不做**：不复刻原始 TCP/RTU 帧/CRC；不把物理分块提到 Information（若需要可另在 Debug 加 chunk 行，本迭代不做）。
- HMI 日志页预览仍是尾部窗口——完整内容以 `Logs/Comm` 文件为准（[`LogsViewModel`](src/AutoScrew.Hmi/ViewModels/LogsViewModel.cs) 不改）。

单条参数写约 349×5 ≈ 2KB 字符，顺序约 3KB，对日滚动 Comm 文件可接受；磁盘提醒已在 readme，保留。

## 实现步骤

1. **改 [`LoggingModbusTransport`](src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs)**
   - 将 `Summarize` 改为完整格式化（或抽 `ModbusPayloadFormatter` 便于单测）：`Words={n} End=0x{addr+n-1:X4} Sha256={hex} Data=[@0000: w0 … | @0010: …]`。
   - 成功日志模板增加 `Words` / `End` / `DataSha256` 字段（或全部塞进 `Data=` 字符串，保持一行可搜）。
   - 失败的 `ExecuteAsync` catch：若本次操作已有待写/可读 payload 工厂，一并打 Warning（写路径用现有 `data` Func）。

2. **改测试 [`LoggingModbusTransportTests`](tests/UDL.Delta.IemdSd.Tests/LoggingModbusTransportTests.cs)**
   - 删除/改写 `Summary_IsBoundedAndDoesNotMutatePayload`：断言 **17+ word 不再含 `truncated`**，且第 17 个 word `0011` 出现在消息中；payload 仍不被改写。
   - 新增：大块（如 349）日志含全部末尾 word、含 `Sha256=`、含 `@` 偏移；空数组/`null` 行为不变。

3. **文档**：更新 [`readme.md`](readme.md)「控制器通信日志」——去掉「最多 16 个寄存器截断」；说明完整 hex + SHA256 + 偏移标记；提醒大块写会产生较长行、仍按权限管理。

4. **验证**：`dotnet test tests/UDL.Delta.IemdSd.Tests --filter LoggingModbusTransport`；真机/仿真写一次参数后打开 `Logs/Comm/comm-*.log`，确认 `#100`/`WriteMultiple` 行可搜到完整尾部寄存器。

## 关键文件

- [`src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs`](src/UDL.Delta.IemdSd/Modbus/LoggingModbusTransport.cs)
- [`tests/UDL.Delta.IemdSd.Tests/LoggingModbusTransportTests.cs`](tests/UDL.Delta.IemdSd.Tests/LoggingModbusTransportTests.cs)
- [`readme.md`](readme.md)（通讯日志小节）
