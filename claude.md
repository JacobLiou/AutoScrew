# AutoScrew / 智能锁附系统 — AI 助手说明

## 项目目的与边界

本仓库用于**智能锁附系统**的产线导入：自动取钉/供钉、智能电批拧紧、扭矩–角度曲线与规则判定、SN–PN 任务与 MES/服务器数据贯通。需求与验收的权威追溯见：

- [readme.md](readme.md) — 项目门面与架构概览  
- [doc/SPEC.md](doc/SPEC.md) — 对齐 [doc/智能锁附系统开发 (V1).xls](<doc/智能锁附系统开发 (V1).xls>) 的规格与 KPI  
- [doc/PRD.md](doc/PRD.md) — 会议结论（吸嘴快切、供料器尺寸、校正周期、控制器体积等）
- [doc/RISK_REGISTER.md](doc/RISK_REGISTER.md) — 风险登记  
- [doc/VERIFICATION_FAT_SAT.md](doc/VERIFICATION_FAT_SAT.md) — FAT/SAT 检查表  
- [doc/DATA_AND_TRACE.md](doc/DATA_AND_TRACE.md) — 数据与追溯契约（MES 字段变更优先改此文档）

不在未确认需求内自行扩大范围（例如将二期视觉标为已实现）。

## 安全与合规（产线）

- **禁止**建议或实现绕过安全门、急停、互锁、扭矩保护等 EHS/设备安全机制。
- 涉及真机运动、扭矩或高压上电的改动，应假定需在**脱机/仿真/受控维护模式**下验证；不在文档中鼓励带电插拔或违规短接。
- 验收与法规以用户现场与供应商合同为准；README/SPEC 中的数值来自 V1 表，若与最新合同冲突以合同为准。

## 代码与文档习惯

- **改动范围**：只改与当前任务相关的文件；避免无关格式化与同 diff 无关的重命名。
- **语言**：面向人的说明以**中文**为主；代码标识符、仓库目录与 API 名称遵循团队既有英文规范（若无则采用清晰英文全词）。
- **风格**：修改前先读上下文，匹配现有命名、分层与错误处理方式；不堆砌无意义注释与防御性 try/except。

## 技术栈（待项目确认后补全）

下列项在规格中未写死品牌，**不要编造**具体型号或协议细节：

- 主控形态（PLC / IPC + 软 PLC 等）  
- 现场总线（如 EtherCAT、Profinet 等）  
- 智能电批与拧紧控制器型号、开放 API  
- 视觉若二期引入：相机与算法栈  

确认后可在本文件增加「构建与运行」一小节（命令、环境变量、对 MES 的 mock 方式等）。

## 常用路径

| 文件 | 用途 |
|------|------|
| `readme.md` | 目标、范围、架构、参考链接 |
| `doc/SPEC.md` | V1 需求与验收追溯 |
| `doc/PRD.md` | PRD 会议摘录 |
| `doc/RISK_REGISTER.md` | 风险台账 |
| `doc/VERIFICATION_FAT_SAT.md` | FAT/SAT 验证检查表 |
| `doc/DATA_AND_TRACE.md` | 采样率、字段、SN–位绑定、MES 最小集 |
| `doc/*.xls` | 原始需求表（含图） |

## 协作提示

- 需求变更应同步 **Excel 或 SPEC**，避免 readme 与 SPEC 长期不一致。  
- **MES、曲线存储、SN–位号规则**变更：先改 [doc/DATA_AND_TRACE.md](doc/DATA_AND_TRACE.md)，再改实现，并视需要更新 [doc/VERIFICATION_FAT_SAT.md](doc/VERIFICATION_FAT_SAT.md)。  
- 若用户未提供新证据，不要擅自修改验收数字或阶段日期。
