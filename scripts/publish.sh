#!/bin/bash
# AutoScrew HMI 发布脚本（Bash 版本）

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# 项目配置
HMI_PROJECT="$PROJECT_ROOT/src/AutoScrew.Hmi/AutoScrew.Hmi.csproj"
CONFIGURATION="${1:-Release}"
RUNTIME_ID="${2:-win-x64}"
OUTPUT_BASE="${3:-$PROJECT_ROOT/publish}"

# 设置完整输出路径
OUTPUT_PATH="$OUTPUT_BASE/$RUNTIME_ID/$CONFIGURATION"

# 检查项目文件
if [ ! -f "$HMI_PROJECT" ]; then
    echo "错误：找不到项目文件 $HMI_PROJECT"
    exit 1
fi

echo ""
echo "========================================"
echo "AutoScrew HMI 应用发布"
echo "========================================"
echo "项目：$HMI_PROJECT"
echo "配置：$CONFIGURATION"
echo "运行时：$RUNTIME_ID"
echo "输出目录：$OUTPUT_PATH"
echo "========================================"
echo ""

# 执行发布
echo "执行发布..."
dotnet publish "$HMI_PROJECT" \
    -c "$CONFIGURATION" \
    -r "$RUNTIME_ID" \
    -o "$OUTPUT_PATH" \
    --self-contained \
    /p:DebugType=embedded \
    /p:DebugSymbols=true

if [ $? -ne 0 ]; then
    echo ""
    echo "错误：发布失败"
    exit 1
fi

echo ""
echo "✓ 发布成功！"
echo "输出位置：$OUTPUT_PATH"
echo ""
echo "可执行文件：$OUTPUT_PATH/AutoScrew.Hmi.exe"
echo ""
