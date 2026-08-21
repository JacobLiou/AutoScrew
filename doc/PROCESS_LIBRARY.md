# 工艺库（A 层：参数 + 拧紧顺序）

| 版本 | 日期 | 状态 |
|------|------|------|
| 1.6 | 2026-08-21 | TXT 全量键：策略四模式、自创 5–6 段、阶段高级设定；全量样板 AllTemplate* |
| 1.5 | 2026-08-19 | 参数/顺序页从工艺库多选导入；同 PN 覆盖、跨 PN 只新增 |
| 1.4 | 2026-08-17 | 拧紧参数/顺序页导出 TXT·Excel 模板；槽位→设备 ID = 槽位+1 |
| 1.3 | 2026-08-07 | 作业台扫 SN 换产警告 + 覆盖下发；同 PN+版本跳过 |
| 1.2 | 2026-08-06 | 同产品 PN 目录增加 `sequences/` 与按 PN 下发顺序 |
| 1.1 | 2026-08-05 | 适配最终评审 TXT：`参数：螺钉PN-槽位` |
| 1.0 | 2026-08-03 | 落地：产品 PN 目录 + TXT 工艺卡 + 按槽位下发 |

**需求追溯**：[PRD.md](PRD.md) §3.2.4 模板库（A 工艺库）  
**全量样板（推荐填空）**：[samples/AllTemplateParams.txt](samples/AllTemplateParams.txt)、[samples/AllTemplateSequence.xlsx](samples/AllTemplateSequence.xlsx)  
**现场样例**：[samples/1830330479 _00.txt](samples/1830330479%20_00.txt)  
**兼容样例（旧）**：[tests Fixtures/1830330479_00.txt](../tests/AutoScrew.Tests/Fixtures/1830330479_00.txt)

## 1. 业务模型

| 概念 | 含义 |
|------|------|
| **产品 PN** | 拉取/换产主键；一个产品下多颗螺钉参数与多条拧紧顺序 |
| **螺钉 PN** | 工艺卡身份；写入参数 `Name` 便于追溯 |
| **槽位 / ParameterId** | 本地槽位 `00` → **设备 ParameterId = 槽位 + 1**（`00→1`，`01→2`…）；见 `ProcessParameterCode.ToDeviceParameterId` |
| **顺序 / SequenceId** | 设备顺序号；落盘为 `sequences/{id:D2}.json` |

> 台达手册参数 ID **1–500**。现场工艺卡以 `00` 起槽；上位机统一用 **槽位+1** 作为设备 ID（卡内「参数ID」行不作为设备身份）。

## 2. 目录约定

优先写局域网（MES「局域网根路径」或 `AutoScrew:OptionalNetworkArchiveRoot`），否则回落本机 `{DataDirectory}/process`：

```text
{processRoot}/{产品PN}/
  product.json
  screws/
    00.txt
    01.txt
    …
  sequences/
    01.json
    02.json
    …
```

`product.json`：

```json
{
  "schemaVersion": 1,
  "productPn": "YOUR-PRODUCT-PN",
  "updatedUtc": "2026-08-03T01:00:00Z",
  "slots": [
    {
      "slotId": 0,
      "screwPn": "1830330479",
      "fileName": "screws/00.txt",
      "displayName": "1830330479"
    }
  ],
  "sequences": [
    {
      "sequenceId": 1,
      "fileName": "sequences/01.json",
      "displayName": "SEQ-01"
    }
  ]
}
```

兼容：旧清单无 `sequences` 时视为空列表；仍可正常加载 `slots`。

顺序 JSON 与本机/设备顺序预设同构（`ControllerSequencePresetDocument` / `TighteningSequencePackage`）。上传时反序列化校验后按包内 `SequenceId` 落盘为 `sequences/{id:D2}.json`，并更新清单。

局域网账号复用 SN 归档：`PRED-TESTING` + `AutoScrew:LanSharePasswordAes256`。

## 3. TXT 工艺卡（拧紧参数）

格式：键值对 + 行尾 `<说明>`（解析时忽略说明）。以 `#` / `//` 开头的行为注释。

全量键与自创 6 段示例见 [samples/AllTemplateParams.txt](samples/AllTemplateParams.txt)。

### 3.1 身份字段（最终评审）

