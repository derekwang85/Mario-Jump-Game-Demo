"""
超级马里奥风格横版跑酷游戏
使用方向键控制跳跃，躲避敌人得分
"""

import pygame
import random
import sys
import os

# 初始化Pygame
pygame.init()

# 游戏常量
SCREEN_WIDTH = 1000
SCREEN_HEIGHT = 600
FPS = 60
GRAVITY = 0.8
JUMP_STRENGTH = -15
SCROLL_SPEED = 5

# 颜色定义
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
SKY_BLUE = (135, 206, 235)
GREEN = (34, 139, 34)
RED = (255, 0, 0)
YELLOW = (255, 215, 0)
BROWN = (139, 69, 19)

# 创建游戏窗口
screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
pygame.display.set_caption("马里奥跑酷大冒险")
clock = pygame.time.Clock()

# 字体设置
font_large = pygame.font.Font(None, 74)
font_medium = pygame.font.Font(None, 48)
font_small = pygame.font.Font(None, 36)


class Player(pygame.sprite.Sprite):
    """玩家角色类 - 马里奥风格"""
    
    def __init__(self):
        super().__init__()
        self.width = 50
        self.height = 60
        self.image = pygame.Surface((self.width, self.height), pygame.SRCALPHA)
        self.draw_player()
        self.rect = self.image.get_rect()
        self.rect.x = 100
        self.rect.y = SCREEN_HEIGHT - 150
        self.velocity_y = 0
        self.is_jumping = False
        self.animation_frame = 0
        
    def draw_player(self):
        """绘制马里奥风格的角色"""
        # 帽子(红色)
        pygame.draw.rect(self.image, RED, (10, 5, 30, 10))
        # 脸部(肤色)
        pygame.draw.ellipse(self.image, (255, 220, 177), (12, 12, 26, 20))
        # 眼睛
        pygame.draw.circle(self.image, BLACK, (20, 20), 3)
        pygame.draw.circle(self.image, BLACK, (30, 20), 3)
        # 胡子
        pygame.draw.rect(self.image, BROWN, (18, 26, 14, 4))
        # 身体(蓝色上衣)
        pygame.draw.rect(self.image, (0, 0, 255), (15, 32, 20, 18))
        # 背带裤(红色)
        pygame.draw.rect(self.image, RED, (15, 50, 8, 10))
        pygame.draw.rect(self.image, RED, (27, 50, 8, 10))
        # 腿(蓝色)
        pygame.draw.rect(self.image, (0, 100, 200), (12, 50, 12, 8))
        pygame.draw.rect(self.image, (0, 100, 200), (26, 50, 12, 8))
        # 鞋子(棕色)
        pygame.draw.ellipse(self.image, BROWN, (10, 54, 14, 6))
        pygame.draw.ellipse(self.image, BROWN, (26, 54, 14, 6))
        
    def update(self):
        """更新玩家状态"""
        # 应用重力
        self.velocity_y += GRAVITY
        self.rect.y += self.velocity_y
        
        # 地面碰撞检测
        if self.rect.bottom >= SCREEN_HEIGHT - 100:
            self.rect.bottom = SCREEN_HEIGHT - 100
            self.velocity_y = 0
            self.is_jumping = False
            
    def jump(self):
        """跳跃功能"""
        if not self.is_jumping:
            self.velocity_y = JUMP_STRENGTH
            self.is_jumping = True


