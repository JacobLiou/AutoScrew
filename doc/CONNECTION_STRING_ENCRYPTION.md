# MIMS 连接串加密说明

## 概述

AutoScrew 使用对称加密保护 MIMS MySQL 连接串。新版本支持**可跨机器部署**的加密，同时兼容旧版本 Windows DPAPI 密文。

## 加密方式对比

| 方式 | 位置 | 跨机 | 用途 | 状态 |
|------|------|------|------|------|
| **DPAPI 旧版** | 密钥 = 本机/用户 | ❌ 不行 | 仅本机用 | 仍可读取 |
| **AES-256 新版** | 密钥 = 固定 Entropy | ✅ 可以 | 任意部署 | 现在推荐 |
| 明文 | 不加密 | ✅ 可以 | 应急 | 不推荐 |

## 生成加密连接串

### 步骤 1：准备连接串明文文件

在任意机器上创建一个临时文本文件，例如 `conn.txt`：
```
Server=192.168.1.100;Database=mims;User ID=operator;Password=MySecurePassword;Charset=utf8mb4;
```

> **提示**：
> - 必须包含 `Charset=utf8mb4`（AutoScrew 强制要求）
> - 密码中若含特殊字符，需按 MySQL 连接串规则转义

### 步骤 2b：从旧 DPAPI 密文迁移（换机部署）

若现有 `appsettings.json` 中为旧版 `AQAAANCMnd8...` 格式（绑定本机），在**生成旧密文的同一台电脑**上运行：

```bash
dotnet run --project tools/EncryptMimsConnectionString -- --migrate-dpapi "<旧密文>" LocalMachine
```

输出 `aes256:...` 替换 `ConnectionStringDpapiBase64` 后即可拷贝到任意 Windows PC。

### 步骤 2：运行加密工具

从仓库目录运行：
```bash
dotnet run --project tools/EncryptMimsConnectionString -- <conn.txt的路径>
```

示例：
```bash
dotnet run --project tools/EncryptMimsConnectionString -- "C:\Temp\conn.txt"
```

输出示例：
```
aes256:NqR51ZRITS6v7pPKm1mLLskOdZ7mBKDnUs7+wHZgf7uxzzmXWconuEhLpyb9AQKT5nxco1XIJbieJ4FOSZzzfy31TpcvWLzBBPuOZoD3JeEf2W1Pji/5zxPpQewqYZwG
```

### 步骤 3：复制密文到目标机器配置

将工具输出的密文复制到任意部署机器的 `appsettings.json`：
```json
{
  "Authentication": {
    "Mims": {
      "ConnectionString": "",
      "ConnectionStringDpapiBase64": "aes256:NqR51ZRITS6v7pPKm1mLLskOdZ7mBKDnUs7+wHZgf7uxzzmXWconuEhLpyb9AQKT5nxco1XIJbieJ4FOSZzzfy31TpcvWLzBBPuOZoD3JeEf2W1Pji/5zxPpQewqYZwG",
      "DpapiScope": "LocalMachine"
    }
  }
}
```

> **重要**：
> - 保留 `ConnectionString` 为空（明文优先级更高，仅作备选）
> - `DpapiScope` 参数仅用于兼容旧版 DPAPI 密文，新版加密不使用此参数

### 步骤 4：启动应用

在任意 Windows 机器上启动应用，它会自动解密连接串并连接到数据库。

## 应急方案：使用明文连接串

若无法生成密文或需要快速调试，可直接在 `appsettings.json` 中配置明文连接串：
```json
{
  "Authentication": {
    "Mims": {
      "ConnectionString": "Server=192.168.1.100;Database=mims;User ID=operator;Password=MySecurePassword;Charset=utf8mb4;",
      "ConnectionStringDpapiBase64": "",
      "DpapiScope": "LocalMachine"
    }
  }
}
```

应用将直接使用 `ConnectionString` 字段，忽略 `ConnectionStringDpapiBase64`。

> **安全警告**：明文连接串包含敏感凭证，建议仅用于开发/测试环境，或由 CI/CD 工具在部署时动态注入。

## 兼容旧版 DPAPI 密文

若现有部署仍在使用旧版 Windows DPAPI 密文（无 `aes256:` 前缀），应用仍会尝试以旧方式解密：
1. 先检查是否有 `aes256:` 前缀（新版）
2. 若无前缀，按 DPAPI 模式解密（旧版）

无需修改任何代码即可平滑过渡。

## 迁移指南

### 旧版（DPAPI）➜ 新版（AES）

1. 在任意机器用新工具重新生成密文（见上文"步骤 1-2"）
2. 替换 `appsettings.json` 中的 `ConnectionStringDpapiBase64` 值
3. 重新启动应用

新密文可在任意 Windows 机器使用，无需绑定原始加密机。

## 密钥安全说明

- **新版 AES 密钥**：派生自 SHA256（固定 Entropy），存储在代码中  
  → 强度适中，足以保护传输中配置  
  → 不依赖本机/用户，支持跨机部署
  
- **明文连接串**：应由环境变量或密钥管理系统（如 Azure Key Vault）提供  
  → 不应硬编码在版本库中

## 故障排查

### 错误："无法解密 Authentication:Mims:ConnectionStringDpapiBase64"

**原因**：旧版密文格式或密钥不匹配

**解决**：
1. 检查日志是否显示旧版 DPAPI 解密失败
2. 用新工具重新生成密文（会自动使用新格式）
3. 如果仍需用旧版 DPAPI，确保 `DpapiScope` 值与生成时一致

### 错误："未配置 MIMS 数据库连接"

**原因**：`ConnectionString` 和 `ConnectionStringDpapiBase64` 都为空

**解决**：
1. 检查 `appsettings.json` 是否包含有效的 `ConnectionStringDpapiBase64` 或 `ConnectionString`
2. 确保密文格式正确（新版以 `aes256:` 开头；旧版为纯 Base64）
3. 参考"步骤 1-3"生成新的加密连接串

### 无法连接到 MySQL（解密成功但连接失败）

**排查**：
1. 检查网络和防火墙（能否 ping 到数据库服务器）
2. 验证 MySQL 用户名/密码是否正确
3. 确认 MySQL 允许该 IP 连接（检查 `max_connections` 等配置）

## 测试加密/解密

运行单元测试验证功能：
```bash
dotnet test tests/AutoScrew.Tests/ --filter MimsConnectionString
```

应输出 2 个测试通过：
- ✓ `ProtectToBase64_uses_portable_prefix_and_roundtrips`（新格式往返）
- ✓ `UnprotectFromBase64_still_supports_legacy_dpapi_payload`（旧版兼容）

## 技术细节

### 新版加密格式

```
aes256:<Base64(IV + AES-256-CBC(plaintext))>
```

- **算法**：AES-256-CBC
- **密钥**：SHA256("AutoScrew.MimsConnection.Portable.v2")
- **IV**：每次加密随机生成（前 16 字节）
- **数据**：UTF-8 编码的连接串

### 实现位置

- **加密/解密逻辑**：`src/AutoScrew.Infrastructure/Authentication/MimsConnectionStringDpapi.cs`
- **加密工具**：`tools/EncryptMimsConnectionString/Program.cs`
- **单元测试**：`tests/AutoScrew.Tests/MimsConnectionStringDpapiTests.cs`
