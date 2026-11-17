# TopDownSurvivor 俯视角动作生存游戏

一款类似《吸血鬼幸存者》的俯视角生存闯关游戏。玩家在场上移动躲避，自动攻击消灭从四周不断涌来的敌人，击杀掉落经验、升级三选一强化，尽可能存活更久。项目重点在于**代码架构**与**性能优化**。

## 技术亮点

- **对象池（Object Pool）**：子弹、敌人都复用，避免频繁 `Instantiate`/`Destroy` 导致的 GC 卡顿（性能优化核心）
- **敌人追击 AI（Seek）**：通过向量相减 + 归一化实现追踪行为
- **刷怪系统**：使用协程（Coroutine）定时从玩家四周生成敌人
- **自动攻击**：遍历敌人查找最近目标（线性求最小值算法）
- **事件系统（EventBus）**：泛型类型安全事件总线，敌人死亡事件解耦经验掉落等系统
- **单例管理器（Singleton）**：统一的 MonoBehaviour 单例基类
- **升级系统**：三选一强化（攻速/攻击力/移速），用代码动态创建 UI
- **一键搭建场景**：编辑器脚本自动生成玩家/敌人/相机/刷怪器/升级 UI，并自动保存场景

## 已完成功能

- [x] 玩家八方向移动（WASD，斜向移动不加速）
- [x] 敌人追击 AI + 刷怪系统
- [x] 自动攻击 + 子弹对象池 + 敌人对象池
- [x] 碰撞检测 + 伤害判定
- [x] 击杀掉落经验宝石 + 自动拾取
- [x] 升级系统（三选一强化面板）
- [ ] 玩家生命值 + 碰撞伤害
- [ ] HUD（血条 / 时间 / 击杀数）
- [ ] 开始菜单 + 死亡结算
- [ ] 音效 + 存档 + 打包优化

## 目录结构

```
Assets/
├── Scripts/
│   ├── Core/       # 核心框架（单例 / 事件总线 / 对象池 / 游戏状态）
│   ├── Player/     # 玩家逻辑（移动 / 自动攻击 / 经验）
│   ├── Enemies/    # 敌人逻辑（追击 AI / 刷怪器）
│   ├── Weapons/    # 武器（子弹）
│   └── Systems/    # 系统（经验掉落 / 升级管理）
├── Editor/         # 编辑器工具（一键搭建场景）
├── Prefabs/        # 预制体（子弹 / 敌人 / 经验宝石）
├── Sprites/        # 占位精灵
└── Scenes/         # 场景
```

## 核心脚本说明

| 文件 | 作用 |
|---|---|
| Core/Singleton.cs | MonoBehaviour 单例基类，保证全局唯一实例 |
| Core/EventBus.cs | 泛型事件总线，模块间解耦通信 |
| Core/GameManager.cs | 游戏状态管理（菜单/游戏中/升级/结束） |
| Core/ObjectPool.cs | 通用对象池，内部用队列管理复用 |
| Player/PlayerMovement.cs | 玩家八方向移动 |
| Player/PlayerAutoAttack.cs | 自动攻击，找最近敌人发射子弹 |
| Player/PlayerExperience.cs | 经验/等级数据，触发升级 |
| Enemies/Enemy.cs | 敌人追击 AI + 生命值 + 死亡事件广播 |
| Enemies/EnemySpawner.cs | 刷怪器，从对象池取敌人 |
| Weapons/Bullet.cs | 子弹，直线飞行 + 命中回收 |
| Systems/ExperienceGem.cs | 经验宝石，玩家拾取得经验 |
| Systems/ExperienceDropper.cs | 订阅敌人死亡事件，掉落经验宝石 |
| Systems/UpgradeManager.cs | 升级三选一强化面板（代码动态创建 UI） |
| Editor/TopDownSurvivorSceneSetup.cs | 一键搭建场景，自动配置所有引用 |

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
5. 阶段 5：完整 UI（进行中）
6. 阶段 6：音效 + 存档 + 打包 + 优化