class Enemy(pygame.sprite.Sprite):
    """敌人基类"""
    
    def __init__(self, enemy_type, x_pos):
        super().__init__()
        self.enemy_type = enemy_type
        self.width = 40
        self.height = 40
        
        if enemy_type == "turtle":
            self.height = 35
        elif enemy_type == "rabbit":
            self.height = 45
        elif enemy_type == "mushroom":
            self.height = 38
            
        self.image = pygame.Surface((self.width, self.height), pygame.SRCALPHA)
        self.draw_enemy()
        self.rect = self.image.get_rect()
        self.rect.x = x_pos
        self.rect.y = SCREEN_HEIGHT - 100 - self.height
        self.passed = False  # 标记是否已经被躲避过
        
    def draw_enemy(self):
        """根据类型绘制不同的敌人"""
        if self.enemy_type == "turtle":
            # 乌龟壳(绿色)
            pygame.draw.ellipse(self.image, (0, 150, 0), (5, 5, 30, 25))
            # 壳纹理
            pygame.draw.arc(self.image, (0, 100, 0), (8, 8, 24, 20), 0, 3.14, 3)
            # 头部
            pygame.draw.circle(self.image, (100, 200, 100), (35, 20), 8)
            # 眼睛
            pygame.draw.circle(self.image, BLACK, (37, 18), 2)
            # 脚
            pygame.draw.circle(self.image, (100, 200, 100), (8, 28), 4)
            pygame.draw.circle(self.image, (100, 200, 100), (32, 28), 4)
            
        elif self.enemy_type == "rabbit":
            # 身体(白色)
            pygame.draw.ellipse(self.image, (230, 230, 230), (10, 20, 20, 20))
            # 头部
            pygame.draw.circle(self.image, (230, 230, 230), (20, 15), 10)
            # 耳朵
            pygame.draw.ellipse(self.image, (230, 230, 230), (15, 0, 6, 18))
            pygame.draw.ellipse(self.image, (230, 230, 230), (23, 0, 6, 18))
            # 耳朵内部(粉色)
            pygame.draw.ellipse(self.image, (255, 192, 203), (17, 4, 3, 10))
            pygame.draw.ellipse(self.image, (255, 192, 203), (25, 4, 3, 10))
            # 眼睛
            pygame.draw.circle(self.image, BLACK, (18, 14), 2)
            pygame.draw.circle(self.image, BLACK, (24, 14), 2)
            # 鼻子
            pygame.draw.circle(self.image, (255, 182, 193), (21, 18), 2)
            
        elif self.enemy_type == "mushroom":
            # 蘑菇帽(红色带白点)
            pygame.draw.ellipse(self.image, RED, (2, 5, 36, 20))
            # 白色斑点
            pygame.draw.circle(self.image, WHITE, (12, 12), 4)
            pygame.draw.circle(self.image, WHITE, (28, 12), 4)
            pygame.draw.circle(self.image, WHITE, (20, 18), 3)
            # 蘑菇茎(白色)
            pygame.draw.rect(self.image, (245, 245, 245), (14, 22, 12, 14))
            # 眼睛(恶意的表情)
            pygame.draw.circle(self.image, BLACK, (18, 10), 2)
            pygame.draw.circle(self.image, BLACK, (26, 10), 2)
            
    def update(self):
        """更新敌人位置"""
        self.rect.x -= SCROLL_SPEED
        
        # 如果敌人移出屏幕左侧,删除它
        if self.rect.right < 0:
            self.kill()


class Cloud(pygame.sprite.Sprite):
    """云朵类 - 用于背景装饰"""
    
    def __init__(self, x, y):
        super().__init__()
        self.image = pygame.Surface((80, 40), pygame.SRCALPHA)
        # 绘制云朵
        pygame.draw.ellipse(self.image, WHITE, (0, 15, 30, 25))
        pygame.draw.ellipse(self.image, WHITE, (20, 10, 35, 30))
        pygame.draw.ellipse(self.image, WHITE, (45, 15, 30, 25))
        self.rect = self.image.get_rect()
        self.rect.x = x
        self.rect.y = y
        
    def update(self):
        """云朵缓慢移动"""
        self.rect.x -= 2
        if self.rect.right < 0:
            self.rect.x = SCREEN_WIDTH
            self.rect.y = random.randint(50, 200)


class Bush(pygame.sprite.Sprite):
    """灌木类 - 地面装饰"""
    
    def __init__(self, x):
        super().__init__()
        self.image = pygame.Surface((60, 30), pygame.SRCALPHA)
        # 绘制灌木
        pygame.draw.ellipse(self.image, (0, 180, 0), (0, 10, 20, 20))
        pygame.draw.ellipse(self.image, (0, 180, 0), (15, 5, 25, 25))
        pygame.draw.ellipse(self.image, (0, 180, 0), (35, 10, 20, 20))
        self.rect = self.image.get_rect()
        self.rect.x = x
        self.rect.y = SCREEN_HEIGHT - 120
        
    def update(self):
        """灌木随背景移动"""
        self.rect.x -= SCROLL_SPEED
        if self.rect.right < 0:
            self.rect.x = SCREEN_WIDTH + random.randint(0, 300)


