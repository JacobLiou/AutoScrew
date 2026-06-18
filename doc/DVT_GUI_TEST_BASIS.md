# AutoScrew GUI DVT测试依据（需求说明 + 基础使用方法）

> 文档目的：为DVT测试组提供可直接落地的GUI测试依据，用于编写测试方案、测试用例与执行记录。
> 
> 适用版本：当前仓库主干（2026-05-26）
> 
> 说明：本文件采用“双轨口径”编写。
> 
> - A轨：PRD/README中的目标需求（应测目标）。
> - B轨：当前代码已实现行为（可测现状）。
> - Gap：目标与现状差异（测试方案需标记“待实现”或“条件执行”）。

---

## 1. 资料来源与优先级

### 1.1 权威来源（需求侧）

以王工的Excel文档位基础

### 1.2 GUI实现来源（代码侧）

1. 启动/登录流程

- [src/AutoScrew.Hmi/App.xaml.cs](../src/AutoScrew.Hmi/App.xaml.cs)
- [src/AutoScrew.Hmi/LoginWindow.xaml](../src/AutoScrew.Hmi/LoginWindow.xaml)
- [src/AutoScrew.Hmi/ViewModels/LoginViewModel.cs](../src/AutoScrew.Hmi/ViewModels/LoginViewModel.cs)

2. 主窗体/导航/权限

- [src/AutoScrew.Hmi/MainWindow.xaml](../src/AutoScrew.Hmi/MainWindow.xaml)
- [src/AutoScrew.Hmi/ViewModels/MainShellViewModel.cs](../src/AutoScrew.Hmi/ViewModels/MainShellViewModel.cs)
- [src/AutoScrew.Application/Abstractions/ICurrentUser.cs](../src/AutoScrew.Application/Abstractions/ICurrentUser.cs)
- [src/AutoScrew.Infrastructure/SessionCurrentUser.cs](../src/AutoScrew.Infrastructure/SessionCurrentUser.cs)

3. 作业流程与状态机

- [src/AutoScrew.Hmi/Views/OperationPageView.xaml](../src/AutoScrew.Hmi/Views/OperationPageView.xaml)
- [src/AutoScrew.Hmi/ViewModels/MainViewModel.cs](../src/AutoScrew.Hmi/ViewModels/MainViewModel.cs)
- [src/AutoScrew.Application/Services/OperatorSessionController.cs](../src/AutoScrew.Application/Services/OperatorSessionController.cs)
- [src/AutoScrew.Domain/Session/JobSessionPhaseMachine.cs](../src/AutoScrew.Domain/Session/JobSessionPhaseMachine.cs)

4. 模板编辑（技术员）

- [src/AutoScrew.Hmi/Views/TemplateBoardView.xaml](../src/AutoScrew.Hmi/Views/TemplateBoardView.xaml)
- [src/AutoScrew.Hmi/ViewModels/TemplateBoardViewModel.cs](../src/AutoScrew.Hmi/ViewModels/TemplateBoardViewModel.cs)
- [src/AutoScrew.Hmi/Services/TemplateJsonSerializer.cs](../src/AutoScrew.Hmi/Services/TemplateJsonSerializer.cs)

5. 测试环境配置

- [src/AutoScrew.Hmi/appsettings.Development.json](../src/AutoScrew.Hmi/appsettings.Development.json)
- [src/AutoScrew.Hmi/appsettings.json](../src/AutoScrew.Hmi/appsettings.json)
- [src/AutoScrew.Hmi/Samples/demo-template.json](../src/AutoScrew.Hmi/Samples/demo-template.json)



---

## 2. DVT测试边界

### 2.1 本文覆盖

1. GUI可见功能与交互流程（登录、作业、模板、导航、系统工具）。
2. 与GUI直接相关的会话状态、权限控制、异常提示。
3. GUI触发的数据行为（曲线显示、模板读写、结果上传触发、日志入口）。

### 2.2 本文不覆盖

1. 具体硬件品牌协议深度联调细节。
2. 未落地MES字段定稿（以IT接口规范为准）。
3. 非GUI内部算法精度验证（应由算法/域逻辑专项测试补充）。

---

