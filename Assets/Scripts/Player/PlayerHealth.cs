using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Player
{
    /// <summary>
    /// 玩家死亡事件：通过 EventBus 广播，供菜单/结算系统订阅。
    /// </summary>
    public readonly struct PlayerDiedEvent
    {
    }

    /// <summary>
    /// 玩家生命值：受伤扣血，血空死亡。
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private int _currentHealth;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// 受到伤害，扣血；血空则死亡。
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (_currentHealth <= 0)
            {
                return; // 已死亡，不再受伤
            }

            _currentHealth -= damage;
            Debug.Log($"[TopDownSurvivor] 玩家受伤 -{damage}，剩余 {_currentHealth}/{_maxHealth}");
            AudioManager.Instance.PlayPlayerHurtSound();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("[TopDownSurvivor] 玩家死亡！");
            EventBus<PlayerDiedEvent>.Publish(new PlayerDiedEvent());
        }
    }
}
