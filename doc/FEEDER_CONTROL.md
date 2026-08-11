# 程控供料器 — 控制契约（**已作废**）

| 版本 | 日期 | 说明 |
|------|------|------|
| 0.1 | 2026-06-11 | PRD V1.1：上料/供钉由上位机自动触发（占位契约） |
| 0.2 | 2026-08-11 | **作废**：现场确认供料为**手动**；不做上位机驱动、调度与配置页 |

**状态：已作废 — 勿再按本文实现驱动或验收。**

权威现状：[PRD.md](PRD.md) V1.4、[TODO.md](TODO.md) T-06/T-06a/T-06b（作废）、联调路径 [IEMD_SD_MANUAL_FEED_COMMISSIONING.md](IEMD_SD_MANUAL_FEED_COMMISSIONING.md)。

---

## 结论（V1.4）

- α / 产线：**只控制 IEMD-SD** 锁附（参数 / 顺序 / 来源 / 拧紧 / 曲线）。
- 取钉/供料：操作员现场手动；无 `IFeeder`、无供料器 HMI、无供料协议联调义务。
- 下文为历史草案，仅供查阅，不再维护。

---

<details>
<summary>历史草案（V1.1，只读）</summary>

曾规划：每钉拧紧前 `FeedAsync` → 成功后再 `#302` / 拧紧；`IFeeder` + HMI 供料器连接页；错误码 `FEED_TIMEOUT` / `FEED_EMPTY` / `FEED_JAM`。

仿真配置（代码中可能仍存在，非 α 必验）：`AutoScrew:Simulation:FeedFailureMode` 等。

</details>
