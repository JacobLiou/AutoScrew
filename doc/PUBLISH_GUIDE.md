# AutoScrew HMI 自包含发布指南

## 概述

本指南说明如何使用 `dotnet publish` 命令将 AutoScrew HMI 应用打包为不依赖 .NET 运行时的独立可执行文件。

## 关键配置

### 项目文件配置 (AutoScrew.Hmi.csproj)

```xml
<PropertyGroup>
  <!-- 自包含发布配置 -->
  <RuntimeIdentifiers>win-x64;win-x86;win-arm64</RuntimeIdentifiers>
  <SelfContained>true</SelfContained>
  <PublishTrimmed>true</PublishTrimmed>
  <PublishReadyToRun>true</PublishReadyToRun>
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
</PropertyGroup>
```

### 配置说明

| 配置项 | 说明 |
|--------|------|
| `RuntimeIdentifiers` | 支持的运行时标识符 (win-x64, win-x86, win-arm64) |
| `SelfContained` | 自包含部署 - 包含 .NET 运行时 |
| `PublishTrimmed` | 修剪未使用的代码，减小包大小 |
| `PublishReadyToRun` | 预编译为本地代码，提高启动性能 |
| `IncludeNativeLibrariesForSelfExtract` | 包含本地库 |

## 快速开始

### 方法 1：使用发布脚本（推荐）

```powershell
# 发布 x64 版本（Release）
.\scripts\publish.ps1 -RuntimeId win-x64

# 发布 Debug 版本
.\scripts\publish.ps1 -Configuration Debug -RuntimeId win-x64

# 发布到自定义目录
.\scripts\publish.ps1 -RuntimeId win-x64 -OutputPath "D:\MyPublish"

# 同时发布多个架构
.\scripts\publish.ps1 -RuntimeId win-x64
.\scripts\publish.ps1 -RuntimeId win-x86
.\scripts\publish.ps1 -RuntimeId win-arm64
```

### 方法 2：直接使用 dotnet 命令

#### 发布 x64 Release 版本

```bash
dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
  -c Release `
  -r win-x64 `
  -o ./publish/win-x64/Release `
  --self-contained
```

#### 发布 x86 Release 版本

```bash
dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
  -c Release `
  -r win-x86 `
  -o ./publish/win-x86/Release `
  --self-contained
```

#### 发布 ARM64 Release 版本

```bash
dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
  -c Release `
  -r win-arm64 `
  -o ./publish/win-arm64/Release `
  --self-contained
```

#### 发布 Debug 版本（含符号）

```bash
dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
  -c Debug `
  -r win-x64 `
  -o ./publish/win-x64/Debug `
  --self-contained `
  /p:DebugType=embedded `
  /p:DebugSymbols=true
```

## 发布输出

发布完成后，输出目录包含：

```
publish/
├── win-x64/
│   ├── Release/
│   │   ├── AutoScrew.Hmi.exe          # 主可执行文件
│   │   ├── AutoScrew.Hmi.dll          # 主程序集
│   │   ├── dotnet.exe                 # .NET 运行时
│   │   ├── appsettings.json           # 配置文件
│   │   └── [其他依赖项和运行时文件]
│   └── Debug/
└── win-x86/
    └── Release/
        └── [类似结构]
```

## 包大小优化

### 当前配置优化

- **PublishTrimmed=true**：移除未使用的代码
- **PublishReadyToRun=true**：预编译为本地代码，增加 ~10-15% 大小但改善启动性能

### 进一步优化建议

#### 1. 启用单文件发布

```bash
dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
  -c Release `
  -r win-x64 `
  -o ./publish `
  /p:PublishSingleFile=true `
  /p:SelfContained=true
```

#### 2. 压缩输出

```bash
dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
  -c Release `
  -r win-x64 `
  -o ./publish `
  /p:PublishSingleFile=true `
  /p:PublishCompressed=true `
  /p:SelfContained=true
```

#### 3. 禁用即时编译（JIT）符号

```bash
/p:DebugSymbols=false
```

### 典型包大小

| 配置 | 大小 |
|-----|------|
| Release（标准） | ~150-200 MB |
| Release + PublishTrimmed | ~100-150 MB |
| Release + PublishTrimmed + PublishSingleFile | ~80-120 MB |
| Release + PublishTrimmed + PublishSingleFile + PublishCompressed | ~50-80 MB |

## 部署和运行

### 在目标机器上运行

```bash
# 进入发布目录
cd publish\win-x64\Release

# 运行应用（无需安装 .NET 运行时）
.\AutoScrew.Hmi.exe
```

### 创建快捷方式

```powershell
# PowerShell 脚本：创建桌面快捷方式
$targetPath = "C:\Path\To\publish\win-x64\Release\AutoScrew.Hmi.exe"
$shortcutPath = [System.Environment]::GetFolderPath('Desktop') + '\AutoScrew HMI.lnk'

$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $targetPath
$shortcut.WorkingDirectory = Split-Path $targetPath
$shortcut.IconLocation = $targetPath
$shortcut.Save()

Write-Host "快捷方式已创建：$shortcutPath"
```

## 故障排除

### 发布失败

**问题**：`error : No runtime(s) found matching: win-x64`

**解决**：
```bash
# 查看可用运行时
dotnet --list-runtimes

# 确保系统已安装对应 .NET 版本
dotnet --version
```

### 运行时错误

**问题**：发布后在目标机器运行出错

**排查步骤**：
1. 检查 .NET 版本兼容性
2. 查看应用日志（Logs/ 目录）
3. 验证配置文件（appsettings.json）存在
4. 检查权限和依赖

### 包过大

**优化方案**：
1. 启用 PublishTrimmed
2. 启用 PublishReadyToRun
3. 考虑单文件发布 + 压缩
4. 审查项目依赖（移除不必要的包）

## CI/CD 集成示例

### GitHub Actions

```yaml
name: Publish AutoScrew HMI

on:
  push:
    branches: [ main, release/* ]

jobs:
  publish:
    runs-on: windows-latest
    strategy:
      matrix:
        rid: [win-x64, win-x86, win-arm64]
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Publish
      run: |
        dotnet publish src/AutoScrew.Hmi/AutoScrew.Hmi.csproj `
          -c Release `
          -r ${{ matrix.rid }} `
          -o ./publish/${{ matrix.rid }}
    
    - name: Upload artifacts
      uses: actions/upload-artifact@v3
      with:
        name: AutoScrew-HMI-${{ matrix.rid }}
        path: ./publish/${{ matrix.rid }}/Release/
```

## 参考资源

- [.NET 自包含应用部署](https://learn.microsoft.com/en-us/dotnet/core/deploying/self-contained)
- [dotnet publish 文档](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)
- [运行时标识符 (RID)](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)
- [应用修剪](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained)

## 常见选项汇总

| 选项 | 说明 | 示例 |
|------|------|------|
| `-c, --configuration` | 构建配置 | `-c Release` |
| `-r, --runtime` | 运行时标识符 | `-r win-x64` |
| `-o, --output` | 输出目录 | `-o ./publish` |
| `--self-contained` | 自包含部署 | 必需 |
| `/p:PublishTrimmed=true` | 启用修剪 | MSBuild 属性 |
| `/p:PublishReadyToRun=true` | 启用 R2R 编译 | 提高启动速度 |
| `/p:PublishSingleFile=true` | 单文件部署 | 简化分发 |
| `/p:PublishCompressed=true` | 压缩输出 | 减小包大小 |
