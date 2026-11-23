using System.Collections;
using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Systems;

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
        [SerializeField] private float _invincibleDuration = 1f; // 受伤后无敌时长（秒）

        private int _currentHealth;
        private bool _isInvincible;
        private SpriteRenderer _sr;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        /// <summary>
        /// 提升生命上限并回复等量生命（升级强化用）。
        /// </summary>
        public void IncreaseMaxHealth(int amount)
        {
            _maxHealth += amount;
            _currentHealth += amount;
        }

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 受到伤害，扣血；血空则死亡。
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (_isInvincible || _currentHealth <= 0)
            {
                return; // 无敌中 或 已死亡，不受伤
            }

            _currentHealth -= damage;
            Debug.Log($"[TopDownSurvivor] 玩家受伤 -{damage}，剩余 {_currentHealth}/{_maxHealth}");
            if (DamagePopupManager.Instance != null)
            {
                DamagePopupManager.Instance.ShowDamage(transform.position, damage, Color.red);
            }
            AudioManager.Instance.PlayPlayerHurtSound();
            CameraShake.Instance.Shake();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityRoutine());
            }
        }

        /// <summary>
        /// 无敌帧：受伤后闪烁 1 秒，期间不再掉血。
        /// </summary>
        private IEnumerator InvincibilityRoutine()
        {
            _isInvincible = true;
            float t = 0f;
            while (t < _invincibleDuration)
            {
                if (_sr != null)
                {
                    _sr.enabled = !_sr.enabled; // 开关闪烁
                }
                yield return new WaitForSeconds(0.1f);
                t += 0.1f;
            }

            if (_sr != null)
            {
                _sr.enabled = true; // 恢复显示
            }
            _isInvincible = false;
        }

        private void Die()
        {
            Debug.Log("[TopDownSurvivor] 玩家死亡！");
            EventBus<PlayerDiedEvent>.Publish(new PlayerDiedEvent());
        }
    }
}
