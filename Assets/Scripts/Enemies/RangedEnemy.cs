using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Enemies
{
    /// <summary>
    /// 远程敌人：继承 Enemy，保持距离并定时朝玩家发射子弹。
    /// </summary>
    public class RangedEnemy : Enemy
    {
        [SerializeField] private float _attackRange = 6f;     // 攻击范围（保持的距离）
        [SerializeField] private float _fireInterval = 2f;    // 射击间隔
        [SerializeField] private EnemyBullet _bulletPrefab;   // 敌人子弹预制体

        private float _fireTimer;

        /// <summary>
        /// 重写移动：保持距离（太远靠近，够近停下），并定时射击。
        /// </summary>
        protected override void Move()
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            // 太远就靠近，够近就停下（保持 _attackRange 距离）
            if (distance > _attackRange)
            {
                Vector3 dir = (_player.position - transform.position).normalized;
                transform.position += dir * _moveSpeed * Time.deltaTime;
            }

            // 定时射击
            _fireTimer += Time.deltaTime;
            if (_fireTimer >= _fireInterval)
            {
                _fireTimer = 0f;
                Fire();
            }
        }

        /// <summary>
        /// 重写出生：先按普通逻辑初始化，再把颜色改成橙色（远程怪专属）。
        /// </summary>
        public override void Init(Vector3 spawnPos, ObjectPool<Enemy> pool, float moveSpeed, bool isElite)
        {
            base.Init(spawnPos, pool, moveSpeed, isElite);
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.2f); // 橙色
        }

        private void Fire()
        {
            if (_bulletPrefab == null)
            {
                return;
            }

            Vector3 dir = (_player.position - transform.position).normalized;
            EnemyBullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            bullet.Init(dir);
        }
    }
}
