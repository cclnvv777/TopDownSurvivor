using System.Collections;
using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Enemies
{
    /// <summary>
    /// 刷怪器：每隔一段时间，从对象池取出敌人，放在玩家周围随机位置。
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy _enemyPrefab;        // 敌人预制体（Enemy 类型）
        [SerializeField] private float _spawnInterval = 1.5f; // 生成间隔（秒）
        [SerializeField] private float _spawnRadius = 8f;     // 在玩家周围这个半径外生成
        [SerializeField] private int _poolSize = 20;          // 预创建敌人数量

        private Transform _player;
        private ObjectPool<Enemy> _enemyPool;

        private void Start()
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                _player = player.transform;
            }

            // 创建敌人对象池（预创建 _poolSize 个）
            _enemyPool = new ObjectPool<Enemy>(_enemyPrefab, _poolSize);
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                // 暂停 _spawnInterval 秒，再继续往下执行
                yield return new WaitForSeconds(_spawnInterval);

                SpawnOneEnemy();
            }
        }

        private void SpawnOneEnemy()
        {
            if (_enemyPrefab == null)
            {
                return;
            }

            // 随机一个方向，再乘半径，得到「玩家周围一圈」上的随机点
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector3 center = _player != null ? _player.position : transform.position;
            Vector3 spawnPos = center + (Vector3)(dir * _spawnRadius);

            Enemy enemy = _enemyPool.Get();   // 从池里取（不再 Instantiate）
            enemy.Init(spawnPos, _enemyPool); // 设置位置 + 重置生命值 + 记录池
        }
    }
}
