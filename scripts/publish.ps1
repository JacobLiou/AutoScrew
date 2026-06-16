#!/usr/bin/env pwsh
<#
.SYNOPSIS
    AutoScrew HMI 应用发布脚本（自包含部署）

.DESCRIPTION
    将 AutoScrew HMI 应用打包为不依赖 .NET 运行时的独立可执行文件。

.PARAMETER Configuration
    构建配置（Debug 或 Release，默认为 Release）

.PARAMETER RuntimeId
    目标运行时标识符（win-x64, win-x86, win-arm64，默认为 win-x64）

.PARAMETER OutputPath
    发布输出目录（默认为当前目录的 publish 文件夹）

.EXAMPLE
    # 发布 x64 版本（Release）
    .\publish.ps1 -RuntimeId win-x64
    
    # 发布 Debug 版本到自定义目录
    .\publish.ps1 -Configuration Debug -OutputPath C:\publish\debug
    
    # 同时发布多个架构
    .\publish.ps1 -RuntimeId win-x64
    .\publish.ps1 -RuntimeId win-x86
    .\publish.ps1 -RuntimeId win-arm64
#>

param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    
    [ValidateSet('win-x64', 'win-x86', 'win-arm64')]
    [string]$RuntimeId = 'win-x64',
    
    [string]$OutputPath = ''
)

# 获取脚本所在目录
$scriptDir = Split-Path -Parent $MyInvocation.MyCommandPath
$projectRoot = Split-Path -Parent $scriptDir

# 项目路径
$hmiProjectPath = Join-Path $projectRoot "src\AutoScrew.Hmi\AutoScrew.Hmi.csproj"

if (-not (Test-Path $hmiProjectPath)) {
    Write-Error "找不到项目文件：$hmiProjectPath"
    exit 1
}

# 设置默认输出路径
if ([string]::IsNullOrEmpty($OutputPath)) {
    $OutputPath = Join-Path $projectRoot "publish"
}

# 为每个运行时创建子目录
$fullOutputPath = Join-Path $OutputPath $RuntimeId $Configuration

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "AutoScrew HMI 应用发布" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "项目：$hmiProjectPath"
Write-Host "配置：$Configuration"
Write-Host "运行时：$RuntimeId"
Write-Host "输出目录：$fullOutputPath"
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 执行发布
$publishArgs = @(
    "publish",
    $hmiProjectPath,
    "-c", $Configuration,
    "-r", $RuntimeId,
    "-o", $fullOutputPath,
    "--self-contained",
    "/p:DebugType=embedded",
    "/p:DebugSymbols=true"
)

Write-Host "执行命令：dotnet $($publishArgs -join ' ')" -ForegroundColor Yellow
Write-Host ""

& dotnet @publishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "发布失败，退出代码：$LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "✓ 发布成功！" -ForegroundColor Green
Write-Host "输出位置：$fullOutputPath" -ForegroundColor Green
Write-Host ""

# 显示输出目录大小
$outputSize = Get-ChildItem -Path $fullOutputPath -Recurse -File | 
    Measure-Object -Property Length -Sum | 
    Select-Object -ExpandProperty Sum

if ($outputSize) {
    $sizeMB = [math]::Round($outputSize / 1MB, 2)
    Write-Host "包体大小：$sizeMB MB" -ForegroundColor Green
}

Write-Host ""
Write-Host "可执行文件：$fullOutputPath\AutoScrew.Hmi.exe" -ForegroundColor Cyan
