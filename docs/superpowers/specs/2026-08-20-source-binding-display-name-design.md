# 拧紧来源绑定显示：ID + 名称（设备读名）

**日期**：2026-08-20  
**状态**：待用户审阅  
**范围**：HMI「拧紧来源」选择弹窗及选定后「名称」列显示  
**非目标**：不下发字段变更；不改参数/顺序编辑页既有 `001 · 名称` 展示；不引入新的设备协议命令

---

## 1. 背景与问题

Delta 控制器 HMI「拧紧来源」名称列展示工艺名（如 `YUDINGWEI`）。  
上位机打开「选择拧紧来源」时，目前仅调用：

- `ListDeviceParameterIdsAsync` / `ListDeviceSequenceIdsAsync`
- `ControllerParameterListItem.ForDeviceSlot` / `ControllerSequenceListItem.ForDeviceSlot`

因此弹窗与绑定按钮侧多半只能看到 ID（或 `001 1` 这类占位），无法与设备上的真实名称对应，不便区分。

**已确认决策**：从设备逐条读取名称（方案 B）；服务层提供带名称的设备列表 API，弹窗侧有限并发（方案 2）。

---

## 2. 目标

1. 弹窗列表与选定后「名称」按钮文本统一为：**`{ID} {名称}`**（空格分隔，ID **不**补零），例：`1 YUDINGWEI`。
2. 名称来自设备参数/顺序内容块中的 `Name` 字段（与 Delta 一致）。
3. 单条读取失败时该条降级为仅 ID，不阻断整表选择。
4. 打开弹窗前有简短「正在从设备读取名称…」类状态提示（Status / Snackbar）。

---

## 3. 架构与组件

```
ControllerSourceViewModel.OpenBindingPickerAsync
        │
        ├─► IControllerParameterPresetService.ListDeviceParameterEntriesAsync
        │         ListDeviceParameterIdsAsync → 有限并发 ReadFromDeviceAsync → Name
        │
        └─► IControllerSequencePresetService.ListDeviceSequenceEntriesAsync
                  ListDeviceSequenceIdsAsync → 有限并发 ReadFromDeviceAsync → Name
        │
        ▼
SourceBindingPickerDialog（列表 DisplayText = FormatIdName）
        │
        ▼
ControllerSourceBindingRowViewModel.ApplyPickerSelection（BindingDisplayText 同格式）
```

### 3.1 Application 契约

新增轻量摘要（可复用或并列于现有 `*PresetSummary`）：

| 类型 | 字段 |
|------|------|
| `ControllerDeviceParameterEntry` | `ParameterId`, `Name` |
| `ControllerDeviceSequenceEntry` | `SequenceId`, `Name`（可选附带 `StepCount`/`BitId` 若读包时已有，便于现有 carry；**非必须**，carry 仍可走现有 `ResolveCarryFromDeviceAsync`） |

接口新增：

- `Task<IReadOnlyList<ControllerDeviceParameterEntry>> ListDeviceParameterEntriesAsync(...)`
- `Task<IReadOnlyList<ControllerDeviceSequenceEntry>> ListDeviceSequenceEntriesAsync(...)`

语义：

1. 先列已配置 ID（现有 `#160` / `#260` 逻辑不变）。
2. 对每个 ID 调用现有 `ReadFromDeviceAsync`，取 `Core.Name`（Trim）。
3. **有限并发**：默认并发度 **2**（可内部 `const`，本期不做成用户配置）。
4. 单条异常：记录 Warning 日志，该条 `Name = ""`，仍出现在列表中。
5. **不**因列表读名而写入本地预设（只读展示；避免误改本地库）。

### 3.2 显示格式

统一辅助方法（建议放在 HMI 或 Application 静态小工具，一处定义）：

```text
FormatSourceBindingDisplay(id, name):
  name 非空 → $"{id} {name}"
  否则     → $"{id}"
```

- ID：十进制、不补零（`1` 而非 `001`）。
- 弹窗左侧 `IdText` 可继续显示纯数字；右侧 / 选定后文本使用上述格式。
- `ControllerParameterListItem` / `ControllerSequenceListItem` 构造时传入显式 `displayText: Format...`，避免落入现有 `D3 ·` 默认格式。

### 3.3 HMI

- `OpenBindingPickerAsync`：改为调用 `ListDevice*EntriesAsync`，映射为带 `displayText` 的 list item；打开前设置 Status + Caution/Info Snackbar「正在从设备读取名称…」。
- `SourceBindingPickerDialog`：继续绑定 `DisplayText`；无需改布局（除非后续要做进度条，本期不做）。
- 选定后 `ApplyPickerSelection` 使用弹窗行的 `DisplayText`，与列表一致。
- `EnsureTargetInCatalog` / 本地目录刷新路径：若仍用 `ForDeviceSlot`，**不在本期强制**改为读设备名；仅保证「刚从弹窗选定」的显示正确。若选定后立刻 `RefreshCatalogsAsync` 用本地预设覆盖，本地有名则仍可用本地 `DisplayText`（可含 `D3 ·`）；为避免闪烁不一致，选定后优先保留弹窗写入的 `BindingDisplayText`，刷新时仅在目录命中同 ID 时用 **FormatSourceBindingDisplay(id, catalog.Name)** 重算（推荐，保证格式统一）。

### 3.4 错误与空列表

| 情况 | 行为 |
|------|------|
| 设备不可用 | 保持现有：提示先连接 |
| ID 列表为空 | 保持现有：设备列表为空提示 |
| 全部条目 Name 读失败 | 列表仍为纯 ID，可选择 |
| 部分失败 | 有名显示 `ID Name`，无名仅 `ID` |

---

## 4. 测试

- **单元**：`FormatSourceBindingDisplay`（有名 / 空名 / 空白名）。
- **服务（可选，Mock 客户端）**：entries API 在部分 `ReadFromDevice` 失败时仍返回全量 ID，失败项 Name 为空。
- **手工**：真机打开拧紧来源选择 → 参数 Tab / 顺序 Tab 可见 `ID 名称`；确认后名称列同文案；下发绑定仍为原 TargetId。

---

## 5. 风险与缓解

| 风险 | 缓解 |
|------|------|
| 条目多时打开变慢 | 并发 2；Status 提示；后续可再优化为「仅名称」轻量读（本期不做） |
| Modbus 忙冲突 | 复用现有单客户端服务路径；并发上限保守 |
| 名称含空格 | 仍用首个空格分隔语义上「ID + 剩余为名」；显示整串即可，不做解析 |

---

## 6. Spec 自检

- [x] 无 TBD/占位未决项（并发度已定为 2）
- [x] 与「只改显示、不改协议下发」一致
- [x] 范围不含参数/顺序编辑页全局改格式
- [x] 失败降级与空名行为已写明
- [x] 用户规则：本设计文档**不自动 commit**，待用户明确要求再提交