## 3. 双轨需求基线（目标 vs 现状）

## 3.1 A轨：目标需求（PRD/README）

1. 操作主流程：扫码SN -> 校验 -> 拉取PN工艺与模板 -> 引导锁附 -> 实时曲线判定 -> 全部完成后上传。
2. 状态反馈：待处理黄闪、进行中黄常亮、OK绿、NG红并报警。
3. 权限：操作员仅作业，技术员可参数与NG解锁，管理员可系统配置。
4. 异常：浮锁/滑牙/斜锁/卡钉/漏锁需要检测与提示。
5. 追溯：按SN/位号保存结果与曲线，支持断网缓存与重传。

## 3.2 B轨：当前实现（代码证据）

1. 已实现登录窗 + 主窗体切换，且支持Development演示账号与MIMS模式切换。
2. 已实现作业页基础按钮链路：打开扫码、提交SN、运行当前螺钉、技术员解锁NG、复位会话。
3. 已实现会话状态机：Idle/SnPending/SnRejected/LoadingRecipe/Running/NgLocked/Completed。
4. 已实现模板编辑页（技术员可见）：底图加载、画板尺寸、双击加点、多选删除、JSON读写。
5. 已实现曲线展示（最近一步扭矩-角度）与模拟硬件、Mock MES通路。



---

## 4. GUI功能需求矩阵（供DVT拆测试项）

| 编号 | 模块 | A轨目标需求 | B轨当前实现 | DVT判定建议 |
|---|---|---|---|---|
| GUI-LOGIN-001 | 登录 | 支持账号登录与角色区分 | 支持用户名/密码登录，账号来源于配置或MIMS | 功能可执行 |
| GUI-LOGIN-002 | 登录记忆 | 支持记住登录信息 | 支持记住用户名+密码（本地凭据存储） | 功能可执行 |
| GUI-LOGIN-003 | 登录帮助 | 提供创建账号/忘记密码/其他帮助入口 | 已实现提示或邮件跳转 | 功能可执行 |
| GUI-SHELL-001 | 主壳导航 | 具备作业页、配置页入口 | 已有Operation与Template入口，Template受角色控制 | 功能可执行 |
| GUI-SHELL-002 | 权限隐藏 | 操作员不可进入模板配置 | CanUseTemplateBoard基于角色判定 | 功能可执行 |
| GUI-SHELL-003 | 登出 | 支持会话结束并回到登录 | 已实现登出确认与返回登录页 | 功能可执行 |
| GUI-OP-001 | SN输入提交 | 支持SN录入/校验/失败提示 | 已实现SN提交，短SN返回无效提示 | 功能可执行 |
| GUI-OP-002 | 作业状态 | 显示阶段与状态消息 | 已实现PhaseDisplay与StatusMessage | 功能可执行 |
| GUI-OP-003 | 螺钉位引导 | 显示产品图+点位状态变化 | 已实现点位状态Pending/InProgress/Ok/Ng渲染 | 功能可执行 |
| GUI-OP-004 | 曲线显示 | 显示扭矩-角度曲线 | 已实现最近一步曲线绘制 | 功能可执行 |
| GUI-OP-005 | NG锁定与解锁 | NG后需技术员介入解锁 | 已实现NgLocked与UnlockNg权限校验 | 功能可执行 |
| GUI-OP-006 | 作业完成 | 全部完成后结果归档并上传 | 已实现本地log保存、上传及失败入Outbox | 条件执行（依环境） |
| GUI-TPL-001 | 模板底图 | 支持加载产品底图并对齐画板 | 已实现底图加载、透明度、画板=底图像素 | 功能可执行 |
| GUI-TPL-002 | 点位标注 | 支持新增/选中/删除/编号 | 已实现双击新增、Ctrl多选、删除、自动重排编号 | 功能可执行 |
| GUI-TPL-003 | 螺钉规格 | 支持不同螺钉类型圈径 | 已实现右键类型菜单与圈径映射 | 功能可执行 |
| GUI-TPL-004 | 模板文件 | 支持JSON导入导出 | 已实现打开/保存JSON，支持相对/绝对底图路径 | 功能可执行 |
| GUI-SYS-001 | 工具入口 | 日志目录、程序目录、截图、关于 | 已实现对应命令 | 功能可执行 |
| GUI-PRD-001 | 三级权限完整能力 | 管理员系统配置能力完整 | 当前管理员在GUI上与技术员差异较少 | 待细化（需求补齐） |
| GUI-PRD-002 | 全量异常覆盖 | 浮锁/滑牙/斜锁/卡钉/漏锁完整可视化闭环 | 规则链路在域层有基础支持，GUI细粒度异常专题仍需增强 | 条件执行 |

