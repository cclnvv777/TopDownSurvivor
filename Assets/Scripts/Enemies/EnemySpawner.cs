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
        [SerializeField] private float _minSpawnInterval = 0.3f;          // 刷怪间隔下限
        [SerializeField] private float _intervalDecreasePerSpawn = 0.03f; // 每次刷怪间隔递减
        [SerializeField] private float _enemySpeedIncreasePerSpawn = 0.05f; // 敌人移速每次递增
        [SerializeField] private float _eliteBaseChance = 0.05f;    // 精英初始概率
        [SerializeField] private float _eliteMaxChance = 0.5f;      // 精英最大概率
        [SerializeField] private float _eliteChanceIncreasePerSec = 0.01f; // 每秒增加概率
        [SerializeField] private RangedEnemy _rangedEnemyPrefab; // 远程怪预制体
        [SerializeField] private float _rangedBaseChance = 0.05f;  // 远程怪初始概率
        [SerializeField] private float _rangedMaxChance = 0.3f;    // 远程怪最大概率
        [SerializeField] private float _rangedChanceIncreasePerSec = 0.005f; // 每秒增加概率

        private Transform _player;
        private ObjectPool<Enemy> _enemyPool;
        private ObjectPool<Enemy> _rangedEnemyPool; // 远程怪池（用 Enemy 类型存 RangedEnemy）
        private float _enemySpeed = 2f; // 当前敌人移速（随难度递增）
        private float _surviveTime; // 存活时间（用于概率递增）

        private void Start()
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                _player = player.transform;
            }

            // 创建敌人对象池（预创建 _poolSize 个）
            _enemyPool = new ObjectPool<Enemy>(_enemyPrefab, _poolSize);
            _rangedEnemyPool = new ObjectPool<Enemy>(_rangedEnemyPrefab, _poolSize);
            StartCoroutine(SpawnLoop());
        }

        private void Update()
        {
            if (Time.timeScale > 0f)
            {
                _surviveTime += Time.deltaTime; // 累计存活时间
            }
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

            // 远程怪概率随时间递增
            float rangedChance = Mathf.Min(_rangedMaxChance, _rangedBaseChance + _surviveTime * _rangedChanceIncreasePerSec);
            if (Random.value < rangedChance)
            {
                Enemy ranged = _rangedEnemyPool.Get();
                ranged.Init(spawnPos, _rangedEnemyPool, _enemySpeed, false);
            }
            else
            {
                // 精英概率随时间递增：初始 5%，每秒 +1%，最高 50%
                float eliteChance = Mathf.Min(_eliteMaxChance, _eliteBaseChance + _surviveTime * _eliteChanceIncreasePerSec);
                bool isElite = Random.value < eliteChance;
                Enemy enemy = _enemyPool.Get();
                enemy.Init(spawnPos, _enemyPool, _enemySpeed, isElite);
            }

            // 难度递增：刷怪间隔递减 + 敌人移速递增
            _spawnInterval = Mathf.Max(_minSpawnInterval, _spawnInterval - _intervalDecreasePerSpawn);
            _enemySpeed += _enemySpeedIncreasePerSpawn;
        }
    }
}
