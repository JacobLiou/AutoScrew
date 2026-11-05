---
name: 工艺库新建删除PN
overview: 在工艺库页 PN 选择区增加「新建 PN」「删除 PN」：新建在 process 根下创建空产品目录；删除递归清空该 PN（含 template/screws/sequences），并经两次严重确认。
todos:
  - id: store-api
    content: ProcessLibraryStore/Service 增加 CreateProduct / DeleteProduct
    status: completed
  - id: hmi-ui
    content: 工艺库页按钮 + ViewModel 命令与两次删除确认 + 双语字符串
    status: completed
  - id: docs-tests
    content: 更新 PROCESS_LIBRARY.md；补充 Create/Delete PN 单元测试；dotnet build/test
    status: completed
isProject: false
---

# 工艺库新建 / 删除 PN

## 背景

工艺库页顶部目前只有「加载产品内容」+「刷新列表」。业务需要显式管理 `process/{产品PN}/` 目录。目录约定见 [doc/PROCESS_LIBRARY.md](doc/PROCESS_LIBRARY.md)：

```text
{processRoot}/{产品PN}/
  product.json
  screws/ …          # 拧紧参数
  sequences/ …       # 拧紧顺序
  template/ …        # 螺钉模板
```

现有 [`ProcessLibraryStore.GetProductDirectory`](src/AutoScrew.Infrastructure/ProcessLibrary/ProcessLibraryStore.cs) / `ListProductPns` / `EnsureLanConnectedForWrite` 已可复用。

## 行为约定（已选定）

| 操作 | 行为 |
|------|------|
| **新建 PN** | 取 ComboBox 当前输入的 PN（经既有 `SanitizeFileName`）；在 `ProcessRootPath` 下 `CreateDirectory`；写入空 `product.json`（Slots/Sequences 空），便于「加载产品内容」得到空列表。若目录已存在 → 提示已存在，不覆盖。成功后刷新 PN 列表、选中该 PN、自动加载空内容。 |
| **删除 PN** | 递归 `Directory.Delete(productDir, recursive: true)`，清掉 template / screws / sequences / product.json 等全部内容。**两次** `ConfirmTips`：第一次写明不可恢复并列举三类数据；第二次「再次确认删除 PN xxx」。成功后刷新列表、清空当前选中与表格。 |
| **顺序门禁不变** | 空 PN 仍不能上传顺序（[`EnsureProductExistsWithParameters`](src/AutoScrew.Infrastructure/ProcessLibrary/ProcessLibraryStore.cs) 仍要求至少有参数槽）。 |

```mermaid
flowchart LR
  UI[ProcessLibraryPage] --> VM[ProcessLibraryViewModel]
  VM --> Svc[IProcessLibraryService]
  Svc --> Store[ProcessLibraryStore]
  Store --> Disk["process/PN/"]
```

## 实现要点

### 1. Store + Service API

[`ProcessLibraryStore`](src/AutoScrew.Infrastructure/ProcessLibrary/ProcessLibraryStore.cs)：

- `CreateProduct(string productPn)`：校验 PN → `EnsureRoot` + LAN 写连接 → 若 `Directory.Exists` 抛/返回已存在 → 否则建目录并 `SaveManifest` 空清单。
- `DeleteProduct(string productPn)`：校验 PN → LAN 写连接 → 目录不存在则视为已删或报错 → `Directory.Delete(..., true)` + 日志。

[`IProcessLibraryService`](src/AutoScrew.Application/Abstractions/IProcessLibraryService.cs) + [`ProcessLibraryService`](src/AutoScrew.Infrastructure/ProcessLibrary/ProcessLibraryService.cs)：暴露 `CreateProductAsync` / `DeleteProductAsync`（薄封装 + 审计由 VM 现有 `Audit` 完成）。

### 2. HMI

[`ProcessLibraryPage.xaml`](src/AutoScrew.Hmi/Views/Pages/ProcessLibraryPage.xaml) PN 区右侧按钮组，在「加载 / 刷新列表」旁增加：

- 「新建 PN」（Primary 或 Secondary）
- 「删除 PN」（`Appearance="Danger"`）

[`ProcessLibraryViewModel`](src/AutoScrew.Hmi/ViewModels/ProcessLibraryViewModel.cs)：`CreateProductCommand` / `DeleteProductCommand`；删除两次确认后调服务；复用 `RefreshProductListAsync` / `RefreshProductContentAsync` / 清空 Slots·Sequences。

双语键（[`Strings.zh-CN.xaml`](src/AutoScrew.Hmi/Themes/Strings.zh-CN.xaml) / [`Strings.en-US.xaml`](src/AutoScrew.Hmi/Themes/Strings.en-US.xaml)）：按钮文案、已存在/成功/失败、删除标题与两段正文（明确螺钉模板、拧紧参数、拧紧顺序、不可恢复）。

同步改 `S.ProcessLibrary.ProductPnHint`：说明可新建 PN 后再上传内容。

### 3. 文档与测试

- 更新 [doc/PROCESS_LIBRARY.md](doc/PROCESS_LIBRARY.md)「PN 建目录规则」：增加「界面新建 PN」可建空目录一行。
- 测试：在 `tests/AutoScrew.Tests` 用临时目录覆盖 store 根路径（对齐现有 ProcessLibrary 测试写法）覆盖：新建成功、重复新建失败、删除后 `ListProductPns` 不含该 PN、删除后 `template`/`screws`/`sequences` 均不存在。

## 非范围

- 不改拧紧参数页 JSON 导入导出；不改 Excel 相关 Infrastructure。
- 不删本地参数/顺序预设（仅删工艺库局域网/本机 `process/{PN}`）。
- 不引入「输入 PN 才能删除」的新对话框控件。
