# 数据与追溯（占位 / 与实现对齐）

本文档为 [doc/Design.md](../doc/Design.md) 与上位机实现之间的**追溯字段锚点**。MES 最终字段以公司 IT 规范为准；变更时先更新本文再改 `IMesClient` / `LockJobResultPayload` / 出站队列表结构。

## 出站与幂等

- 出站表：`outbox_uploads`（`IdempotencyKey` = `{SerialNumber}:{CompletedAt:O}` 草案，可按 IT 要求替换为 GUID 或 MES 分配键）。
- 重传：后台 `OutboxMesRetryHostedService` 周期调用 `IMesClient.UploadResultAsync`。
- 数据库迁移：`dotnet ef database update --project src/AutoScrew.Infrastructure --startup-project src/AutoScrew.Hmi`（开发机）；首次运行 HMI 时 `InitializeAutoScrewDatabase` 会执行 `Migrate()`。

## 本地文件（PRD 草案对齐）

- 工作根：`%LocalAppData%\AutoScrew\work\{SN}\`（可通过 `AutoScrew:DataDirectory` 覆盖）。
- 曲线文件：`torque_curve_{positionIndex}_{timestamp}.csv`
- 锁附日志：`lock_log_{timestamp}.json`
- 可选网络镜像：`AutoScrew:OptionalNetworkArchiveRoot`（复制失败不阻塞产线）。
- **产品模板库（V1.2）**：`{HMI.exe 同级}/Templates/{PN}/`（`AutoScrew:TemplateDirectory`，默认 `Templates`）；含 v2 JSON 与背景图；脱机 SN 注册表 `Templates/local-recipes.json`（可选）。

## SQLite 实体（Infrastructure）

- `lock_records` / `screw_details` / `error_logs`：表已建；**T-08**：作业完成时 [`SaveLockRecordAsync`](../src/AutoScrew.Application/Abstractions/ILockSessionRepository.cs) 写入 `lock_records` + `screw_details`（`PositionIndex` = 全局位号）。
- `session_checkpoints`：作业断电恢复 checkpoint（JSON）；**T-07** 启动时 HMI 提示恢复，重载模板并合并螺钉状态（不含 `_screwRecords` 扭矩/曲线路径）。
- `outbox_uploads`：MES 上传重试队列。
- `user_audit_logs`：用户操作审计（仅追加 INSERT，HMI 无删除 API）。
- **`product_template_sync`（V1.2）**：PN、`LocalRelativePath`、`SyncState`（`LocalOnly` / `DownloadedFromMes` / `PendingUpload` / `Synced` / `Failed`）、`LocalFileHash`、`LastMesPullUtc` / `LastMesPushUtc`、`LastError`。

## 用户操作审计（操作员 / 技术员）

- **开关**：`AutoScrew:AuditLogEnabled`（默认 `true`）。
- **JSONL**：`{AuditDirectory}/user-audit-{yyyy-MM-dd}.jsonl`；`AuditDirectory` 为空时使用 `{DataDirectory}/audit`。
- **SQLite 表**：`user_audit_logs`（字段：`Timestamp`、`StationId`、`UserId`、`DisplayName`、`Role`、`Category`、`Action`、`Target`、`Detail`、`Success`、`SerialNumber`）。
- **采集**：登录/登出、页面导航、全局按钮点击、确认框、设置变更、作业台业务动作、技术员配置（模板/设备/拧紧参数）。
- **禁止写入**：口令明文、完整 Modbus 参数块。
- **保留**：现场按 IT 策略备份 `audit` 目录与 `autoscrew.db`；HMI 不提供审计删除入口。

## 多面产品模板（草案）

- **契约**：[MULTI_SURFACE_TEMPLATE.md](MULTI_SURFACE_TEMPLATE.md)（`schemaVersion: 2`、面 ID、全局/面内位号）。
- **HMI 线框**：[MULTI_SURFACE_UI_WIREFRAME.md](MULTI_SURFACE_UI_WIREFRAME.md)。
- **定稿后**须在本节增补：`screw_details.surface_id`、`local_index`、`global_index` 字段及曲线文件命名；`RecipeBundle` / `LockJobResultPayload` 与 MES 对齐后再改实现。
- **Phase 1（已实现）**：HMI 技术员侧 v2 整包编辑（左树 + 右画板）；`LoadProductAsync` 加载完整产品包。
- **Phase 2（已实现 · 作业台）**：
  - `OperatorSessionController` 多面 runtime（`ActiveSurfaceOrdinal`、按面 `SurfaceCheckpointSurface`）。
  - `session_checkpoints` JSON 含 `activeSurfaceOrdinal` + 每面 `surfaceId` / `progressState` / `screwStates`。
  - 曲线文件仍用 `torque_curve_{globalPositionIndex}_{timestamp}.csv`（global 按 `surfaceOrderThenLocalIndex`）。
  - **MES 待定稿**：`ScrewResultDto` 暂不增加 `surface_id` / `local_index` 上报字段；本地 checkpoint 与 `_screwRecords` 已按 `surfaceId` + `localIndex` 区分。
- **Phase 3（V1.2 · 已实现）**：
  - 本地库：`IProductTemplateLocalStore` → `{TemplateDirectory}/{PN}/{PN}.product-template.json`。
  - 扫码：`IRecipeProvisioningService` — MES recipe + `templatePackageUrl` 下载 zip → 失败则本地 fallback。
  - 技术员保存 → `product_template_sync.PendingUpload`；MES 下载成功 → `DownloadedFromMes`。

## 程控供料（PRD V1.1 — 占位）

- 契约草案：[FEEDER_CONTROL.md](FEEDER_CONTROL.md)
- 实现前不在 MES 载荷中上报供料字段；本地 `_screwRecords` / 审计可先记录 `Operation.Feed*` 动作
- 定稿后在本节增补：`ScrewResultDto.feed_*`、供料失败 `error_code` 枚举（如 `FEED_001`）

## HTTPS

- 生产环境 MES 基址使用 HTTPS（TLS 1.2+），见 `MesHttpClient` 与现场证书策略。

## MES HTTP v1（占位）

> **说明**：正式 MES 规范未定稿；下列路径与 JSON 形状与 [`MesHttpClient`](../src/AutoScrew.Infrastructure/Mes/MesHttpClient.cs) 一致，供 FAT 与 `tools/MesMockServer` 联调。IT 定稿后先改本节再改实现。

### 运行时配置

- 持久化文件：`{DataDirectory}/mes-settings.json`（HMI Mes 页 **保存**）。
- 字段：`UseMockMes`（`true` = 内存 Mock，不发 HTTP）、`BaseUrl`（须以 `/` 结尾）、`ApiKey`（可选）、`TimeoutSeconds`。
- 首次无文件时从 `appsettings` 的 `AutoScrew:UseMockMes`、`AutoScrew:MesBaseUrl` 种子。
- HMI **应用** 刷新内存快照，无需重启；出站与扫码走 `ConfigurableMesClient` → `MockMesClient` 或 `MesHttpClient`。
- 所有请求 query 带 `stationId`（`AutoScrew:StationId`）；`ApiKey` 非空时 Header `X-Api-Key`。

### 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `api/health` | 连通测试（可选）；200 = OK |
| GET | `api/sn/validate?sn=&stationId=` | 响应 `{ valid, partNumber, message }` |
| GET | `api/recipe?sn=&pn=&stationId=` | 响应 `{ templateJsonPath, templatePackageUrl, productImageUrl, screws[] }`；`templatePackageUrl` 相对 MES 基址，GET 返回 zip（含 JSON + images/）；`screws[].index` 映射 `ScrewRecipeDto.PositionIndex` |
| GET | `api/templates/{pn}/package` | 模板工程整包 zip（含 `{PN}.product-template.json` 与 `images/` 等；FAT / MesMockServer） |
| GET | `api/templates?stationId=` | 远端模板目录：`[{ partNumber, contentHash, modifiedUtc, packageUrl }]`；`contentHash` 为 PN 文件夹整包指纹 |
| POST | `api/templates/{pn}/package` | Body = `application/zip`（整包上传）；响应 `{ accepted, revision, contentHash }` |
| POST | `api/results` | Body = [`LockJobResultPayload`](../src/AutoScrew.Application/Abstractions/IMesClient.cs)；成功 2xx |

### 本地 Mock

```bash
dotnet run --project tools/MesMockServer
```

默认 `http://localhost:5080/`；Mes 页关 Mock 并填该基址即可测试连接与扫码流程。

