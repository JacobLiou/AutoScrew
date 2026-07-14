# IEMD-SD 单设备联调 SOP（Modbus only）

| 版本 | 日期 | 说明 |
|------|------|------|
| 1.0 | 2026-07-14 | 对齐单设备会话锁重构后联调 |

## 强制约束

1. **同时只允许一个客户端**连接控制器 `IP:502`（关掉 Supplier Demo / 其它上位机）。
2. AutoScrew：`AutoScrew:UseSimulatedHardware=false`，设备连接页 **测试连接 = 应用**（同一持久连接）。
3. 作业拧紧进行中，勿在拧紧参数/顺序/来源页下发；按钮会因 `IsDeviceBusy` 置灰。

## 最小路径

1. **设备连接** → 填 Host/Port/ToolIndex → **应用**（成功即长连接）。
2. **拧紧参数** → **刷新设备列表 (#160)** → 对列表中 ID（或控制器确认存在的 ID）→ **从设备读取 (#150)**。
3. 按需修改 → **下发 (#100)** → **激活 (#302)**（来源切换方式须为手动）。
4. **拧紧来源** → 手动切换 → 写 #300/#301。
5. （可选）**拧紧顺序** → 读写/激活 #303。
6. **作业台** → 扫 SN → 等扳机 → 完成一钉；确认右侧曲线/日志。

## 排障

| 现象 | 处理 |
|------|------|
| `#150` code 3 | ID 未配置；刷新 #160 或核对 ToolIndex |
| `Device is busy` | 等待作业周期结束 |
| `Modbus not connected` | 再点「应用」重连；确认网线与独占 502 |
| `Write … 0xC8 failed` | 独占连接；勿与 Demo 并用；重应用后重试 |
| WaitFinish timeout | 检查扳机/Ready；`TighteningCycleTimeoutMs` 默认 120s |

参见 [driverDebug.md](driverDebug.md)（现象速查与 Demo 对照）、[IEMD_SD_MANUAL_FEED_COMMISSIONING.md](IEMD_SD_MANUAL_FEED_COMMISSIONING.md)、[driverAnaC.md](driverAnaC.md)。
