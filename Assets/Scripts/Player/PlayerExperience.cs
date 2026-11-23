using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Systems;

namespace TopDownSurvivor.Player
{
    /// <summary>
    /// 强化类型：升级时可选的能力提升。
    /// </summary>
    public enum UpgradeType
    {
        FireRate,      // 攻速
        Damage,        // 攻击力
        MoveSpeed,     // 移速
        MaxHealth,     // 生命上限
        ExperienceGain,// 经验获取
        OrbitingBlade, // 环绕刀刃
        MultiShot      // 散射（多发子弹）
    }

    /// <summary>
    /// 玩家经验/等级数据：拾取经验宝石累积经验，升级时触发三选一强化面板。
    /// </summary>
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField] private int _experience = 0;
        [SerializeField] private int _experienceToNextLevel = 5; // 升到下一级需要的经验
        [SerializeField] private int _level = 1;
        [SerializeField] private float _experienceMultiplier = 1f; // 经验获取倍率

        public int Experience => _experience;
        public int Level => _level;

        /// <summary>
        /// 增加经验；经验满则升级。
        /// </summary>
        public void AddExperience(int amount)
        {
            _experience += Mathf.RoundToInt(amount * _experienceMultiplier);

            while (_experience >= _experienceToNextLevel)
            {
                _experience -= _experienceToNextLevel;
                _level++;
                _experienceToNextLevel += 5; // 每级需求递增
                LevelUp();
            }
        }

        private void LevelUp()
        {
            AudioManager.Instance.PlayLevelUpSound();
            UpgradeManager manager = FindObjectOfType<UpgradeManager>();
            if (manager != null)
            {
                manager.ShowUpgrades(); // 弹出三选一强化面板
            }
        }

        /// <summary>
        /// 提升经验获取倍率（升级强化用）。
        /// </summary>
        public void IncreaseExperienceGain(float amount)
        {
            _experienceMultiplier += amount;
        }
    }
}
