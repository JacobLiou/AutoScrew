---
name: Export Param Seq Templates
overview: 在拧紧参数页/拧紧顺序页增加「导出为工艺模板」：当前编辑器内容分别导出为可再导入工艺库的 TXT 工艺卡与 Excel 顺序表，支撑「设备调教 → 导出模板」闭环。
todos:
  - id: txt-writer
    content: 实现 ProcessCardTxtWriter + Parse/Write 往返单测
    status: completed
  - id: excel-writer
    content: 实现 SequenceExcelWriter + Write/Parse 往返单测
    status: completed
  - id: hmi-param-export
    content: 参数页：导出表单（螺钉PN+槽位）+ ExportProcessCardTxt + 按钮/字符串
    status: completed
  - id: hmi-seq-export
    content: 顺序页：导出表单（默认螺钉PN+位置写法）+ ExportSequenceExcel + 按钮/字符串
    status: completed
  - id: docs
    content: 更新 PROCESS_LIBRARY.md 导出说明与槽位→设备ID
    status: completed
isProject: false
---

# 拧紧参数/顺序导出为工艺模板

## 背景与目标

技术员在设备上调出最佳参数后，需要把结果落成可再上传工艺库的模板：

| 对象 | 导出格式 | 再导入入口 |
|------|----------|------------|
| 拧紧参数 | TXT 工艺卡（对齐 [ProcessCardTxtParser](src/AutoScrew.Infrastructure/ProcessLibrary/ProcessCardTxtParser.cs) / 样例 `tests/.../1830330479_00.txt`） | 工艺库「上传工艺卡」 |
| 拧紧顺序 | Excel（对齐 [SequenceExcelParser](src/AutoScrew.Infrastructure/ProcessLibrary/SequenceExcelParser.cs) 表头） | 工艺库「上传顺序 Excel」 |

**默认决策（按现场「先设备调教再导出」）**：数据取自**当前编辑器工作副本**（通常已 `#150`/`#250` 导入）；按钮放在**拧紧参数页 / 拧紧顺序页**工具栏，紧挨「从设备导入」。不在本次做工艺库批量导出。

```mermaid
flowchart LR
  Device[设备调教] -->|Import_150_250| Editor[参数或顺序编辑器]
  Editor --> Form[导出确认表单]
  Form -->|Export_TXT_Excel| File[模板文件]
  File -->|工艺库上传| Library[process_PN]
```

## 导出前表单：哪些要问用户

设备侧只有**设备参数 ID / 顺序步骤(ParameterId, Quantity, BitId)**，工艺模板还需要**螺钉 PN、槽位、位置**等工艺身份。这些无法从电批可靠还原，必须在导出前用小表单确认（类似 Web Form），再 `SaveFileDialog` 选路径。

### 原则

| 类别 | 处理 |
|------|------|
| 扭矩/角度/阶段/数量/批头等工艺数值 | **不问**，全部取自当前编辑器 |
| 工艺身份（螺钉 PN、槽位） | **要问**（可预填，可改） |
| 产品 PN | **不问**（不在 TXT/Excel 文件身份里；上传工艺库时由工艺库页填写） |
| 存盘路径 | `SaveFileDialog`，不算业务表单 |

### A. 拧紧参数 → TXT：导出确认表单

| 字段 | 必填 | 预填 | 说明 |
|------|------|------|------|
| **螺钉 PN** | 是 | 编辑器 `Name`（控制器 ASCII 名，常不完整） | 写入 `参数：{螺钉PN}-{槽位}`；用户常需改成真实料号如 `1830330479` |
| **槽位** | 是 | `ParameterId - 1`（设备 ID=1 → `00`） | 可改：设备上可能用临时 ID 调教，导出时要落成产品槽位 `00/01…` |
| 设备参数 ID | 只读 | `槽位 + 1` | 展示映射，防填错；不单独输入 |

校验：螺钉 PN 非空（建议 ASCII 字母数字）；槽位 `0–499`；默认文件名 `{螺钉PN}_{槽位:D2}.txt`。

### B. 拧紧顺序 → Excel：导出确认表单

顺序步骤在设备/编辑器里**没有**「螺钉 PN」「位置」「备注」，Excel 却要求这些列。

| 字段 | 必填 | 预填 | 说明 |
|------|------|------|------|
| **默认螺钉 PN** | 是 | 空或上次导出记忆 | 某步无法从本地参数预设 `Name` 解析时使用；`拧紧参数`=`{螺钉PN}-{槽位:D2}`，槽位=`步骤.ParameterId-1` |
| **位置列写法** | 否 | `步骤{序号}` | 三选一：`步骤{n}` / 留空 / 自定义前缀+序号；现场可再在 Excel 改成「pump1锁附」等 |