## 控制器条码（T-03）

- SN MES 校验成功后，若 `AutoScrew:WriteSnToController=true` 且非仿真硬件，经 [`IemdSdControllerTraceService`](../src/AutoScrew.Infrastructure/Hardware/IemdSdControllerTraceService.cs) 写 IEMD-SD `#401`（`WriteBarcodeAsync`）。
- `StrictSnToController=false`（默认）：写失败仅日志，不阻断配方加载；`true` 时抛错拒绝作业。

## 供料失败码（T-06b · 仿真已用）

| 错误码 | 含义 | 解锁 |
|--------|------|------|
| `FEED_TIMEOUT` | 供料超时 | 技术员 `UnlockNgContinue` |
| `FEED_EMPTY` | 缺料 | 同上 |
| `FEED_JAM` | 卡料 | 同上 |

- 作业流：`PickScrewAsync` 抛 [`FeedFaultException`](../src/AutoScrew.Application/Abstractions/FeedFaultException.cs) → `NgLocked` + 审计 `Operation.FeedNg`。
- 无真机验收：Development 下 `AutoScrew:Simulation:FeedFailureMode` / `FeedFailureOnScrewIndex`（见 [DVT_GUI_TEST_BASIS.md](DVT_GUI_TEST_BASIS.md)）。