---

## 5. 角色与权限测试依据

| 角色 | 最小能力 | 禁止能力 | 关键测试点 |
|---|---|---|---|
| 操作员 Operator | 登录、SN作业、运行当前螺钉、复位 | 模板页访问、NG解锁 | 侧栏不可见Template入口，解锁按钮执行应被拒绝 |
| 技术员 Technician | 操作员全部能力 + 模板编辑 + NG解锁 | 系统级未授权配置（若后续新增） | NG后可解锁继续，模板页可正常增删改保存 |
| 管理员 Administrator | 现阶段与技术员等价或更高 | 无（按现阶段） | 兼容技术员路径；后续需补管理员专属能力用例 |

权限判定依据：`Role >= Technician` 可模板访问与NG解锁。

---

## 6. 测试环境准备（执行前置）

## 6.1 推荐DVT基础环境（先跑通）

使用Development配置：

1. UseMockMes = true
2. UseSimulatedHardware = true
3. TemplateDirectory = Samples
4. 演示账号

- operator / demo
- tech / demo
- admin / demo

可选：`Authentication:FallbackToMockAccountsOnMimsFailure=true` 且保留 MIMS 连接串 + `Accounts` 时，演示前可故意断开 MIMS 网络，验证 `operator/demo` 回退登录仍能进入主界面；MIMS 恢复后自动恢复真库认证。

### 仿真场景（`AutoScrew:Simulation`）

| 键 | 值 | 效果 |
|----|-----|------|
| `FeedFailureMode` | `None` / `Timeout` / `Empty` / `Jam` | 第 N 次取钉失败 → `FEED_xxx` + NG 遮罩 |
| `FeedFailureOnScrewIndex` | `1` | 仅第 1 颗；`0`=关；`-1`=每颗 |
| `TighteningProfile` | `Ok` / `FloatLock` / `OverTorque` | 合成曲线 OK 或规则 NG |

示例（`appsettings.Development.json`）：

```json
"Simulation": {
  "FeedFailureMode": "Empty",
  "FeedFailureOnScrewIndex": 2,
  "TighteningProfile": "Ok"
}
```

适用文件：[src/AutoScrew.Hmi/appsettings.Development.json](../src/AutoScrew.Hmi/appsettings.Development.json)

## 6.2 生产联调环境（条件执行）

使用MIMS模式：

1. Authentication:Mode = MimsMySql
2. MIMS连接串与网络可达
3. MesBaseUrl可用且证书策略通过
4. UseMockMes/UseSimulatedHardware按真实联调切换

适用文件：[src/AutoScrew.Hmi/appsettings.json](../src/AutoScrew.Hmi/appsettings.json)

---

## 7. 桌面软件基础使用方法（按角色）

## 7.1 操作员基础流程（Operator SOP）

### 步骤1：启动并登录

1. 启动HMI程序，出现登录窗口。
2. 输入账号（operator）与密码（demo），点击登录。

预期结果：

1. 登录成功，进入主界面。
2. 左侧显示作业相关导航。
3. 若用户名密码错误，显示“用户名或密码错误”。

### 步骤2：进入作业台并发起SN流程

1. 在作业页点击“扫码 / 输入 SN”。
2. 在输入框录入SN并点击“提交 SN”。

预期结果：

1. 状态栏显示“Validating SN...”。
2. SN合法时进入配方加载并准备作业。
3. SN非法时提示错误并保持可重输。

### 步骤3：执行当前螺钉作业

1. 点击“当前螺钉：取钉+拧紧”。
2. 观察点位状态变化与曲线更新。

预期结果：

