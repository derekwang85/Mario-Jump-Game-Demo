using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarioRunner
{
    /// <summary>
    /// 游戏对象基类
    /// </summary>
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public bool IsActive { get; set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y
        );

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }

    /// <summary>
    /// 玩家类 - 马里奥风格
    /// </summary>
    public class Player : GameObject
    {
        private const float Gravity = 0.8f;
        private const float JumpStrength = -15f;
        private float velocityY = 0;
        private bool isJumping = false;
        private Texture2D texture;

        public Player(GraphicsDevice graphicsDevice)
        {
            Size = new Vector2(50, 60);
            Position = new Vector2(100, 400);
            CreateTexture(graphicsDevice);
        }

        private void CreateTexture(GraphicsDevice graphicsDevice)
        {
            int width = (int)Size.X;
            int height = (int)Size.Y;
            texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            // 填充透明背景
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // 绘制马里奥角色
            // 帽子(红色)
            DrawRect(data, width, 10, 5, 30, 10, Color.Red);
            
            // 脸部(肤色)
            DrawEllipse(data, width, 25, 22, 13, 10, new Color(255, 220, 177));
            
            // 眼睛
            DrawCircle(data, width, 20, 20, 3, Color.Black);
            DrawCircle(data, width, 30, 20, 3, Color.Black);
            
            // 胡子
            DrawRect(data, width, 18, 26, 14, 4, Color.Brown);
            
            // 身体(蓝色上衣)
            DrawRect(data, width, 15, 32, 20, 18, Color.Blue);
            
            // 背带裤(红色)
            DrawRect(data, width, 15, 50, 8, 10, Color.Red);
            DrawRect(data, width, 27, 50, 8, 10, Color.Red);
            
            // 腿(蓝色)
            DrawRect(data, width, 12, 50, 12, 8, new Color(0, 100, 200));
            DrawRect(data, width, 26, 50, 12, 8, new Color(0, 100, 200));
            
            // 鞋子(棕色)
            DrawEllipse(data, width, 17, 57, 7, 3, Color.Brown);
            DrawEllipse(data, width, 33, 57, 7, 3, Color.Brown);

            texture.SetData(data);
        }

        public void Jump()
        {
            if (!isJumping)
            {
                velocityY = JumpStrength;
                isJumping = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // 应用重力
            velocityY += Gravity;
            Position = new Vector2(Position.X, Position.Y + velocityY);

            // 地面碰撞检测
            if (Position.Y >= 440)
            {
                Position = new Vector2(Position.X, 440);
                velocityY = 0;
                isJumping = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }

        // 辅助绘制方法
        private void DrawRect(Color[] data, int width, int x, int y, int w, int h, Color color)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int px = x + j;
                    int py = y + i;
                    if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                        data[py * width + px] = color;
                }
            }
        }

        private void DrawCircle(Color[] data, int width, int cx, int cy, int radius, Color color)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                            data[py * width + px] = color;
                    }
                }
            }
        }

        private void DrawEllipse(Color[] data, int width, int cx, int cy, int rx, int ry, Color color)
        {
            for (int y = -ry; y <= ry; y++)
            {
                for (int x = -rx; x <= rx; x++)
                {
                    if (x * x * ry * ry + y * y * rx * rx <= rx * rx * ry * ry)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                            data[py * width + px] = color;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 敌人类
    /// </summary>
    public class Enemy : GameObject
    {
        public enum EnemyType { Turtle, Rabbit, Mushroom }
        
        private EnemyType type;
        private Texture2D texture;
        private const float ScrollSpeed = 5f;
        public bool HasPassed { get; set; } = false;

        public Enemy(GraphicsDevice graphicsDevice, EnemyType enemyType, float xPosition)
        {
            type = enemyType;
            
            switch (type)
            {
                case EnemyType.Turtle:
                    Size = new Vector2(40, 35);
                    break;
                case EnemyType.Rabbit:
                    Size = new Vector2(40, 45);
                    break;
                case EnemyType.Mushroom:
                    Size = new Vector2(40, 38);
                    break;
            }

            Position = new Vector2(xPosition, 500 - Size.Y);
            CreateTexture(graphicsDevice);
        }

        private void CreateTexture(GraphicsDevice graphicsDevice)
        {
            int width = (int)Size.X;
            int height = (int)Size.Y;
            texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            // 填充透明背景
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            switch (type)
            {
                case EnemyType.Turtle:
                    DrawTurtle(data, width, height);
                    break;
                case EnemyType.Rabbit:
                    DrawRabbit(data, width, height);
                    break;
                case EnemyType.Mushroom:
                    DrawMushroom(data, width, height);
                    break;
            }

            texture.SetData(data);
        }

        private void DrawTurtle(Color[] data, int width, int height)
        {
            // 龟壳(绿色)
            DrawEllipse(data, width, 20, 15, 15, 12, new Color(0, 150, 0));
            // 壳纹理(深绿色)
            DrawEllipse(data, width, 20, 15, 12, 10, new Color(0, 100, 0));
            // 头部
            DrawCircle(data, width, 35, 20, 8, new Color(100, 200, 100));
            // 眼睛
            DrawCircle(data, width, 37, 18, 2, Color.Black);
            // 脚
            DrawCircle(data, width, 8, 28, 4, new Color(100, 200, 100));
            DrawCircle(data, width, 32, 28, 4, new Color(100, 200, 100));
        }

        private void DrawRabbit(Color[] data, int width, int height)
        {
            // 身体(白色)
            DrawEllipse(data, width, 20, 30, 10, 10, new Color(230, 230, 230));
            // 头部
            DrawCircle(data, width, 20, 20, 10, new Color(230, 230, 230));
            // 耳朵
            DrawEllipse(data, width, 15, 9, 3, 9, new Color(230, 230, 230));
            DrawEllipse(data, width, 25, 9, 3, 9, new Color(230, 230, 230));
            // 耳朵内部(粉色)
            DrawEllipse(data, width, 15, 9, 2, 6, Color.Pink);
            DrawEllipse(data, width, 25, 9, 2, 6, Color.Pink);
            // 眼睛
            DrawCircle(data, width, 18, 18, 2, Color.Black);
            DrawCircle(data, width, 24, 18, 2, Color.Black);
            // 鼻子
            DrawCircle(data, width, 21, 22, 2, new Color(255, 182, 193));
        }

        private void DrawMushroom(Color[] data, int width, int height)
        {
            // 蘑菇帽(红色)
            DrawEllipse(data, width, 20, 12, 18, 10, Color.Red);
            // 白色斑点
            DrawCircle(data, width, 12, 12, 4, Color.White);
            DrawCircle(data, width, 28, 12, 4, Color.White);
            DrawCircle(data, width, 20, 16, 3, Color.White);
            // 蘑菇茎(白色)
            DrawRect(data, width, 14, 22, 12, 14, new Color(245, 245, 245));
            // 眼睛
            DrawCircle(data, width, 18, 10, 2, Color.Black);
            DrawCircle(data, width, 26, 10, 2, Color.Black);
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X - ScrollSpeed, Position.Y);
            
            // 如果移出屏幕,标记为不活跃
            if (Position.X + Size.X < 0)
                IsActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }

        // 辅助绘制方法
        private void DrawRect(Color[] data, int width, int x, int y, int w, int h, Color color)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int px = x + j;
                    int py = y + i;
                    if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                        data[py * width + px] = color;
                }
            }
        }

        private void DrawCircle(Color[] data, int width, int cx, int cy, int radius, Color color)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                            data[py * width + px] = color;
                    }
                }
            }
        }

        private void DrawEllipse(Color[] data, int width, int cx, int cy, int rx, int ry, Color color)
        {
            for (int y = -ry; y <= ry; y++)
            {
                for (int x = -rx; x <= rx; x++)
                {
                    if (x * x * ry * ry + y * y * rx * rx <= rx * rx * ry * ry)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                            data[py * width + px] = color;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 云朵装饰类
    /// </summary>
    public class Cloud : GameObject
    {
        private Texture2D texture;

        public Cloud(GraphicsDevice graphicsDevice, float x, float y)
        {
            Size = new Vector2(80, 40);
            Position = new Vector2(x, y);
            CreateTexture(graphicsDevice);
        }

        private void CreateTexture(GraphicsDevice graphicsDevice)
        {
            int width = (int)Size.X;
            int height = (int)Size.Y;
            texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // 绘制云朵
            DrawEllipse(data, width, 15, 25, 15, 12, Color.White);
            DrawEllipse(data, width, 40, 20, 17, 15, Color.White);
            DrawEllipse(data, width, 60, 25, 15, 12, Color.White);

            texture.SetData(data);
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X - 2, Position.Y);
            
            if (Position.X + Size.X < 0)
            {
                Position = new Vector2(1000, new Random().Next(50, 200));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }

        private void DrawEllipse(Color[] data, int width, int cx, int cy, int rx, int ry, Color color)
        {
            for (int y = -ry; y <= ry; y++)
            {
                for (int x = -rx; x <= rx; x++)
                {
                    if (x * x * ry * ry + y * y * rx * rx <= rx * rx * ry * ry)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                            data[py * width + px] = color;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 灌木装饰类
    /// </summary>
    public class Bush : GameObject
    {
        private Texture2D texture;
        private const float ScrollSpeed = 5f;

        public Bush(GraphicsDevice graphicsDevice, float x)
        {
            Size = new Vector2(60, 30);
            Position = new Vector2(x, 480);
            CreateTexture(graphicsDevice);
        }

        private void CreateTexture(GraphicsDevice graphicsDevice)
        {
            int width = (int)Size.X;
            int height = (int)Size.Y;
            texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // 绘制灌木
            Color bushColor = new Color(0, 180, 0);
            DrawEllipse(data, width, 10, 20, 10, 10, bushColor);
            DrawEllipse(data, width, 30, 17, 12, 12, bushColor);
            DrawEllipse(data, width, 50, 20, 10, 10, bushColor);

            texture.SetData(data);
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X - ScrollSpeed, Position.Y);
            
            if (Position.X + Size.X < 0)
            {
                Position = new Vector2(1000 + new Random().Next(0, 300), Position.Y);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }

        private void DrawEllipse(Color[] data, int width, int cx, int cy, int rx, int ry, Color color)
        {
            for (int y = -ry; y <= ry; y++)
            {
                for (int x = -rx; x <= rx; x++)
                {
                    if (x * x * ry * ry + y * y * rx * rx <= rx * rx * ry * ry)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < width && py >= 0 && py < data.Length / width)
                            data[py * width + px] = color;
                    }
                }
            }
        }
    }
}
