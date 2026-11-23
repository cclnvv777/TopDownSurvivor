# TopDownSurvivor 俯视角动作生存游戏

一款类似《吸血鬼幸存者》的俯视角生存闯关游戏。玩家在场上移动躲避，自动攻击消灭从四周不断涌来的敌人，击杀掉落经验、升级三选一强化，尽可能存活更久。项目重点在于**代码架构**与**性能优化**。

## 技术亮点

- **对象池（Object Pool）**：子弹、敌人都复用，避免频繁 `Instantiate`/`Destroy` 导致的 GC 卡顿（性能优化核心）
- **敌人追击 AI（Seek）**：通过向量相减 + 归一化实现追踪行为
- **刷怪系统**：使用协程（Coroutine）定时从玩家四周生成敌人，难度随时间递增（刷怪加快、敌人变快）
- **多种敌人类型**：普通怪、精英怪（血厚伤害高）、远程怪（保持距离射击），基于继承 + 重写实现
- **自动攻击**：遍历敌人查找最近目标（线性求最小值算法）
- **事件系统（EventBus）**：泛型类型安全事件总线，敌人死亡、玩家死亡事件解耦多个系统
- **单例管理器（Singleton）**：懒加载 MonoBehaviour 单例基类
- **升级系统**：三选一强化（攻速/攻击力/移速/生命上限/经验获取/环绕刀刃/散射），用洗牌算法去重，代码动态创建 UI
- **多武器**：自动射击（可散射）+ 环绕刀刃（刀刃绕玩家旋转）
- **战斗反馈**：伤害数字飘字、受伤无敌帧（闪烁）、屏幕震动
- **UI 系统**：血条、HUD、菜单、结算全部代码动态创建，不依赖手动拖拽
- **音效系统**：AudioManager 单例 + 事件触发
- **存档系统**：PlayerPrefs 保存最高存活时间
- **一键搭建场景**：编辑器脚本自动生成所有对象、自动设置引用、自动保存场景

## 已完成功能

- [x] 玩家八方向移动（WASD，斜向移动不加速）
- [x] 敌人追击 AI + 刷怪系统
- [x] 自动攻击 + 子弹对象池 + 敌人对象池
- [x] 碰撞检测 + 伤害判定
- [x] 玩家生命值 + 敌人碰撞伤害 + 死亡
- [x] 击杀掉落经验宝石 + 自动拾取
- [x] 升级系统（三选一强化面板）
- [x] 难度随时间递增（刷怪加快 / 敌人变快）
- [x] 多种敌人类型（普通 / 精英 / 远程）
- [x] 经验宝石对象池 + 自动吸附
- [x] 伤害数字飘字 / 玩家无敌帧 / 屏幕震动 / 散射子弹
- [x] 敌人死亡特效 + 暂停菜单（ESC）
- [x] 更多强化（生命上限 / 经验获取 / 环绕刀刃）
- [x] 环绕刀刃武器（绕玩家旋转）
- [x] 血条数字显示
- [x] HUD（血条 / 存活时间 / 击杀数）
- [x] 开始菜单 + 死亡结算 + 重新开始
- [x] 音效 + 存档（最高分）
- [ ] 打包发布

## 目录结构

```
Assets/
├── Scripts/
│   ├── Core/       # 核心框架（单例 / 事件总线 / 对象池 / 音效 / 存档）
│   ├── Player/     # 玩家（移动 / 自动攻击 / 经验 / 生命值）
│   ├── Enemies/    # 敌人（追击 AI / 刷怪器）
│   ├── Weapons/    # 武器（子弹）
│   └── Systems/    # 系统（经验掉落 / 升级 / HUD / 菜单）
├── Editor/         # 编辑器工具（一键搭建场景）
├── Prefabs/        # 预制体（子弹 / 敌人 / 经验宝石）
├── Sprites/        # 占位精灵
└── Scenes/         # 场景
```

## 核心脚本说明

| 文件 | 作用 |
|---|---|
| Core/Singleton.cs | MonoBehaviour 单例基类（懒加载） |
| Core/EventBus.cs | 泛型事件总线，模块解耦 |
| Core/GameManager.cs | 游戏状态管理 |
| Core/ObjectPool.cs | 通用对象池（队列） |
| Core/AudioManager.cs | 音效单例 + 事件订阅播放 |
| Core/SaveSystem.cs | 存档（PlayerPrefs 最高分） |
| Player/PlayerMovement.cs | 八方向移动 |
| Player/PlayerAutoAttack.cs | 自动攻击（找最近敌人） |
| Player/PlayerExperience.cs | 经验/等级/升级触发 |
| Player/PlayerHealth.cs | 生命值/受伤/死亡 |
| Enemies/Enemy.cs | 追击AI + 生命值 + 死亡事件 |
| Enemies/RangedEnemy.cs | 远程怪（继承 Enemy，保持距离射击） |
| Enemies/EnemyBullet.cs | 敌人子弹（伤害玩家） |
| Enemies/EnemySpawner.cs | 刷怪器（对象池 + 难度递增） |
| Weapons/Bullet.cs | 子弹（对象池） |
| Weapons/OrbitingWeapon.cs | 环绕刀刃（绕玩家旋转） |
| Weapons/BladeHit.cs | 刀刃碰撞伤害 |
| Systems/ExperienceGem.cs | 经验宝石 |
| Systems/ExperienceDropper.cs | 经验掉落（订阅敌人死亡事件） |
| Systems/UpgradeManager.cs | 升级三选一强化面板 |
| Systems/HUDManager.cs | HUD（血条/时间/击杀数） |
| Systems/MenuManager.cs | 开始菜单 + 死亡结算 + 游戏流程 |
| Systems/CameraShake.cs | 屏幕震动 |
| Systems/DamagePopup.cs | 伤害数字飘字 |
| Systems/DamagePopupManager.cs | 飘字对象池管理 |
| Systems/DeathEffect.cs | 敌人死亡特效 |
| Systems/DeathEffectManager.cs | 死亡特效对象池 |
| Systems/PauseMenuManager.cs | 暂停菜单（ESC） |
| Editor/TopDownSurvivorSceneSetup.cs | 一键搭建场景 |

## 技术栈

- Unity **2022.3.62f3c1**（2D Built-In Render Pipeline）
- C#

## 如何运行

1. 使用 Unity Hub 打开项目（编辑器版本 2022.3.62f3c1）
2. 打开场景 `Assets/Scenes/SampleScene`
3. 点击菜单 `Tools → TopDownSurvivor → Setup Scene` 一键搭建场景
4. 点击 Play，使用 WASD 移动

## 开发路线

1. 阶段 1：玩家八方向移动（已完成）
2. 阶段 2：敌人追击 AI + 刷怪系统（已完成）
3. 阶段 3：自动攻击 + 子弹 + 对象池（已完成）
4. 阶段 4：击杀掉落经验 + 升级系统（已完成）
5. 阶段 5：完整 UI（已完成）
6. 阶段 6：音效 + 存档 + 打包（音效存档完成，打包待做）
