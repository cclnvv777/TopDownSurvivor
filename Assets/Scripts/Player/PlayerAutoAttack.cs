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

            // 发射方向：从玩家指向最近敌人
            Vector3 direction = (nearest.transform.position - transform.position).normalized;

            Bullet bullet = _bulletPool.Get();             // 从池里取（不再 Instantiate）
            bullet.transform.position = transform.position; // 设置出生位置
            bullet.Init(direction, _bulletPool);            // 设定方向 + 归属的池
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
    }
}
