@echo off
REM AutoScrew HMI 发布脚本（Batch 版本）

setlocal enabledelayedexpansion

REM 获取脚本目录
set SCRIPT_DIR=%~dp0
set PROJECT_ROOT=%SCRIPT_DIR%..

REM 项目配置
set HMI_PROJECT=%PROJECT_ROOT%\src\AutoScrew.Hmi\AutoScrew.Hmi.csproj
set CONFIGURATION=Release
set RUNTIME_ID=win-x64
set OUTPUT_BASE=%PROJECT_ROOT%\publish

REM 处理命令行参数
if not "%~1"=="" set CONFIGURATION=%~1
if not "%~2"=="" set RUNTIME_ID=%~2
if not "%~3"=="" set OUTPUT_BASE=%~3

REM 设置完整输出路径
set OUTPUT_PATH=%OUTPUT_BASE%\%RUNTIME_ID%\%CONFIGURATION%

REM 检查项目文件
if not exist "%HMI_PROJECT%" (
    echo 错误：找不到项目文件 %HMI_PROJECT%
    exit /b 1
)

echo.
echo ========================================
echo AutoScrew HMI 应用发布
echo ========================================
echo 项目：%HMI_PROJECT%
echo 配置：%CONFIGURATION%
echo 运行时：%RUNTIME_ID%
echo 输出目录：%OUTPUT_PATH%
echo ========================================
echo.

REM 执行发布
echo 执行发布...
dotnet publish "%HMI_PROJECT%" ^
    -c %CONFIGURATION% ^
    -r %RUNTIME_ID% ^
    -o "%OUTPUT_PATH%" ^
    --self-contained ^
    /p:DebugType=embedded ^
    /p:DebugSymbols=true

if errorlevel 1 (
    echo.
    echo 错误：发布失败
    exit /b 1
)

echo.
echo ✓ 发布成功！
echo 输出位置：%OUTPUT_PATH%
echo.
echo 可执行文件：%OUTPUT_PATH%\AutoScrew.Hmi.exe
echo.

endlocal
