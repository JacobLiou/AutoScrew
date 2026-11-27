# 工艺卡交换格式迁移计划（短期止血 → 中期 JSON）

| 版本 | 日期 | 状态 |
|------|------|------|
| 0.1 | 2026-11-25 | 草案：先文档后实现；对齐现场 OK/NG 样例与业务反馈 |

**权威现况**：[PROCESS_LIBRARY.md](PROCESS_LIBRARY.md)（A 层目录、TXT Ver01/Ver02、槽位→设备 ID=槽位+1）  
**现场样例**：`\\zh-mfs-srv.oplink.com.cn\Passive\EDFATemp\lmh\新建文件夹\`（`1830331040_33  OK.txt` vs `NG.txt`）  
**相关代码**：`ProcessCardTxtWriterVer02` / `ProcessCardVer02Parser` / `ParameterBlockReader`（`#150`） / `ProcessLibraryStore`

---

## 0. 和业务对齐的一句话

**模板不好用 ≠ TXT 文案/排版问题，而是交换格式不该以人手填表为源。**

正确源顺序：

1. 设备上调好的参数（`#150` 读回）  
2. 上位机结构化落盘（短期仍为「完整」工艺卡；中期为 JSON）  
3. 再上传工艺库 / 再下发  

人手改 TXT、半成品空白表、条件导出缺行，一律视为不合格源。

---

## 1. 问题结论（已调查，本计划不重开争论）

| 现象 | 结论 |
|------|------|
| 厂商 SDSoft 能改参拧紧，我们导出→导入「不好用」 | 线协议不同（私有 TCP vs Modbus），**参数块语义同源**；主因不是「没用私有协议」 |
| OK vs NG 同 PN/后缀 | NG **关键字段缺行或掏空**（扭矩行缺失、角窗 0、两阶段 0），非 Ver02 语法损坏 |
| 业务要求把「后缀」当随机控制器序号、填进 `ID【】` | **与现契约冲突**：后缀=库槽位；设备 ParameterId=**槽位+1**；卡上 ID **故意空且不参与身份** |
| SDSoft 崩溃栈夹在样例目录 | 厂商 UI bug，与工艺卡解析无关 |

**本计划明确不做**：改走 SDSoft 私有 TCP；把卡上 `ID` 改成「每次下发随机号」；继续扩大人手填 TXT 为权威源。

---

## 2. 目标与阶段

```text
短期（不改交换格式）          中期（改权威落盘）
─────────────────────        ────────────────────────────
完整卡准入 + 缺行拒收    →    screws/{slot}.json 为权威
排查 #150 / 条件导出缺行 →    UI 默认 JSON 或 process-pack.zip
操作规范与拒收提示       →    TXT 仅迁移/只读兼容
```

成功标准（产品语言）：

- 技术员按「设备读回 → 导出 → 上传」路径，**不再出现静默半成品卡进库**。  
- 业务理解：槽位后缀稳定；设备 ID 系统推导；不填红圈 ID。  
- 中期后：工艺库参数槽默认 JSON；往返字段全量（0 也保留）；TXT 不再作为新建主路径。

---

## 3. 短期：不改格式也能止血

### 3.1 操作规定（先发业务 / 产线，可不发版）

1. **只认**「拧紧参数页：从设备 `#150` 读入编辑器 → 导出」得到的卡。  
2. **拒收** NG 类缺行卡：对照同策略完整样板，缺启动/旋入扭矩行、拧紧角窗全 0、两阶段应开却全 0 等，不得上传工艺库、不得作为下发源。  
3. **禁止**以空白 Ver02 表手填后冒充设备导出。  
4. **身份口径**：`拧紧参数后缀【NN】` = 库槽位；设备 ID = NN+1；`ID【】` 不用填、填了也不生效。  
5. 导入导出「不一致」先核对：是否把后缀当成了设备号（例：后缀 33 → 设备 **34**）。

### 3.2 工程止血（实现任务，仍保持 TXT）

| ID | 任务 | 说明 | 主要触及 |
|----|------|------|----------|
| S1 | **完整卡校验（拒收）** | 上传工艺库 / 可选导出后自检：标准策略下前 4 段必填字段集合；缺关键行或「有效段扭矩/角窗明显空洞」→ 明确错误，禁止入库 | `ProcessLibraryService.UploadProcessCardAsync`；新建 `ProcessCardCompletenessValidator`（名可调整） |
| S2 | **条件导出改为全量行** | Ver02 `WriteStage`：目标扭矩/扭矩率/夹紧等**按模板固定行写出**（值为 0 也写），避免 `#150` 读回 0 时再导出「缺行」被误认为另一种格式 | `ProcessCardTxtWriterVer02.WriteStage` |
| S3 | **`#150` 读不全排查** | 对照 SDSoft / 手册：349 word 是否整块进 `TighteningParameterCore`；进阶/两阶段/角窗是否丢；日志打 ParameterId、非零字段摘要 | `ParameterBlockReader` / `TighteningParameterCodec`；必要时加单测用已知 word 夹具 |
| S4 | **HMI 提示** | 导出成功提示「请仅上传本导出文件」；上传失败展示校验明细（缺哪些字段） | `ControllerParameterViewModel` / `ProcessLibraryViewModel` + 中英文字符串 |
| S5 | **回归** | OK 样例可通过；人为删扭矩行的 NG 夹具必须拒收；现有 Ver01/Ver02 往返单测仍绿 | `tests/AutoScrew.Tests` |

