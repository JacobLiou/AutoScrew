# AutoScrew HMI 发布快速参考

## 快速开始（3 步）

### 步骤 1：使用 PowerShell 脚本

```powershell
# 进入项目根目录
cd C:\path\to\AutoScrew

# 发布 x64 Release 版本
.\scripts\publish.ps1 -RuntimeId win-x64

# 发布 x86 Release 版本
.\scripts\publish.ps1 -RuntimeId win-x86

# 发布 ARM64 Release 版本
.\scripts\publish.ps1 -RuntimeId win-arm64
```

### 步骤 2：查看输出

```
publish/
├── win-x64/Release/          # x64 版本
│   ├── AutoScrew.Hmi.exe     # ← 主可执行文件
│   ├── AutoScrew.Hmi.dll
│   ├── dotnet.exe            # .NET 运行时
│   └── [所有依赖和运行时文件]
├── win-x86/Release/          # x86 版本
└── win-arm64/Release/        # ARM64 版本
```

### 步骤 3：分发和运行

- **无需安装** .NET 运行时，直接运行 `AutoScrew.Hmi.exe`
- 可以复制整个文件夹到目标机器
- 大小约 150-200 MB（取决于架构和配置）

## 常用命令

### PowerShell 脚本方式（推荐）

```powershell
# 默认：Release x64
.\scripts\publish.ps1

# 发布 Debug 版本
.\scripts\publish.ps1 -Configuration Debug

# 发布到自定义目录
.\scripts\publish.ps1 -OutputPath "D:\publish"

# 一次发布所有架构
@('win-x64', 'win-x86', 'win-arm64') | ForEach-Object {
    .\scripts\publish.ps1 -RuntimeId $_
}
```

### Batch 脚本方式

```batch
REM 发布 x64 Release 版本
publish.bat Release win-x64

REM 发布 Debug 版本
publish.bat Debug win-x64
```

### 直接 dotnet 命令方式

```powershell
# x64 Release
dotnet publish src\AutoScrew.Hmi\AutoScrew.Hmi.csproj -c Release -r win-x64 -o publish\win-x64\Release --self-contained

# x86 Release
dotnet publish src\AutoScrew.Hmi\AutoScrew.Hmi.csproj -c Release -r win-x86 -o publish\win-x86\Release --self-contained

# Debug with symbols
dotnet publish src\AutoScrew.Hmi\AutoScrew.Hmi.csproj -c Debug -r win-x64 -o publish\win-x64\Debug --self-contained /p:DebugType=embedded
```

## 运行时选择

| RID | 说明 | 兼容性 |
|-----|------|--------|
| `win-x64` | 64-bit Windows | 推荐用于现代 PC |
| `win-x86` | 32-bit Windows | 兼容老旧系统 |
| `win-arm64` | Windows ARM64 | Surface Pro X 等 ARM 设备 |

## 优化选项

### 减小包大小

已启用：`PublishTrimmed=true`（移除未使用代码）

可选增强：

```powershell
# 单文件 + 压缩（最小化包大小）
dotnet publish src\AutoScrew.Hmi\AutoScrew.Hmi.csproj `
  -c Release -r win-x64 `
  /p:PublishSingleFile=true `
  /p:PublishCompressed=true

# 预编译为本地代码（已启用，提高启动速度）
# /p:PublishReadyToRun=true （已在项目配置中）
```

### 加快启动速度

已启用：`PublishReadyToRun=true`（预编译为本地代码）

效果：启动时间缩短 30-50%，代价是包大小增加 10-15%

## 故障排除

| 问题 | 解决方案 |
|------|--------|
| 找不到脚本 | 确保在项目根目录运行脚本 |
| 权限被拒绝 | 运行 `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| 发布失败 | 运行 `dotnet clean` 后重试 |
| 包太大 | 启用 `PublishTrimmed` 和 `PublishSingleFile` |
| 运行时错误 | 检查 appsettings.json 配置文件 |

## 文件清单

| 文件 | 用途 |
|------|------|
| `scripts/publish.ps1` | PowerShell 发布脚本 |
| `scripts/publish.bat` | Batch 发布脚本 |
| `scripts/publish.sh` | Bash 发布脚本 |
| `doc/PUBLISH_GUIDE.md` | 详细文档 |
| `src/AutoScrew.Hmi/AutoScrew.Hmi.csproj` | 项目配置（已更新） |

## 部署建议

### 单机部署

1. 发布应用：`.\scripts\publish.ps1 -RuntimeId win-x64`
2. 复制 `publish/win-x64/Release/` 文件夹到目标机器
3. 创建快捷方式或批处理脚本启动应用

### 批量部署

使用配置管理工具（如 Chocolatey、SCCM 等）分发发布的文件夹

### 云部署

将发布的文件上传到云存储（Azure Blob、AWS S3 等），用户下载后直接运行

## 性能指标

| 配置 | 大小 | 启动时间 |
|-----|------|--------|
| Release（标准） | ~180 MB | 5-8 秒 |
| Release（PublishTrimmed） | ~100 MB | 5-8 秒 |
| Release（R2R 编译） | ~200 MB | 2-3 秒 |
| Release（R2R + PublishTrimmed） | ~120 MB | 2-3 秒 |

## 获取帮助

详细文档：[doc/PUBLISH_GUIDE.md](../doc/PUBLISH_GUIDE.md)

常见问题：参见 PUBLISH_GUIDE.md 中的"故障排除"部分
