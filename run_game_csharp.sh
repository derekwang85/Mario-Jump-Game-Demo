#!/bin/bash
# 马里奥跑酷游戏启动脚本 (C# 版本)

echo "🎮 启动马里奥跑酷大冒险 (C# 版本)..."
echo ""

# 检查 .NET SDK 是否已安装
if ! command -v dotnet &> /dev/null; then
    echo "❌ 错误: 未找到 .NET SDK"
    echo "请先安装 .NET 6.0 或更高版本:"
    echo "  macOS: brew install --cask dotnet-sdk"
    echo "  或访问: https://dotnet.microsoft.com/download"
    exit 1
fi

# 显示 .NET 版本
echo "✅ .NET 版本: $(dotnet --version)"
echo ""

# 还原 NuGet 包
if [ ! -d "obj" ] || [ ! -d "bin" ]; then
    echo "📦 首次运行,正在还原 NuGet 包..."
    dotnet restore
    echo ""
fi

# 编译项目
echo "🔨 正在编译项目..."
dotnet build --configuration Release
if [ $? -ne 0 ]; then
    echo "❌ 编译失败,请检查错误信息"
    exit 1
fi
echo "✅ 编译成功!"
echo ""

# 运行游戏
echo "🚀 游戏启动中..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "操作提示:"
echo "  空格/↑ : 跳跃"
echo "  R      : 重新开始"
echo "  Q/ESC  : 退出游戏"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

dotnet run --configuration Release