| TXT | 映射 |
|-----|------|
| `参数：{螺钉PN}-{槽位}` | 例 `1830330479-00` → `ScrewPn=1830330479`，**设备 ParameterId=1**，`Core.Name=1830330479` |
| `参数ID` | **可选**可读字段（常写螺钉 PN）；**不作为**设备参数 ID |

**兼容旧卡**：`参数：00` + `参数ID：1830330479` → 槽位 0 + 螺钉 PN。

### 3.2 策略与阶段标题

| TXT | 映射 |
|-----|------|
| `策略` | `标准` / `加强` / `预定位` / `自创` → `Core.Strategy`；**缺省=标准**（旧卡兼容） |
| `阶段有效：N` | 自创时表示前 N 段意图；实际以段标题序号灌入对应槽 |
| `1.启动` … `4.拧紧` | 槽 0–3（命名四阶段） |
| `5.阶段5` / `6.阶段6` | 槽 4–5（自创） |
| `N.阶段N` | 自创兜底标题 |

写出时：标准写活动前缀段；加强仅写 `4.拧紧`；预定位写启动+旋入；自创按已配置槽写出，并带 `控制模式`。

### 3.3 工艺字段

| TXT | 映射 |
|-----|------|
| 最大/最小总角度 | `MaxAngleDeg` / `MinAngleDeg` |
| 最大拧紧时间（秒） | `MaxTighteningTimeTenthSec`（×10） |
| 拧紧启动延时（×0.01） | `TighteningStartDelayCentiSec` |
| 拧松同类 | `MaxLoosenTimeTenthSec` / `LoosenStartDelayCentiSec` / `MaxLoosenAngleDeg` |
| 进阶 ON/OFF、补偿 ID 等 | 对应 `TighteningParameterCore` 字段 |
| 曲线取样起始扭矩（mNm） | 已是 mN·m，直接写入 |
| 各段扭矩（lbf.in） | `TorqueUnitConverter` → mN·m |
| 拧松两段角度/速度、生产履历、最小扭矩 | `TighteningLoosenCore` |
| 第一段/第二段内 `加速时间（ms）` | `Loosen.Stage1AccelMs` / `Stage2AccelMs` |

控制模式：

- 可选显式键 `控制模式：角度|扭矩|扭矩率|夹紧扭矩|夹紧角度`（自创导出必写）
- 缺省时启发式（与工艺卡「n 选 1」一致）：
  - **1.启动**：固定角度  
  - **2.旋入**：扭矩率>0 → 扭矩率；否则有角度 → 角度；否则扭矩  
  - **3.预紧**：扭矩率>0 → 扭矩率；夹紧扭矩>0 → 夹紧扭矩（旧卡）；否则扭矩  
  - **4.拧紧及以后**：按夹紧角度 / 夹紧扭矩 / 扭矩率 / 扭矩 / 角度优先级  

扭矩/角度判断：

- 显式 `OFF` → 对应上下限写 0  
- 显式 `ON`，或**键缺失且上下限有正值** → 写入上下限（最终模板第 4 段无「扭矩判断」行时仍生效）

### 3.4 阶段高级设定（可选；导出时非默认才写出）

| TXT | 映射 |
|-----|------|
| `扭矩率角度间隔（×0.1°）`（段内） | `TorqueRateAngleIntervalTenthDeg` |
| `加速时间（ms）` / `减速时间（ms）` | `AccelTimeMs` / `DecelTimeMs` |
| `最大/最小运行时间（×0.01s）` | `Max/MinRunTimeCentiSec` |
| `补偿扭矩` + `补偿扭矩角度百分比` | `CompTorqueEnabled` / `CompTorqueAnglePercent` |
| `暂停时间（ms）` | `PauseTimeMs` |
| `最小夹紧扭矩（lbf.in）` / `最小夹紧角度（°）` | `MinClamp*` |
| `一段扭矩` / `一段暂停` / `二段加速` / `最终速度` | `Segment*` / `FinalSpeedRpm` |

## 4. 拧紧顺序（JSON / Excel）

| 操作 | 行为 |
|------|------|
| 上传 JSON | 校验 → `sequences/{SequenceId:D2}.json` → 更新 `product.json.sequences` |
| 上传 Excel | 表头：`拧紧顺序\|位置\|螺钉PN\|拧紧参数\|数量\|批头\|备注`；`拧紧参数`=`螺钉PN-槽位`；步骤设备 ID=槽位+1。全量样板见 [samples/AllTemplateSequence.xlsx](samples/AllTemplateSequence.xlsx) |
| 删除 | 仅删工艺库文件与清单项，不删设备顺序 |
| 按 PN 下发 | 逐条覆盖写入设备（`WriteToDevice`）并 `SaveLocalPreset`；遇错停止；**不**先全删设备顺序 |

