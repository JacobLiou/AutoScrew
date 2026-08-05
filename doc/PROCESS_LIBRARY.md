# 工艺参数库（A 层）

| 版本 | 日期 | 状态 |
|------|------|------|
| 1.0 | 2026-08-03 | 落地：产品 PN 目录 + TXT 工艺卡 + 按槽位下发 |

**需求追溯**：[PRD.md](PRD.md) §3.2.4 模板库（A 工艺库）  
**样例**：[samples/1830330479_00.txt](samples/1830330479_00.txt)

## 1. 业务模型

| 概念 | 含义 |
|------|------|
| **产品 PN** | 拉取/换产主键；一个产品下多颗螺钉工艺 |
| **螺钉 PN** | 工艺卡身份（TXT `参数ID`），写入参数 `Name` 便于追溯 |
| **槽位 / ParameterId** | TXT `参数：00` → 设备参数号 **0**（`01`→1…） |

> 台达手册多写参数 ID **1–500**；现场工艺卡以 `00` 起槽。上位机协议层已放宽为 **0–500**。若真机拒绝 0，再改为 `slotId+1` 映射。

## 2. 目录约定

优先写局域网（MES「局域网根路径」或 `AutoScrew:OptionalNetworkArchiveRoot`），否则回落本机 `{DataDirectory}/process`：

```text
{processRoot}/{产品PN}/
  product.json
  screws/
    00.txt
    01.txt
    …
```

`product.json`：

```json
{
  "schemaVersion": 1,
  "productPn": "YOUR-PRODUCT-PN",
  "updatedUtc": "2026-08-03T01:00:00Z",
  "slots": [
    {
      "slotId": 0,
      "screwPn": "1830330479",
      "fileName": "screws/00.txt",
      "displayName": "1830330479"
    }
  ]
}
```

局域网账号复用 SN 归档：`PRED-TESTING` + `AutoScrew:LanSharePasswordAes256`。

## 3. TXT 工艺卡（拧紧参数）

关键字段（行尾 `<…>` 注释忽略）：

| TXT | 映射 |
|-----|------|
| `参数ID` | 螺钉 PN → `Core.Name` |
| `参数：NN` | `ParameterId`（十进制，可前导 0） |
| `阶段有效：N` | 使用前 N 段（通常 4） |
| 最大/最小总角度 | `MaxAngleDeg` / `MinAngleDeg` |
| 最大拧紧时间（秒） | `MaxTighteningTimeTenthSec`（×10） |
| 拧紧启动延时（×0.01） | `TighteningStartDelayCentiSec` |
| 拧松同类 | `MaxLoosenTimeTenthSec` / `LoosenStartDelayCentiSec` / `MaxLoosenAngleDeg` |
| 进阶 ON/OFF、补偿 ID 等 | 对应 `TighteningParameterCore` 字段 |
| 曲线取样起始扭矩（mNm） | 已是 mN·m，直接写入 |
| 各段扭矩（lbf.in） | `TorqueUnitConverter` → mN·m |
| 拧松两段角度/速度、生产履历、最小扭矩 | `TighteningLoosenCore` |

控制模式启发式（与工艺卡「n 选 1」一致）：

- **1.启动**：固定角度  
- **2.旋入**：扭矩率>0 → 扭矩率；否则有角度 → 角度；否则扭矩  
- **3.预紧**：夹紧扭矩>0 → 夹紧扭矩；否则扭矩  
- **4.拧紧**：按夹紧角度 / 夹紧扭矩 / 扭矩 / 角度优先级选择  

`扭矩判断`/`角度判断` 为 OFF 时，对应上下限写 0（与参数页开关行为一致）。

本期范围：**仅拧紧参数**；顺序/来源仍走设备页或后续挂到同一产品目录。

## 4. HMI 操作流

1. 输入 **产品 PN** → 刷新槽位列表  
2. **上传工艺卡**：选 TXT → 解析 → 写入 `screws/{slot:00}.txt` 并更新清单  
3. **按下发到设备**：按清单逐槽 `WriteParameter`，并回写本机参数预设  

## 5. 与 B/C 层关系

```text
工艺库(A) --按产品PN--> 设备参数槽 --可选--> 设备配置包(B)
产品模板(C) Templates/{产品PN}/  钉位引导，与 A 共用产品 PN
```
