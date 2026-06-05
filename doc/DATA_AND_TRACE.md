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

## SQLite 实体（Infrastructure）

- `lock_records` / `screw_details` / `error_logs`：表已建，完整写入路径可在后续迭代接 `OperatorSessionController`。
- `session_checkpoints`：作业断电恢复 checkpoint（JSON）。
- `outbox_uploads`：MES 上传重试队列。
- `user_audit_logs`：用户操作审计（仅追加 INSERT，HMI 无删除 API）。

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

## HTTPS

- 生产环境 MES 基址使用 HTTPS（TLS 1.2+），见 `MesHttpClient` 与现场证书策略。

## MIMS MySQL 登录（只读）

- `Authentication:Mode` = `MimsMySql` 时使用 [`MimsMySqlAuthenticationService`](../src/AutoScrew.Infrastructure/Authentication/MimsMySqlAuthenticationService.cs)：仅 `SELECT` `mims_person` + `mims_role`；口令算法见 [`MimsPasswordHasher`](../src/AutoScrew.Infrastructure/Authentication/MimsPasswordHasher.cs)。
- **连接串配置**：见 [CONNECTION_STRING_ENCRYPTION.md](CONNECTION_STRING_ENCRYPTION.md)（部署必读）。
  - `Authentication:Mims:ConnectionString`：明文连接串（User Secrets / 环境变量；开发仅用）。
  - `Authentication:Mims:ConnectionStringDpapiBase64`：加密连接串（推荐生产环境使用）。
- 角色映射（二元）：[`MimsRoleMapper`](../src/AutoScrew.Infrastructure/Authentication/MimsRoleMapper.cs) 读取 `mims_role.name`——名称**含「操作员」**（如 `操作员`、`七分厂操作员`、`单步权限操作员`）→ `Operator`（仅作业）；**其余**（技术员、工程师、Super Admin、生产管理员等）→ `Technician`（模板配置、解锁 NG）。现场 `mims_role.type` 常为 0，**不可**用于授权。`Authentication:Mims:RoleMap` / `UnmappedRoleBehavior` 已废弃，仅保留配置键兼容。