## 无真机仿真（Development）

- `AutoScrew:UseSimulatedHardware=true` + `UseMockMes=true`：完整操作员流程（扫码 → 自动取钉拧紧 → 曲线）。
- `AutoScrew:Simulation`：`FeedFailureMode`（`None|Timeout|Empty|Jam`）、`FeedFailureOnScrewIndex`（1-based，0=关，-1=每颗）、`TighteningProfile`（`Ok|FloatLock|OverTorque`）。

## MIMS MySQL 登录（只读）

- `Authentication:Mode` = `MimsMySql` 时使用 [`MimsMySqlAuthenticationService`](../src/AutoScrew.Infrastructure/Authentication/MimsMySqlAuthenticationService.cs)：仅 `SELECT` `mims_person` + `mims_role`；口令算法见 [`MimsPasswordHasher`](../src/AutoScrew.Infrastructure/Authentication/MimsPasswordHasher.cs)。
- **连接串配置**：见 [CONNECTION_STRING_ENCRYPTION.md](CONNECTION_STRING_ENCRYPTION.md)（部署必读）。
  - `Authentication:Mims:ConnectionString`：明文连接串（User Secrets / 环境变量；开发仅用）。
  - `Authentication:Mims:ConnectionStringDpapiBase64`：加密连接串（推荐生产环境使用）。
- 角色映射（二元）：[`MimsRoleMapper`](../src/AutoScrew.Infrastructure/Authentication/MimsRoleMapper.cs) 读取 `mims_role.name`——名称**含「操作员」**（如 `操作员`、`七分厂操作员`、`单步权限操作员`）→ `Operator`（仅作业）；**其余**（技术员、工程师、Super Admin、生产管理员等）→ `Technician`（模板配置、解锁 NG）。现场 `mims_role.type` 常为 0，**不可**用于授权。`Authentication:Mims:RoleMap` / `UnmappedRoleBehavior` 已废弃，仅保留配置键兼容。
