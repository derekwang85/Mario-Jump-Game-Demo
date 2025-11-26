# 马里奥跑酷大冒险 (C# 版本) 🎮

一款使用C#和MonoGame框架开发的2D横版跑酷游戏,完全重写自Python版本。

## 游戏特色

- 🏃 马里奥风格的主角设计
- 🐢 三种可爱的敌人:乌龟、兔子、蘑菇
- 🌈 自动滚动的背景效果
- ⭐ 简单有趣的计分系统
- 🎯 通关目标:躲避10个敌人
- 💻 使用C#和MonoGame框架开发
- 🎨 所有图形都是代码绘制,无需额外资源文件

## 游戏玩法

### 操作方式
- **空格键** 或 **↑方向键**: 跳跃
- **R键**: 重新开始(游戏结束后)
- **Q键** 或 **ESC键**: 退出游戏

### 游戏规则
1. 主角自动向前跑动,背景自动滚动
2. 敌人从右侧不断出现
3. 使用跳跃躲避敌人
4. 每成功躲避1个敌人得1分
5. 碰到敌人即游戏失败
6. 累计躲避10个敌人即通关胜利

## 技术栈

- **语言**: C# (.NET 6.0)
- **框架**: MonoGame 3.8.1
- **平台**: 跨平台(Windows, macOS, Linux)
- **架构**: 面向对象设计

## 环境要求

### Windows
- .NET 6.0 SDK 或更高版本
- Visual Studio 2022 或 JetBrains Rider

### macOS
- .NET 6.0 SDK 或更高版本
- Visual Studio for Mac 或 JetBrains Rider
- Mono Framework (MonoGame依赖)

### Linux
- .NET 6.0 SDK 或更高版本
- MonoDevelop 或 JetBrains Rider

## 安装说明

### 1. 安装 .NET SDK

**Windows:**
```bash
# 从官网下载安装
https://dotnet.microsoft.com/download
```

**macOS:**
```bash
# 使用 Homebrew
brew install --cask dotnet-sdk
```

**Linux (Ubuntu/Debian):**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 6.0
```

### 2. 验证安装

```bash
dotnet --version
# 应显示 6.0.x 或更高版本
```

### 3. 还原NuGet包

```bash
cd /path/to/MarioRunner
dotnet restore
```

### 4. 编译项目

```bash
dotnet build
```

### 5. 运行游戏

```bash
dotnet run
```

或者使用提供的脚本:

**macOS/Linux:**
```bash
./run_game_csharp.sh
```

**Windows:**
```bash
run_game_csharp.bat
```

## 项目结构

```
MarioRunner/
├── MarioRunner.csproj      # 项目配置文件
├── Program.cs              # 程序入口
├── MarioRunnerGame.cs      # 游戏主类
├── GameObject.cs           # 游戏对象类
│   ├── Player              # 玩家类
│   ├── Enemy               # 敌人类
│   ├── Cloud               # 云朵类
│   └── Bush                # 灌木类
└── README_CSharp.md        # 本文档
```

## 代码特点

### 面向对象设计
```csharp
// 游戏对象基类
public abstract class GameObject
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}
```

### 三种敌人类型
```csharp
public enum EnemyType 
{ 
    Turtle,    // 🐢 乌龟
    Rabbit,    // 🐰 兔子
    Mushroom   // 🍄 蘑菇
}
```

### 动态图形生成
- 所有游戏角色都是通过代码绘制像素生成
- 不依赖外部图片资源
- 使用Texture2D和Color数组实现

### 碰撞检测
```csharp
if (_player.Bounds.Intersects(enemy.Bounds))
{
    _gameOver = true;
}
```

## 自定义修改

在 `MarioRunnerGame.cs` 中可以修改:

```csharp
// 屏幕大小
private const int ScreenWidth = 1000;
private const int ScreenHeight = 600;

// 通关分数
private const int WinScore = 10;
```

在 `GameObject.cs` 中可以修改:

```csharp
// Player类
private const float Gravity = 0.8f;       // 重力
private const float JumpStrength = -15f;  // 跳跃力度

// Enemy类
private const float ScrollSpeed = 5f;     // 滚动速度
```

## MonoGame框架说明

MonoGame是一个开源的跨平台游戏框架,是XNA Framework的继任者:
- 支持2D和3D游戏开发
- 跨平台:Windows, macOS, Linux, iOS, Android等
- C#语言,性能优秀
- 活跃的社区支持

## 与Python版本的对比

| 特性 | Python + Pygame | C# + MonoGame |
|------|----------------|---------------|
| 语言 | Python | C# |
| 框架 | Pygame | MonoGame |
| 性能 | 中等 | 高 |
| 类型安全 | 动态类型 | 静态类型 |
| 开发速度 | 快 | 中等 |
| 可维护性 | 中等 | 高 |
| 跨平台 | ✅ | ✅ |

## 常见问题

**Q: 编译时提示找不到MonoGame?**  
A: 运行 `dotnet restore` 还原NuGet包

**Q: macOS上运行报错?**  
A: 确保已安装Mono Framework: `brew install mono`

**Q: 如何在Visual Studio中打开?**  
A: 直接打开 `MarioRunner.csproj` 文件

**Q: 游戏窗口显示不正常?**  
A: 检查图形驱动是否最新,尝试更新.NET SDK

**Q: 如何发布独立可执行文件?**  
A: 使用以下命令:
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# macOS
dotnet publish -c Release -r osx-x64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

## 性能优化建议

1. **对象池**: 重用敌人对象而非频繁创建销毁
2. **纹理缓存**: 预先创建所有纹理
3. **批量渲染**: 使用SpriteBatch的批处理功能
4. **避免GC**: 减少临时对象分配

## 扩展功能建议

- [ ] 添加音效系统
- [ ] 实现多关卡设计
- [ ] 添加道具系统(金币、星星)
- [ ] 实现分数排行榜
- [ ] 添加不同难度模式
- [ ] 支持手柄控制
- [ ] 添加动画效果

## 学习资源

- [MonoGame官方文档](https://docs.monogame.net/)
- [C# 编程指南](https://docs.microsoft.com/zh-cn/dotnet/csharp/)
- [游戏开发模式](https://gameprogrammingpatterns.com/)

## 许可证

本项目仅供学习和娱乐使用。

## 作者

高级程序员 - C#游戏开发

---

**享受C#版本的马里奥跑酷,祝你通关成功! 🎉**