### 3.3 短期验收

- [ ] 业务书面确认操作规定 3.1（或培训纪要）。  
- [ ] S1：缺行卡上传失败且文案可读。  
- [ ] S2：对「全 0 辅助字段」的设备读回，再导出**行集合稳定**（与完整样板同行对齐，允许值为 0）。  
- [ ] S3：有结论（读丢 / 未丢）；若读丢则修 codec 并补测。  
- [ ] 不改变：槽位+1、TXT Ver01/Ver02 识别、默认超时 120s、MES 字段。

---

## 4. 中期：工艺库权威改为 JSON（TXT 降级）

### 4.1 目标形态

```text
{processRoot}/{产品PN}/
  product.json                 # slots[].fileName → screws/00.json（过渡期可仍指向 .txt）
  screws/
    00.json                    # 权威：TighteningParameterTemplate + 元数据
    00.txt                     # 可选：迁移残留或「导出给人看」；新建默认不再写
  sequences/
    01.json                    # 已是权威，保持
  template/                    # 螺钉模板（C 层），本计划不改契约
```

**交换包（推荐默认导出/导入）**：

```text
{productPn}.process-pack.zip
  manifest.json                # schemaVersion, productPn, contentHash, slots[], sequences[]
  screws/{slot:D2}.json
  sequences/{id:D2}.json
```

### 4.2 JSON 参数卡最小契约（草案，实现前冻结一版）

文件建议名：`screws/{slot:D2}.json`，`schemaVersion: 1`。

| 字段组 | 内容 |
|--------|------|
| 身份 | `screwPn`, `slotId`（0-based）；**不**要求人手填设备 ParameterId |
| 设备映射 | 写入时 `parameterId = slotId + 1`（与现 `ProcessParameterCode` 一致） |
| 核心 | 现有 `TighteningParameterTemplate` / `TighteningParameterCore` 可序列化形状（全量阶段、0 保留） |
| 元数据 | `author`, `exportedAt`, `source`=`deviceRead` \| `migratedFromTxt` \| `manual`（manual 默认禁止生产下发） |
| 单位 | 内部扭矩 **mN·m 整数**；显示层再转 lbf.in（消灭 TXT「显示单位当源」） |

顺序：继续 JSON；TXT/Excel 仅作导入适配器 → 落 `sequences/*.json`。

### 4.3 中期实现任务

| ID | 任务 | 说明 | 主要触及 |
|----|------|------|----------|
| M1 | **Schema + 序列化** | 定义 `ProcessParameterSlotDocument`（或复用 Template + envelope）；读写 API | Infrastructure / Application Abstractions |
| M2 | **Store 双读** | `LoadProductSlot`：优先 `.json`，回落 `.txt`（现 Parser）；`product.json` 的 `fileName` 迁移时改写 | `ProcessLibraryStore` / `ProcessLibraryService` |
| M3 | **TXT→JSON 迁移工具** | 按 PN 扫描 `screws/*.txt` → 校验完整 → 写 `.json` → 更新清单；失败列出文件 | CLI 或工艺库页「迁移本 PN」按钮 |
| M4 | **UI 默认 JSON/zip** | 参数页导出默认 `.json`；工艺库上传接受 `.json` / `.zip`；TXT 标为「旧格式/迁移」 | HMI ViewModels + 过滤器文案 |
| M5 | **全量往返单测** | 设备夹具 word → Core → JSON → Core 相等；TXT 迁移样例（OK 文件） | Tests |
| M6 | **文档与 User Manual** | 更新 `PROCESS_LIBRARY.md` §3/§4.1/§5；Help 操作路径；本文件标「已落地」 | doc/ |

### 4.4 中期验收

- [ ] 新上传默认 JSON；zip 包可整 PN 往返。  
- [ ] 旧 TXT 只读可迁移，迁移后生产路径走 JSON。  
- [ ] 身份规则不变（槽+1；卡/文档仍说明 ID 自动）。  
- [ ] 不引入私有 TCP；不改 MES/追溯字段表。

---

## 5. 与厂商格式的关系（边界）

| 靠近方式 | 本计划态度 |
|----------|------------|
| 参数块语义 / `#100#150` / 全量字段 | **做**（短期 S2/S3、中期 JSON 全量） |
| SDSoft `Tool*Parm.csv` 兼容 | **可选后续**（另开任务；作导入适配，不作唯一权威） |
| 改连私有 TCP | **不做**（见 §1） |

---

## 6. 沟通话术（可直接转业务）

- 红圈 `ID【】`：**不用填**；系统按后缀槽位自动算设备号（槽位+1）。  
- 后缀：`00/10/20` 是**程序类型习惯**，也可以是其它槽号；**不是**「每次下载随机变的顺序号」。  
- 缺行/半成品 TXT：**拒收**；请从本软件「读设备再导出」拿完整卡。  
- 中期会改为 JSON/zip 工艺包，减少表格填空差错；不是再出一套更复杂的 TXT。

---

## 7. 建议排期（相对顺序，不写死日历）

1. 发 §3.1 操作规定 + 收口现场 NG 卡  
2. 实现 S2（全量导出）与 S1（拒收）——立刻降低「缺行进库」  
3. S3 结论与必要修复  
4. 冻结 JSON schema → M1–M3 → M4 UI → M6 文档  

---

## 8. 修订记录

| 日期 | 说明 |
|------|------|
| 2026-11-25 | 初稿：短期止血 + 中期 JSON；基于现场 OK/NG 与身份口径讨论 |
