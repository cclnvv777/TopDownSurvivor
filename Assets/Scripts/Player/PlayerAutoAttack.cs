using System.Collections;
using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;
using TopDownSurvivor.Weapons;

namespace TopDownSurvivor.Player
{
    /// <summary>
    /// 玩家自动攻击：每隔一段时间，朝最近的敌人发射子弹（使用对象池）。
    /// </summary>
    public class PlayerAutoAttack : MonoBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab;    // 子弹预制体（在检查器里拖入）
        [SerializeField] private float _fireInterval = 0.5f; // 发射间隔（秒）
        [SerializeField] private int _poolSize = 20;        // 预创建子弹数量
        [SerializeField] private int _damage = 1;            // 当前攻击力（升级可提升）
        [SerializeField] private int _projectileCount = 1;    // 每次发射的子弹数（升级可提升）
        [SerializeField] private float _spreadAngle = 15f;    // 多颗子弹之间的夹角（度）
        [SerializeField] private int _pierce;                 // 穿透次数（升级可提升）

        private ObjectPool<Bullet> _bulletPool;

        private void Start()
        {
            _bulletPool = new ObjectPool<Bullet>(_bulletPrefab, _poolSize);
            StartCoroutine(FireLoop());
        }

        private IEnumerator FireLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(_fireInterval);
                FireAtNearestEnemy();
            }
        }

        private void FireAtNearestEnemy()
        {
            Enemy nearest = FindNearestEnemy();
            if (nearest == null)
            {
                return; // 场上没有敌人，不发射
            }

            // 基础方向：从玩家指向最近敌人
            Vector3 baseDirection = (nearest.transform.position - transform.position).normalized;

            // 发射多颗子弹（扇形分布）
            for (int i = 0; i < _projectileCount; i++)
            {
                // 中间那颗对准敌人，其余往两边偏移
                float angle = (i - (_projectileCount - 1) / 2f) * _spreadAngle;
                Vector3 direction = Quaternion.Euler(0f, 0f, angle) * baseDirection;

                Bullet bullet = _bulletPool.Get();             // 从池里取（不再 Instantiate）
                bullet.transform.position = transform.position; // 设置出生位置
                bullet.Init(direction, _bulletPool, _damage, _pierce);   // 设定方向 + 归属的池 + 伤害 + 穿透
            }
        }

        /// <summary>
        /// 在一堆敌人里找出离玩家最近的那个。
        /// </summary>
        private Enemy FindNearestEnemy()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            if (enemies.Length == 0)
            {
                return null;
            }

            Enemy nearest = null;
            float minDistance = float.MaxValue; // 先设成「无穷大」

            foreach (Enemy enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        /// <summary>
        /// 提升攻速（发射间隔缩短）。
        /// </summary>
        public void IncreaseFireRate(float amount)
        {
            _fireInterval = Mathf.Max(0.1f, _fireInterval - amount);
        }

        /// <summary>
        /// 提升攻击力（子弹伤害增加）。
        /// </summary>
        public void IncreaseDamage(int amount)
        {
            _damage += amount;
        }

        /// <summary>
        /// 提升散射（每次多发射一颗子弹）。
        /// </summary>
        public void IncreaseProjectileCount(int amount)
        {
            _projectileCount += amount;
        }

        /// <summary>
        /// 提升穿透（子弹可多穿过一个敌人）。
        /// </summary>
        public void IncreasePierce(int amount)
        {
            _pierce += amount;
        }
    }
}
