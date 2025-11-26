#!/bin/bash
# 马里奥跑酷游戏启动脚本

echo "🎮 启动马里奥跑酷大冒险..."
echo ""

# 检查虚拟环境是否存在
if [ ! -d "venv" ]; then
    echo "📦 首次运行,正在创建虚拟环境..."
    python3 -m venv venv
fi

# 激活虚拟环境
source venv/bin/activate

# 检查pygame是否已安装
if ! python -c "import pygame" &> /dev/null; then
    echo "📥 正在安装游戏依赖(Pygame)..."
    pip install -r requirements.txt
    echo "✅ 依赖安装完成!"
    echo ""
fi

# 运行游戏
echo "🚀 游戏启动中..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "操作提示:"
echo "  空格/↑ : 跳跃"
echo "  R      : 重新开始"
echo "  Q      : 退出游戏"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

python mario_runner.py
