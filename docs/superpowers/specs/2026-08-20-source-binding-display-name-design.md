# 设备列表 / 来源绑定显示：ID + 名称（设备读名）

**日期**：2026-08-20  
**状态**：实现中（范围已扩大）  
**范围**：
1. HMI「拧紧参数」设备参数列表 ComboBox  
2. HMI「拧紧顺序」设备顺序列表 ComboBox  
3. HMI「拧紧来源」选择弹窗及选定后「名称」列 / 绑定按钮文案  

**非目标**：不下发字段变更；不改本地预设列表既有 `001 · 名称`；不引入新的设备协议命令

---

## 1. 背景与问题

Delta 控制器 HMI 设备列表展示为 `ID 空格名称`（如 `1 CANSHU`）。  
上位机原先：

- 参数/顺序设备列表：`ForDeviceSlot` → `001 1` 占位  
- 来源选择弹窗：仅 `ListDevice*IdsAsync` + `ForDeviceSlot`

无法与设备真实名称对应。

**已确认决策**：从设备逐条读取名称；服务层 `ListDevice*EntriesAsync`，并发度 **2**。

---

## 2. 目标

1. 三处设备侧展示统一为：**`{ID} {名称}`**（空格分隔，ID **不**补零），例：`1 CANSHU`。  
2. 名称来自设备参数/顺序内容块中的 `Name`（Trim）。  
3. 单条读取失败时该条降级为仅 ID，不阻断整表。  
4. 打开来源弹窗 / 刷新设备列表前有「正在从设备读取名称…」状态提示。

---

## 3. 架构与组件

```
RefreshDeviceList / OpenBindingPicker
        │
        ├─► ListDeviceParameterEntriesAsync
        │         ListDeviceParameterIdsAsync → 并发 ReadFromDeviceAsync → Name
        │
        └─► ListDeviceSequenceEntriesAsync
                  ListDeviceSequenceIdsAsync → 并发 ReadFromDeviceAsync → Name
        │
        ▼
DisplayText = DeviceListDisplayFormat.Format(id, name)
```

### 3.1 Application

- `DeviceListDisplayFormat.Format(int id, string? name)`
- `ControllerDeviceParameterEntry` / `ControllerDeviceSequenceEntry`
- `ListDeviceParameterEntriesAsync` / `ListDeviceSequenceEntriesAsync`

语义：并发度 2；失败 Name=`""`；不写本地预设。

### 3.2 HMI

- `ControllerParameterListItem.ForDeviceEntry` / `ControllerSequenceListItem.ForDeviceEntry`
- `RefreshDeviceListCoreAsync`（参数页、顺序页）改用 entries  
- `OpenBindingPickerAsync` 改用 entries；`ApplyFromEntry` 用 `Format` 重算绑定文案  

---

## 4. 测试

- 单元：`DeviceListDisplayFormatTests`（有名 / 空名 / 空白名）  
- 手工：真机刷新三页设备列表；来源选定后按钮文案一致  

---

## 5. Spec 自检

- [x] 范围含参数/顺序设备列表与来源弹窗  
- [x] 本地预设 `001 ·` 格式保留  
- [x] 失败降级已写明  
- [x] 并发度定为 2  