逐步回填（表单外、自动）：① 按 `ParameterId` 查本地参数预设 Name → ② 否则用默认螺钉 PN → ③ 数量/批头/顺序号来自编辑器，不问。备注列导出为空。

### C. 明确不进导出表单

- 产品 PN、是否立刻写入工艺库  
- 扭矩/阶段明细、导航坐标（已在页上编完）  
- 批量「按产品导出向导」（本次不做）

### D. UI 形态

- 小型对话框：`ExportProcessCardDialog`（2 输入 + 只读设备 ID）、`ExportSequenceExcelDialog`（默认螺钉 PN + 位置写法）  
- 确认后再 `SaveFileDialog`；文案进 `Strings.*.xaml`

## 实现要点

### 1. `ProcessCardTxtWriter`（新建）

路径建议：[`src/AutoScrew.Infrastructure/ProcessLibrary/ProcessCardTxtWriter.cs`](src/AutoScrew.Infrastructure/ProcessLibrary/ProcessCardTxtWriter.cs)

- 输入：`TighteningParameterTemplate` + **表单给出的** `screwPn` + `slotId`（不以编辑器 ParameterId 强行覆盖用户改过的槽位；写出前将 `template.ParameterId = slotId + 1`）
- 输出：UTF-8 文本，键值 + 行尾 `<说明>`（与样例同风格，解析器会忽略说明）
- 身份行：`参数：{screwPn}-{slot:D2}`（可另写 `参数ID：{screwPn}` 作可读兼容）
- 扭矩：mN·m → **lbf.in**（`TorqueUnitConverter`）；时间/延时按解析反向写出
- 阶段：`1.启动`…及拧松两段

单测：Parse → Write → Parse 往返。

### 2. `SequenceExcelWriter`（新建）

路径建议：[`src/AutoScrew.Infrastructure/ProcessLibrary/SequenceExcelWriter.cs`](src/AutoScrew.Infrastructure/ProcessLibrary/SequenceExcelWriter.cs)

- ClosedXML 表头：`拧紧顺序 | 位置 | 螺钉PN | 拧紧参数 | 数量 | 批头`（可选备注空列）
- 行数据由 VM 按「表单 + 本地预设回填」组装后再写入
- 单测：Write → `SequenceExcelParser.Parse` 成功

### 3. Writer 调用方式

与现有 Parser 一致：Infrastructure 静态 Writer；HMI 直接调用（不必强行扩 `IProcessLibraryService`，除非后续工艺库也要导出）。

### 4. HMI

**拧紧参数** — VM + `ParameterEditorView` + **`ExportProcessCardDialog`**

- `ExportProcessCardTxtCommand` → 先开表单（螺钉 PN、槽位）→ 再 `SaveFileDialog`
- 资源：`S.ControllerParam.ExportProcessCard` 及表单标签键

**拧紧顺序** — VM + `SequenceEditorView` + **`ExportSequenceExcelDialog`**

- `ExportSequenceExcelCommand` → 表单（默认螺钉 PN、位置写法）→ `SaveFileDialog`
- 资源：`S.ControllerSeq.ExportSequenceExcel` 及表单标签键

审计：`Configuration.ParamExportProcessCard` / `Configuration.SeqExportExcel`。

### 5. 文档

- 更新 [`doc/PROCESS_LIBRARY.md`](doc/PROCESS_LIBRARY.md)：增加「从设备导出模板」小节；顺带修正槽位→设备 ID 为 **槽位+1**（与现行代码一致，文档仍写 `00→0` 已过时）
- [`doc/TODO.md`](doc/TODO.md) 记一条完成项（可选）

## 明确不做

- 工艺库页批量导出、JSON 导出按钮补齐（已有未绑 JSON API，本次不混做）
- 导出时自动写回工艺库目录（用户先落文件，再按现有上传流程入库）
- 改 PRD KPI / 真机协议

## 验收手测

1. 参数页：从设备 #150 导入 → 导出 TXT → 工艺库上传同卡 → 再「从工艺库导入」字段一致  
2. 顺序页：#250 导入 → 导出 xlsx → 工艺库上传 Excel → 步骤 ParameterId=槽+1、数量/批头一致  
3. `ParameterId=1` 导出槽位 `00`；无效 ID 有明确错误提示  
