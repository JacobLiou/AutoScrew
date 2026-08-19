# 本地配方注册表（脱机验证）

α 阶段用于**无真 MES** 时验证 SN→PN→模板链路。配置 `UseMockMes=true` 且 `UseLocalRecipes=true`（默认）时生效。

## 文件位置

优先读取 `{TemplateDirectory}/local-recipes.json`；若无则读 `{DataDirectory}/local-recipes.json`。  
Development 默认 `TemplateDirectory=Samples`，随 HMI 输出目录拷贝样例。

## 格式

```json
{
  "version": 1,
  "products": [
    {
      "partNumber": "PNDEMO",
      "templateFile": "demo-product-multisurface.product-template.json",
      "serialNumbers": ["SN001", "SN002"],
      "screws": []
    }
  ]
}
```

| 字段 | 说明 |
|------|------|
| `partNumber` | PN，与 MES 返回一致 |
| `templateFile` | 相对 `TemplateDirectory` 的模板；省略时为 `{partNumber}.product-template.json` |
| `serialNumbers` | 该 PN 下允许的 SN 列表（一个 PN 可对应多个 SN） |
| `screws` | 可选；非空时覆盖 MES 螺钉工艺（与线网 Mock 一致） |

## 技术员 / 操作员流程

1. 技术员在**产品模板编辑器**保存 `{PN}.product-template.json` 到 `TemplateDirectory`（或显式 `templateFile` 指向已有文件）。
2. 编辑 `local-recipes.json`：增加 PN、SN 列表、模板文件名。
3. 操作员登录后扫**已登记 SN** → 加载对应 PN 与模板；未登记 SN 拒绝（注册表存在时）。

若 **不存在** `local-recipes.json`，Mock MES 回退旧行为：任意 SN（≥3 字符）→ `PNDEMO` + `demo-product-multisurface.product-template.json`。

## 关闭

`appsettings` 中 `"AutoScrew": { "UseLocalRecipes": false }` 可强制使用旧 Mock，忽略注册表。

## 相关代码

- `LocalJsonRecipeStore` / `LocalRecipeMesClient`
- `ConfigurableMesClient`（Mock 分支）
