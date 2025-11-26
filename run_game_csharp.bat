@echo off
REM 马里奥跑酷游戏启动脚本 (C# 版本 - Windows)

echo 🎮 启动马里奥跑酷大冒险 (C# 版本)...
echo.

REM 检查 .NET SDK 是否已安装
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ 错误: 未找到 .NET SDK
    echo 请先安装 .NET 6.0 或更高版本:
    echo   访问: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM 显示 .NET 版本
echo ✅ .NET 版本:
dotnet --version
echo.

REM 还原 NuGet 包
if not exist "obj\" (
    echo 📦 首次运行,正在还原 NuGet 包...
    dotnet restore
    echo.
)

REM 编译项目
echo 🔨 正在编译项目...
dotnet build --configuration Release
if errorlevel 1 (
    echo ❌ 编译失败,请检查错误信息
    pause
    exit /b 1
)
echo ✅ 编译成功!
echo.

REM 运行游戏
echo 🚀 游戏启动中...
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo 操作提示:
echo   空格/↑ : 跳跃
echo   R      : 重新开始
echo   Q/ESC  : 退出游戏
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo.

dotnet run --configuration Release

pause
