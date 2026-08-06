# 多面产品模板契约（草案）

| 版本 | 日期 | 状态 |
|------|------|------|
| 0.1 | 2026-05-21 | **草案**：待业务 / MES / 现场评审后定稿 |

本文档定义「一个产品 = N 个可配置平面」的模板 JSON 契约、与现有单面模板（`schemaVersion: 1`）的兼容策略，以及追溯位号规则。**实现变更前**须与 [DATA_AND_TRACE.md](DATA_AND_TRACE.md) 同步；MES 字段以公司 IT 最终规范为准。

**关联文档**：[MULTI_SURFACE_UI_WIREFRAME.md](MULTI_SURFACE_UI_WIREFRAME.md)（HMI 交互线框）

---

## 1. 背景与问题

### 1.1 现状（schemaVersion 1）

当前 HMI 与 Application 层使用**单平面**模板：

- 根对象：`boardWidth`、`boardHeight`、一张产品底图、`markers[]`
- 扫码后加载**一个** JSON 文件，在**一张**引导图上逐钉作业
- 代码锚点：`TemplateDocument` / `TemplateLayoutDto`（`src/AutoScrew.Hmi/Models`、`src/AutoScrew.Application/Templates`）

### 1.2 业务缺口

- 整机常有 **6 个正交面**或**任意 N 个可命名平面**（非固定 6）
- 每面独立底图、画板尺寸、螺钉位布局；整机需**按面推进**或**按面返修**
- 预存任务（≥50 组）与 SN→PN 下发应对应**产品模板包**，而非多个散落单文件

---

## 2. 术语

| 术语 | 含义 |
|------|------|
| **产品模板包** | 一个 JSON 文件（或同目录资源文件夹），描述某 PN 的全部平面与螺钉布局 |
| **面（Surface）** | 一个 2D 引导平面：独立底图、画板尺寸、螺钉标注列表 |
| **面 ID（surfaceId）** | 稳定标识，用于追溯与 MES；**不因显示名修改而改变** |
| **面内位号（localIndex）** | 该面 `markers` 内的 `index`（从 1 起） |
| **全局位号（globalIndex）** | 跨面唯一序号，由 `assemblySequence` 规则计算或显式写入 |

---

## 3. schemaVersion 2 根结构

### 3.1 文件命名建议

- 单文件：`{PartNumber}.product-template.json`（示例：`K1927020.product-template.json`）
- 资源目录（可选）：与 JSON 同级的 `images/`、`surfaces/` 子目录

### 3.2 JSON 根字段

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `schemaVersion` | int | 是 | 固定 `2` |
| `productId` | string | 是 | 产品 / PN，与 MES `partNumber` 对齐 |
| `displayName` | string | 否 | 界面显示名 |
| `revision` | string | 否 | 模板修订号（工艺变更留痕） |
| `surfaceCount` | int | 是 | 面数量 N；须与 `surfaces.length` 一致 |
| `surfaces` | array | 是 | 面定义列表，见 §4 |
| `assemblySequence` | string | 是 | 整机作业顺序策略，见 §5 |
| `metadata` | object | 否 | 创建人、时间、备注等 |

### 3.3 示例（节选）

```json
{
  "schemaVersion": 2,
  "productId": "K1927020",
  "displayName": "示例产品",
  "revision": "2026-05-21",
  "surfaceCount": 6,
  "assemblySequence": "surfaceOrderThenLocalIndex",
  "surfaces": [
    {
      "surfaceId": "TOP",
      "name": "顶面",
      "order": 1,
      "boardWidth": 640,
      "boardHeight": 480,
      "circleDiameter": 26,
      "productImageRelativePath": "images/K1927020_top.png",
      "markers": [
        { "index": 1, "centerX": 160, "centerY": 120, "screwTypeId": 3, "partNo": "SCR-M3-8" }
      ]
    }
  ]
}
```

---

## 4. 面（Surface）对象

每个 `surfaces[]` 元素在 v1 单面模板上扩展，字段与现有 `TemplateDocument` **对齐**，便于复用画板编辑器。

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `surfaceId` | string | 是 | 稳定 ID；建议大写英文或 `S1`…`SN`，禁止仅依赖 `name` |
| `name` | string | 是 | 界面显示（如「顶面」「侧面 A」） |
| `order` | int | 是 | 作业默认顺序（1..N，可重复则须业务明确） |
| `boardWidth` | number | 是 | 画板宽（像素或与 v1 一致单位） |
| `boardHeight` | number | 是 | 画板高 |
| `circleDiameter` | number | 否 | 默认圈径，默认 28 |
| `productImageRelativePath` | string | 否 | 相对 JSON 目录的底图路径 |
| `productImageAbsolutePath` | string | 否 | 跨盘符回退（与 v1 相同语义） |
| `productImageOpacity` | number | 否 | 0..1 |
| `markers` | array | 是 | 螺钉标注，见 §4.1 |
| `enabled` | bool | 否 | 默认 `true`；`false` 时该面跳过（可选工艺） |

### 4.1 Marker 对象（与 v1 兼容）