class Game:
    """游戏主类"""
    
    def __init__(self):
        self.reset()
        
    def reset(self):
        """重置游戏状态"""
        self.player = Player()
        self.all_sprites = pygame.sprite.Group()
        self.enemies = pygame.sprite.Group()
        self.clouds = pygame.sprite.Group()
        self.bushes = pygame.sprite.Group()
        
        self.all_sprites.add(self.player)
        
        # 创建初始云朵
        for i in range(5):
            cloud = Cloud(random.randint(0, SCREEN_WIDTH), random.randint(50, 200))
            self.clouds.add(cloud)
            self.all_sprites.add(cloud)
            
        # 创建初始灌木
        for i in range(4):
            bush = Bush(random.randint(0, SCREEN_WIDTH))
            self.bushes.add(bush)
            self.all_sprites.add(bush)
        
        self.score = 0
        self.game_over = False
        self.game_won = False
        self.enemy_spawn_timer = 0
        self.background_offset = 0
        
    def spawn_enemy(self):
        """生成随机敌人"""
        enemy_type = random.choice(["turtle", "rabbit", "mushroom"])
        enemy = Enemy(enemy_type, SCREEN_WIDTH + random.randint(50, 200))
        self.enemies.add(enemy)
        self.all_sprites.add(enemy)
        
    def check_score(self):
        """检查是否有敌人被成功躲避"""
        for enemy in self.enemies:
            if not enemy.passed and enemy.rect.right < self.player.rect.left:
                enemy.passed = True
                self.score += 1
                if self.score >= 10:
                    self.game_won = True
                    
    def check_collision(self):
        """检查玩家与敌人的碰撞"""
        if pygame.sprite.spritecollide(self.player, self.enemies, False):
            self.game_over = True
            
    def draw_ground(self):
        """绘制地面"""
        # 草地
        pygame.draw.rect(screen, GREEN, (0, SCREEN_HEIGHT - 100, SCREEN_WIDTH, 100))
        # 地面纹理
        for i in range(0, SCREEN_WIDTH, 50):
            offset = int((self.background_offset % 50))
            pygame.draw.rect(screen, (30, 120, 30), 
                           (i - offset, SCREEN_HEIGHT - 100, 48, 5))
            
    def draw_ui(self):
        """绘制用户界面"""
        # 分数显示
        score_text = font_medium.render(f"分数: {self.score}/10", True, BLACK)
        screen.blit(score_text, (20, 20))
        
        # 操作提示
        hint_text = font_small.render("空格/↑ 跳跃", True, BLACK)
        screen.blit(hint_text, (20, 70))
        
    def show_game_over(self):
        """显示游戏结束画面"""
        overlay = pygame.Surface((SCREEN_WIDTH, SCREEN_HEIGHT))
        overlay.set_alpha(200)
        overlay.fill(BLACK)
        screen.blit(overlay, (0, 0))
        
        game_over_text = font_large.render("游戏失败!", True, RED)
        score_text = font_medium.render(f"最终分数: {self.score}", True, WHITE)
        restart_text = font_small.render("按 R 重新开始 | 按 Q 退出", True, YELLOW)
        
        screen.blit(game_over_text, 
                   (SCREEN_WIDTH // 2 - game_over_text.get_width() // 2, 200))
        screen.blit(score_text, 
                   (SCREEN_WIDTH // 2 - score_text.get_width() // 2, 300))
        screen.blit(restart_text, 
                   (SCREEN_WIDTH // 2 - restart_text.get_width() // 2, 400))
        
    def show_victory(self):
        """显示胜利画面"""
        overlay = pygame.Surface((SCREEN_WIDTH, SCREEN_HEIGHT))
        overlay.set_alpha(200)
        overlay.fill(BLACK)
        screen.blit(overlay, (0, 0))
        
        victory_text = font_large.render("恭喜通关!", True, YELLOW)
        score_text = font_medium.render("你成功躲避了10个敌人!", True, WHITE)
        restart_text = font_small.render("按 R 重新开始 | 按 Q 退出", True, YELLOW)
        
        screen.blit(victory_text, 
                   (SCREEN_WIDTH // 2 - victory_text.get_width() // 2, 200))
        screen.blit(score_text, 
                   (SCREEN_WIDTH // 2 - score_text.get_width() // 2, 300))
        screen.blit(restart_text, 
                   (SCREEN_WIDTH // 2 - restart_text.get_width() // 2, 400))
        
    def run(self):
        """游戏主循环"""
        running = True
        
        while running:
            clock.tick(FPS)
            
            # 事件处理
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                    
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_SPACE or event.key == pygame.K_UP:
                        if not self.game_over and not self.game_won:
                            self.player.jump()
                    
                    if event.key == pygame.K_r:
                        if self.game_over or self.game_won:
                            self.reset()
                            
                    if event.key == pygame.K_q:
                        running = False
            
            # 游戏逻辑更新
            if not self.game_over and not self.game_won:
                self.background_offset += SCROLL_SPEED
                
                # 更新所有精灵
                self.all_sprites.update()
                
                # 生成敌人
                self.enemy_spawn_timer += 1
                if self.enemy_spawn_timer > random.randint(60, 120):
                    self.spawn_enemy()
                    self.enemy_spawn_timer = 0
                    
                # 检查碰撞和得分
                self.check_collision()
                self.check_score()
            
            # 绘制
            screen.fill(SKY_BLUE)
            self.draw_ground()
            
            # 绘制所有精灵(云朵和灌木在后,玩家和敌人在前)
            self.clouds.draw(screen)
            self.bushes.draw(screen)
            screen.blit(self.player.image, self.player.rect)
            self.enemies.draw(screen)
            
            self.draw_ui()
            
            # 显示游戏结束或胜利画面
            if self.game_over:
                self.show_game_over()
            elif self.game_won:
                self.show_victory()
            
            pygame.display.flip()
        
        pygame.quit()
        sys.exit()


def main():
    """主函数"""
    game = Game()
    game.run()


if __name__ == "__main__":
    main()