> Excel **不**承载导航坐标 / 定位臂 / 工具号；那些字段走顺序 JSON 或设备编辑器。`位置`/`备注` 为人读元数据，上传进包时不落设备步骤。

## 4.1 从设备导出模板（调教闭环）

技术员在设备调出最佳参数后：

1. **拧紧参数页**：`#150` 导入编辑器 → **导出工艺卡 TXT**（表单确认螺钉 PN、槽位）→ 保存 `{螺钉PN}_{槽位:D2}.txt`  
2. **拧紧顺序页**：`#250` 导入编辑器 → **导出顺序 Excel**（表单确认默认螺钉 PN、位置列写法）→ 保存 `sequence-{id}.xlsx`  
3. 再到**工艺库**页选择产品 PN，上传上述 TXT / Excel  

实现：`ProcessCardTxtWriter` / `SequenceExcelWriter`（与 Parser 往返兼容）。产品 PN **不在**导出表单中询问。

## 5. HMI 操作流

1. 输入 **产品 PN** → 加载参数槽 + 顺序列表（Tab：**参数槽位** | **拧紧顺序**）  
2. **上传工艺卡 / 上传顺序 JSON / 上传顺序 Excel** → 写入对应子目录并更新清单  
3. **拧紧参数页「从工艺库导入」**：多选槽位或「导入该 PN 全部」→ **写入本机预设**并打开第一条。同产品 PN 按槽位覆盖；首选设备 ID 已被**其他产品**占用则分配新 ID（1–500），绝不覆盖。状态栏给出「槽 00 → 本机 ID 4（未覆盖其他产品的 ID 1）」。导入**不**自动写设备。  
4. **拧紧参数页「导出工艺卡 TXT」**：确认身份 → 写出可再上传的 TXT（不写工艺库目录）  
5. **拧紧顺序页「从工艺库导入」**：多选或「导入该 PN 全部」→ 先按上款规则补齐引用的参数槽，再把步骤 `ParameterId` 映射到本机 ID 后写入本机顺序预设。导入**不**自动写设备。  
6. **拧紧顺序页「导出顺序 Excel」**：确认默认螺钉 PN → 写出可再上传的 xlsx  
7. **工艺库「下发参数 / 下发顺序到设备」**：按清单覆盖写入设备（仍用库槽位+1），并回写本机预设  

无 `SourceProductPn` 的旧本机文件视为已占用，后导入的产品走新增。生产换产下发仍按库槽位+1 写控制器，与本机重分配无关。  

## 6. 与 B/C 层关系

```text
工艺库(A) --按产品PN--> 设备参数槽 / 顺序 --可选--> 设备配置包(B)
产品模板(C) Templates/{产品PN}/  钉位引导，与 A 共用产品 PN
```

## 7. 扫 SN 换产（作业台）

作业台提交 SN → MES 得到产品 PN 后，与工位已下发状态比较：

| 条件 | 行为 |
|------|------|
| 同 PN 且 `updatedUtc` 相同 | **跳过**写设备，直接进作业 |
| 无工位状态 / PN 变更 / 工艺版本变更 / 库中无产品 | **强制警告确认** → 确认后覆盖下发参数+顺序 → 成功才进作业 |
| 取消警告或下发失败 | 中止本次 SN，可重扫；**不**更新工位状态 |

- 工位状态文件：`{DataDirectory}/station-process-state.json`（`productPn` + `updatedUtc` + `deployedUtc`）
- 操作员与技术员同一扫码路径，无角色豁免
- 写入策略：覆盖下发（不先全删设备）

## 8. 明确后续（本期不做）

| 项 | 说明 |
|----|------|
| 显式「换产」工具栏按钮 | 扫到新 PN 即触发确认 |
| 模板→顺序 | 螺钉模板尚无 ParameterId 映射 |
| 设备全删顺序 | 仅覆盖写入 |
| 拧紧来源 | 本期不管 |
| 工艺库页批量导出 | 先用参数/顺序页单条导出 |
