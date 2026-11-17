using UnityEngine;
using TopDownSurvivor.Systems;

namespace TopDownSurvivor.Player
{
    /// <summary>
    /// 强化类型：升级时可选的能力提升。
    /// </summary>
    public enum UpgradeType
    {
        FireRate,  // 攻速
        Damage,    // 攻击力
        MoveSpeed  // 移速
    }

    /// <summary>
    /// 玩家经验/等级数据：拾取经验宝石累积经验，升级时触发三选一强化面板。
    /// </summary>
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField] private int _experience = 0;
        [SerializeField] private int _experienceToNextLevel = 5; // 升到下一级需要的经验
        [SerializeField] private int _level = 1;

        public int Experience => _experience;
        public int Level => _level;

        /// <summary>
        /// 增加经验；经验满则升级。
        /// </summary>
        public void AddExperience(int amount)
        {
            _experience += amount;

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
            UpgradeManager manager = FindObjectOfType<UpgradeManager>();
            if (manager != null)
            {
                manager.ShowUpgrades(); // 弹出三选一强化面板
            }
        }
    }
}
