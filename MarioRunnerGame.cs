using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarioRunner
{
    /// <summary>
    /// 马里奥跑酷游戏主类
    /// </summary>
    public class MarioRunnerGame : Game
    {
        // 常量定义
        private const int ScreenWidth = 1000;
        private const int ScreenHeight = 600;
        private const int WinScore = 10;

        // 图形相关
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _fontLarge;
        private SpriteFont _fontMedium;
        private SpriteFont _fontSmall;
        private Texture2D _pixelTexture;

        // 游戏对象
        private Player _player;
        private List<Enemy> _enemies;
        private List<Cloud> _clouds;
        private List<Bush> _bushes;

        // 游戏状态
        private int _score;
        private bool _gameOver;
        private bool _gameWon;
        private int _enemySpawnTimer;
        private float _backgroundOffset;
        private Random _random;

        // 输入状态
        private KeyboardState _previousKeyboardState;

        public MarioRunnerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            _random = new Random();
        }

        protected override void Initialize()
        {
            Window.Title = "马里奥跑酷大冒险";
            base.Initialize();
            ResetGame();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // 创建像素纹理用于绘制基本形状
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // 创建字体(使用默认系统字体)
            CreateFonts();
        }

        private void CreateFonts()
        {
            // 由于MonoGame需要预编译字体,这里我们创建简单的调试字体
            // 在实际项目中,应该使用Content Pipeline创建字体
            try
            {
                // 尝试加载字体,如果失败则使用备用方案
                // _fontLarge = Content.Load<SpriteFont>("FontLarge");
            }
            catch
            {
                // 备用方案:不使用字体,使用简单的文本绘制
            }
        }

        private void ResetGame()
        {
            _score = 0;
            _gameOver = false;
            _gameWon = false;
            _enemySpawnTimer = 0;
            _backgroundOffset = 0;

            // 创建玩家
            _player = new Player(GraphicsDevice);

            // 初始化列表
            _enemies = new List<Enemy>();
            _clouds = new List<Cloud>();
            _bushes = new List<Bush>();

            // 创建初始云朵
            for (int i = 0; i < 5; i++)
            {
                _clouds.Add(new Cloud(GraphicsDevice, 
                    _random.Next(0, ScreenWidth), 
                    _random.Next(50, 200)));
            }

            // 创建初始灌木
            for (int i = 0; i < 4; i++)
            {
                _bushes.Add(new Bush(GraphicsDevice, _random.Next(0, ScreenWidth)));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // 退出游戏
            if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Q))
                Exit();

            // 游戏结束后重新开始
            if ((_gameOver || _gameWon) && keyboardState.IsKeyDown(Keys.R) && 
                _previousKeyboardState.IsKeyUp(Keys.R))
            {
                ResetGame();
            }

            // 游戏进行中的更新
            if (!_gameOver && !_gameWon)
            {
                // 跳跃控制
                if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up)) &&
                    (_previousKeyboardState.IsKeyUp(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Up)))
                {
                    _player.Jump();
                }

                // 更新背景偏移
                _backgroundOffset += 5f;

                // 更新玩家
                _player.Update(gameTime);

                // 更新云朵
                foreach (var cloud in _clouds)
                    cloud.Update(gameTime);

                // 更新灌木
                foreach (var bush in _bushes)
                    bush.Update(gameTime);

                // 生成敌人
                _enemySpawnTimer++;
                if (_enemySpawnTimer > _random.Next(60, 120))
                {
                    SpawnEnemy();
                    _enemySpawnTimer = 0;
                }

                // 更新敌人
                foreach (var enemy in _enemies)
                    enemy.Update(gameTime);

                // 移除不活跃的敌人
                _enemies.RemoveAll(e => !e.IsActive);

                // 检查碰撞
                CheckCollisions();

                // 检查得分
                CheckScore();
            }

            _previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        private void SpawnEnemy()
        {
            Enemy.EnemyType type = (Enemy.EnemyType)_random.Next(0, 3);
            _enemies.Add(new Enemy(GraphicsDevice, type, ScreenWidth + _random.Next(50, 200)));
        }

        private void CheckCollisions()
        {
            foreach (var enemy in _enemies)
            {
                if (_player.Bounds.Intersects(enemy.Bounds))
                {
                    _gameOver = true;
                    break;
                }
            }
        }

        private void CheckScore()
        {
            foreach (var enemy in _enemies)
            {
                if (!enemy.HasPassed && enemy.Position.X + enemy.Size.X < _player.Position.X)
                {
                    enemy.HasPassed = true;
                    _score++;

                    if (_score >= WinScore)
                    {
                        _gameWon = true;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(135, 206, 235)); // 天空蓝

            _spriteBatch.Begin();

            // 绘制地面
            DrawGround();

            // 绘制云朵
            foreach (var cloud in _clouds)
                cloud.Draw(_spriteBatch);

            // 绘制灌木
            foreach (var bush in _bushes)
                bush.Draw(_spriteBatch);

            // 绘制玩家
            _player.Draw(_spriteBatch);

            // 绘制敌人
            foreach (var enemy in _enemies)
                enemy.Draw(_spriteBatch);

            // 绘制UI
            DrawUI();

            // 绘制游戏结束或胜利画面
            if (_gameOver)
                DrawGameOver();
            else if (_gameWon)
                DrawVictory();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGround()
        {
            // 草地
            _spriteBatch.Draw(_pixelTexture, 
                new Rectangle(0, 500, ScreenWidth, 100), 
                new Color(34, 139, 34));

            // 地面纹理线条
            int offset = (int)(_backgroundOffset % 50);
            for (int i = 0; i < ScreenWidth; i += 50)
            {
                _spriteBatch.Draw(_pixelTexture,
                    new Rectangle(i - offset, 500, 48, 5),
                    new Color(30, 120, 30));
            }
        }

        private void DrawUI()
        {
            // 分数显示
            DrawText($"分数: {_score}/{WinScore}", new Vector2(20, 20), Color.Black, 2.0f);
            
            // 操作提示
            DrawText("空格/↑ 跳跃", new Vector2(20, 70), Color.Black, 1.2f);
        }

        private void DrawGameOver()
        {
            // 半透明黑色遮罩
            _spriteBatch.Draw(_pixelTexture, 
                new Rectangle(0, 0, ScreenWidth, ScreenHeight), 
                new Color(0, 0, 0, 200));

            // 游戏失败文字
            DrawTextCentered("游戏失败!", new Vector2(ScreenWidth / 2, 200), Color.Red, 3.0f);
            DrawTextCentered($"最终分数: {_score}", new Vector2(ScreenWidth / 2, 300), Color.White, 2.0f);
            DrawTextCentered("按 R 重新开始 | 按 Q 退出", new Vector2(ScreenWidth / 2, 400), Color.Yellow, 1.5f);
        }

        private void DrawVictory()
        {
            // 半透明黑色遮罩
            _spriteBatch.Draw(_pixelTexture,
                new Rectangle(0, 0, ScreenWidth, ScreenHeight),
                new Color(0, 0, 0, 200));

            // 胜利文字
            DrawTextCentered("恭喜通关!", new Vector2(ScreenWidth / 2, 200), Color.Yellow, 3.0f);
            DrawTextCentered("你成功躲避了10个敌人!", new Vector2(ScreenWidth / 2, 300), Color.White, 2.0f);
            DrawTextCentered("按 R 重新开始 | 按 Q 退出", new Vector2(ScreenWidth / 2, 400), Color.Yellow, 1.5f);
        }

        // 简单的文本绘制方法(使用矩形模拟文字,因为没有预编译字体)
        private void DrawText(string text, Vector2 position, Color color, float scale)
        {
            // 这是一个简化版本,实际应该使用SpriteFont
            // 由于MonoGame需要Content Pipeline,这里使用简单的矩形表示文字位置
            int charWidth = (int)(8 * scale);
            int charHeight = (int)(12 * scale);
            
            for (int i = 0; i < text.Length; i++)
            {
                _spriteBatch.Draw(_pixelTexture,
                    new Rectangle((int)position.X + i * charWidth, (int)position.Y, charWidth - 2, charHeight),
                    color * 0.3f);
            }
        }

        private void DrawTextCentered(string text, Vector2 position, Color color, float scale)
        {
            int charWidth = (int)(8 * scale);
            int totalWidth = text.Length * charWidth;
            Vector2 centeredPos = new Vector2(position.X - totalWidth / 2, position.Y);
            DrawText(text, centeredPos, color, scale);
        }
    }
}