| 字段 | 类型 | 说明 |
|------|------|------|
| `index` | int | **面内**位号，从 1 起，面内唯一 |
| `centerX` / `centerY` | number | 圆心坐标 |
| `screwTypeId` | int | 对应 `{DataDirectory}/screw-types.json`（见 [samples/screw-types.json](samples/screw-types.json)；运行时由 `ScrewTypeCatalog` 加载，无界面编辑） |
| `circleDiameter` | number | 可选，覆盖面级默认 |
| `partNo` | string | 可选，螺钉料号（与 MES 工艺对齐） |
| `globalIndex` | int | **可选**；若省略则由运行时按 §5 计算 |

---

## 5. 作业顺序（assemblySequence）

| 值 | 行为 |
|----|------|
| `surfaceOrderThenLocalIndex` | **默认**：按 `surfaces[].order` 升序；同面内按 `markers[].index` 升序 |
| `explicitGlobalIndex` | 每个 marker 必须带 `globalIndex`；运行时仅认全局序 |
| `surfaceOrderFreeWithinSurface` | 按面顺序锁定；**面内**允许任意顺序（返修场景需权限） |

**待业务确认**（评审勾选）：

- [ ] 是否允许操作员**跳面**（未完成作业面）
- [ ] NG 返修是否只允许**当前面 / 当前钉**
- [ ] 翻面是否需**人工确认**或**外部信号**（传感器 / PLC）

---

## 6. 与 v1 的兼容与迁移

### 6.1 读取规则

| 文件 | 处理方式 |
|------|----------|
| `schemaVersion == 1` 或无字段 | 视为 **N=1**：合成 `surfaces[0]`，`surfaceId="DEFAULT"`，`order=1` |
| `schemaVersion == 2` | 按本文档解析；校验 `surfaceCount == surfaces.length` |

### 6.2 升级工具（规划）

- HMI「模板编辑」提供 **导入 v1 → 另存 v2**（单面包装进 `surfaces[0]`）
- 批量升级脚本（可选）：扫描 `TemplateDirectory` 下旧 JSON

### 6.3 代码影响面（实现时对照）

| 模块 | 变更要点 |
|------|----------|
| `TemplateDocument` / `TemplateLayoutDto` | 新增 v2 类型或并列 DTO；Loader 分支 |
| `ITemplateLayoutLoader` | 返回「当前面」或「全产品展开」结构 |
| `OperatorSessionController` | 会话状态：`CurrentSurfaceId`、面完成、全局进度 |
| `RecipeBundle` / `IMesClient` | `TemplateJsonPath` 指向 v2 包；螺钉工艺表是否按面分段待 IT 定 |
| 本地曲线 / DB | 文件名与 `screw_details` 增加 `surface_id`（见 DATA_AND_TRACE 增补） |

---

## 7. MES 与 Recipe 对接（占位）

在 IT 定稿前，建议最小集：

**下发（GetRecipe）**

| 字段 | 说明 |
|------|------|
| `partNumber` | PN |
| `templateJsonPath` 或 `templateJsonUrl` | v2 产品包路径 / URL |
| `screws[]` | 可选；若 MES 仍下发全局工艺表，需含 `globalIndex` 或 `(surfaceId, localIndex)` |

**回传（UploadResult）**

| 字段 | 说明 |
|------|------|
| `serialNumber` | SN |
| `surfaceId` | 完成或 NG 所在面 |
| `localIndex` | 面内位号 |
| `globalIndex` | 推荐始终上报，便于与 v1 报表兼容 |
| `result` / `torqueFinal` / `angleFinal` / `curvePath` | 与现有 `ScrewRecipeDto` 对齐 |

---

## 8. 本地存储与曲线文件（建议）

在 [DATA_AND_TRACE.md](DATA_AND_TRACE.md) 定稿前，草案命名：

```
%LocalAppData%\AutoScrew\work\{SN}\
├── lock_log_{timestamp}.json
├── surfaces\
│   ├── {surfaceId}\
│   │   ├── torque_curve_{localIndex}_{timestamp}.csv
│   │   └── surface_summary.json
└── product_template_snapshot.json   # 可选：作业开始时拷贝的模板包
```

曲线文件若需与 v1 并存，可保留 `torque_curve_{globalIndex}_{timestamp}.csv` 作为别名。

---

## 9. 校验规则（保存时）

1. `surfaceCount === surfaces.length`
2. 每个 `surfaceId` 在产品内唯一
3. 每个面内 `markers[].index` 唯一且 ≥ 1
4. `assemblySequence === explicitGlobalIndex` 时，所有 marker 须有唯一 `globalIndex`
5. 底图路径：相对路径优先；至少一种路径可解析为存在文件（警告 / 错误级别可配置）
6. `order` 建议 1..N 连续（非连续时须文档说明）

---

## 10. 开放问题（评审清单）

| # | 问题 | 建议默认 | 决策人 |
|---|------|----------|--------|
| 1 | N 是否必须固定 6？ | 任意 N ≥ 1，默认常用 6 | 业务 |
| 2 | 全局位号由谁定义？ | 运行时按序计算 + 可选显式覆盖 | 工艺 / MES |
| 3 | 50 组任务存什么？ | 50 个 **product-template.json** | 工艺 |
| 4 | 面未完成能否关单？ | 不允许（全部面 OK 才 Complete） | 质量 |
| 5 | v1 文件何时废弃？ | 兼容读取保留 ≥2 个版本周期 | 项目组 |

---

## 11. 修订记录

| 版本 | 日期 | 说明 |
|------|------|------|
| 0.1 | 2026-05-21 | 初稿：v2 契约、兼容 v1、追溯与 MES 占位 |
