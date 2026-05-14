# AutoScrew.TemplateBoard

独立小工具：在可设定大小的画板上**双击**添加螺钉位（圆圈 + 下方序号），支持**多选**（Ctrl+单击）、**全选**（Ctrl+A 或工具栏）、选中时 **DropShadow** + **1Hz 量级**闪烁预览；**右键**可切换六种螺钉类型（不同圆圈直径）；将布局保存为 **JSON** 模板，供主 HMI（`AutoScrew.Hmi`）后续加载与产线引导（集成方式由主程序实现）。

## 运行

```powershell
cd "src/AutoScrew.TemplateBoard"
dotnet run
```

或在 Visual Studio 中将 `AutoScrew.TemplateBoard` 设为启动项目后 F5。

## 操作说明

1. 在工具栏输入**宽**、**高**，点击「应用尺寸」。
2. 在灰色画板**空白处双击**添加标注；圆心位于双击点，序号按添加顺序递增；新建点默认 **M2（26px）**。
3. **单击**某个标注为单选；**Ctrl+单击**在多选模式下切换该点选中状态。
4. **Ctrl+A** 或工具栏「全选」选中全部标注；选中项带**蓝色阴影**（类似设计器选中效果）并闪烁。
5. 单击画板空白（非标注）可**取消选中**。
6. **右键**标注 →「螺钉类型」子菜单中六种规格，可单独更改该点的圆圈直径。
7. 「删除选中」移除所有当前选中的点，序号自动重排。
8. 「保存 JSON」「打开 JSON」使用文件对话框读写字典文件。

## 产品底图（与螺钉位置对齐）

1. 点击「**加载产品底图**」选择 PNG/JPG 等，图片会铺在画板**最底层**，圆圈叠在图片之上。
2. 为做到**像素级对齐**：加载后点击「**画板=底图像素**」，把画板宽高设为图片的 `PixelWidth`×`PixelHeight`，此时画板坐标与位图像素一一对应，双击打的圆心即对应图上的像素位置。
3. 若需看清下层走线，可调节「**底图透明度**」滑块（仅在有底图时可用）。
4. 「**清除底图**」移除底图，不影响已保存的标注坐标。
5. 保存 JSON 时，若底图与 JSON 在同一目录或子目录下，会尽量写入**相对路径**便于拷贝产线；否则写入绝对路径。打开 JSON 时会自动尝试加载底图。

画板使用 `PreviewMouseLeftButtonDown`，即使底图铺满也可在图上双击添加标注。

## 六种螺钉类型（示意）

| Id | 显示名 | 直径(px) |
|----|--------|----------|
| 1 | M1.0 / 极小 | 18 |
| 2 | M1.4 / 很小 | 22 |
| 3 | M2（默认） | 26 |
| 4 | M2.5 | 30 |
| 5 | M3 | 34 |
| 6 | M4 / 较大 | 40 |

仅用于画板视觉，不等同于工艺公差表。

## JSON 字段（schemaVersion 1）

| 字段 | 类型 | 说明 |
|------|------|------|
| `schemaVersion` | int | 当前为 `1` |
| `boardWidth` | number | 画板宽度（像素） |
| `boardHeight` | number | 画板高度（像素） |
| `circleDiameter` | number | 新建标注时的默认直径（与默认 M2 一致，旧文件兼容用） |
| `productImageRelativePath` | string（可选） | 相对 JSON 所在目录的产品图路径 |
| `productImageAbsolutePath` | string（可选） | 当无法使用相对路径时的绝对路径回退 |
| `productImageOpacity` | number（可选） | 底图不透明度 0..1 |
| `markers` | array | 螺钉位列表 |
| `markers[].index` | int | 序号（从 1 起，保存时会重算） |
| `markers[].centerX` | number | 圆心 X（相对画板左上角） |
| `markers[].centerY` | number | 圆心 Y（相对画板左上角） |
| `markers[].screwTypeId` | int（可选） | 1..6，与上表对应；缺省时按 `circleDiameter` 就近匹配 |
| `markers[].circleDiameter` | number（可选） | 该点圆圈直径（像素）；缺省使用文档级 `circleDiameter` |

示例见自行保存的 `.json` 文件（保存时带缩进）。

## 与主程序关系

本仓库**不引用** `AutoScrew.Hmi`。主程序按上表解析 JSON，将 `centerX/centerY` 与 `circleDiameter`（或 `screwTypeId`）映射到产品图显示坐标（注意缩放与 DPI），并按作业状态驱动闪烁即可。
