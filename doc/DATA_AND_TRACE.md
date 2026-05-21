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

## HTTPS

- 生产环境 MES 基址使用 HTTPS（TLS 1.2+），见 `MesHttpClient` 与现场证书策略。

## MIMS MySQL 登录（只读）

- `Authentication:Mode` = `MimsMySql` 时使用 [`MimsMySqlAuthenticationService`](src/AutoScrew.Infrastructure/Authentication/MimsMySqlAuthenticationService.cs)：仅 `SELECT` `mims_person` + `mims_role`；口令算法见 [`MimsPasswordHasher`](src/AutoScrew.Infrastructure/Authentication/MimsPasswordHasher.cs)。
- 连接串键：`Authentication:Mims:ConnectionString`（**仅 User Secrets / 环境变量**，勿提交 `src/temp/connstring.txt`）。
- 角色映射（二元）：[`MimsRoleMapper`](../src/AutoScrew.Infrastructure/Authentication/MimsRoleMapper.cs) 读取 `mims_role.name`——名称**含「操作员」**（如 `操作员`、`七分厂操作员`、`单步权限操作员`）→ `Operator`（仅作业）；**其余**（技术员、工程师、Super Admin、生产管理员等）→ `Technician`（模板配置、解锁 NG）。现场 `mims_role.type` 常为 0，**不可**用于授权。`Authentication:Mims:RoleMap` / `UnmappedRoleBehavior` 已废弃，仅保留配置键兼容。
