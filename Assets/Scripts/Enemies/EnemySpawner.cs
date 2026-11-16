using System.Collections;
using UnityEngine;

namespace TopDownSurvivor.Enemies
{
    /// <summary>
    /// 刷怪器：每隔一段时间，在玩家周围随机位置生成敌人。
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;    // 敌人预制体（在检查器里拖入）
        [SerializeField] private float _spawnInterval = 1.5f; // 生成间隔（秒）
        [SerializeField] private float _spawnRadius = 8f;     // 在玩家周围这个半径外生成

        private Transform _player;

        private void Start()
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                _player = player.transform;
            }

            // 启动协程：开始循环刷怪
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

            Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
