# IEMD-SD 10.1 寸手册 GUI 提取索引

来源：[IEMD-SD系列10.1寸控制器.pdf](../IEMD-SD系列10.1寸控制器.pdf)（528 页，CH05–CH07 主要为截图，文本层不可检索）  
英文对照：[DELTA_IA-DCSS_SD3_UM_EN_20260310.pdf](../DELTA_IA-DCSS_SD3_UM_EN_20260310.pdf)（CH05 p.60–91、CH06 p.92–103、CH07 p.104+）  
全文摘录：`en_chapters.txt`

## 章节与页码（英文手册）

| 章节 | 内容 | PDF 页 |
|------|------|--------|
| CH05 | 拧紧参数 Parameters | 60–91 |
| CH06 | 拧紧顺序 Sequence | 92–103 |
| CH07 | 拧紧来源 Sources | 104–120 |

## 精选 GUI 截图（`gui/`）

| 文件 | 设备界面 |
|------|----------|
| `00_home_main_menu.jpg` | 首页：拧紧参数 / 顺序 / 来源 / 运行结果 + 底栏六图标 |
| `05_param_tightening_stage_angle.jpg` | 参数编辑：擰緊設定 · 角度控制阶段 |
| `05_param_tightening_stage_clamp_torque.jpg` | 参数编辑：夾緊扭矩阶段 |
| `05_param_tightening_stage_clamp_angle.jpg` | 参数编辑：夾緊角度阶段 |
| `05_param_stage_advanced_dialog.jpg` | 阶段「进阶设定」弹窗 |
| `06_sequence_menu_entry.jpg` | 首页高亮拧紧顺序入口 |
| `06_sequence_navigator_3d.jpg` | 顺序：锁附导引 + 3D 模型 + 步骤条 + D-pad |
| `07_source_single_axis_dual_tool.png` | 来源：**单轴独立** — 工具1/2 各绑一条顺序（ControllerA×8、B×4） |
| `07_source_dual_axis_interactive.png` | 来源：**双轴交互** — 单表 ControllerC×6 |
| `07_source_advanced_settings_dialog.png` | 来源：**进阶设定** — 启动条件 + 12 条互锁规则 |

完整提取：`images/`（CH05–07 区间约 p.49–81，共 103 张图）

## 设备三步 IA（与手册一致）

```mermaid
flowchart LR
  Home[首页] --> P[CH05 拧紧参数]
  P -->|"参数被引用"| S[CH06 拧紧顺序]
  S -->|"顺序被选为名称"| Src[CH07 拧紧来源]
  Home --> Src
```

**依赖关系**：来源页「名称」下拉 = 已建 **顺序**（CH06），非直接选参数（CH05）。底栏仍可直跳各页，但正确配置顺序为 参数 → 顺序 → 来源。

底栏常驻：首页 · 参数 · 顺序 · 来源 · 运行结果 · 报表履历