1. 当前位状态从Pending/InProgress变为Ok或Ng。
2. 右侧曲线区显示最近一步扭矩-角度曲线。
3. StatusMessage显示步骤结果。

### 步骤4：异常与复位

1. 若出现NG，等待技术员解锁。
2. 需要重新开始时点击“复位会话”。

预期结果：

1. NG时普通操作员无法自行解锁。
2. 复位后Phase回到空闲流程可重新作业。

---

## 7.2 技术员基础流程（Technician SOP）

### 步骤1：登录并验证权限

1. 使用tech/demo登录。
2. 检查侧栏是否可进入Template页面。

预期结果：

1. Template入口可见可点。
2. 作业页功能与操作员一致。

### 步骤2：NG解锁继续

1. 在作业中制造或等待NG状态。
2. 点击“技术员解锁 NG”。

预期结果：

1. 会话从NgLocked返回Running。
2. 可继续后续螺钉作业。

### 步骤3：模板编辑基础操作

1. 进入Template页面。
2. 加载底图，必要时点击“画板=底图像素”。
3. 双击画板新增点位，Ctrl+单击多选，删除选中。
4. 右键点位切换螺钉类型（圈径）。
5. 保存JSON并重新打开验证一致性。

预期结果：

1. 状态栏显示操作成功或失败信息。
2. 点位编号自动连续重排。
3. 打开JSON后点位数量、位置、圈径信息保持一致。

---

## 7.3 管理员基础流程（Administrator SOP）

### 步骤1：登录与通路检查

1. 使用admin/demo登录。
2. 验证可访问作业与模板页。

预期结果：

1. 页面访问能力不低于技术员。

### 步骤2：系统工具使用

1. 执行程序截图（保存jpg）。
2. 打开日志目录、程序目录、关于窗口。
3. 执行登出并返回登录页。

预期结果：

1. 工具命令可执行且不导致主流程中断。
2. 登出后可再次登录进入系统。

---

## 8. 状态机测试依据（核心）

目标状态与触发：

1. Idle -> RequestScan -> SnPending
2. SnPending -> SnValidated -> LoadingRecipe
3. SnPending -> SnRejected -> SnRejected
4. LoadingRecipe -> RecipeLoaded -> Running
5. Running -> ScrewNg -> NgLocked
6. NgLocked -> TechUnlockContinue -> Running
7. Running -> AllScrewsComplete -> Completed
8. Completed -> ResetToIdle -> Idle

DVT建议：以上每条迁移至少覆盖1个正向用例，非法迁移至少覆盖1个反向用例。

---

## 9. 数据与追溯相关GUI检查点

1. 完成作业后应触发锁附日志JSON落盘。
2. 每个螺钉步骤应触发曲线CSV落盘。
3. 上传失败场景应触发Outbox入队，不阻断GUI会话结束。
4. 日志目录可从GUI工具入口直接打开。

注：具体字段结构以 [doc/DATA_AND_TRACE.md](./DATA_AND_TRACE.md) 与后续IT契约为准。

---

## 10. DVT测试方案编写建议

## 10.1 测试分层

1. 冒烟测试（P0）

- 登录、SN提交、单步锁附、曲线显示、复位、登出。

2. 核心功能测试（P1）

- 权限隔离、NgLocked解锁链路、模板读写一致性。

3. 异常与边界测试（P1）

- 错误SN、模板路径无效、无底图模板、空点位模板、保存异常。

4. 条件联调测试（P2）

- MIMS认证、真实MES上传、离线重传。

## 10.2 用例模板字段（建议）

1. 用例ID
2. 来源需求ID（可先映射为本文件编号）
3. 前置条件
4. 操作步骤
5. 预期结果（UI + 数据）
6. 实际结果
7. 结论（Pass/Fail/Blocked）
8. 证据（截图/日志/文件）

---

## 11. 已识别风险与测试注意事项

1. 需求文档与实现阶段存在差异，DVT报告需明确“未实现项”与“缺陷项”边界。
2. 当前仓库缺少SPEC/FAT-SAT实体文档，验收阈值需在后续版本补齐后回刷。
3. 生产环境依赖外部系统（MIMS/MES/网络证书），建议先完成Mock稳定性验证再切换联调。

