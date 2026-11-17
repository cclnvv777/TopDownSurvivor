using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Enemies
{
    /// <summary>
    /// 敌人死亡事件：通过 EventBus 广播，供经验掉落、音效、击杀计数等系统订阅。
    /// </summary>
    public readonly struct EnemyDiedEvent
    {
        public readonly Vector3 Position;

        public EnemyDiedEvent(Vector3 position)
        {
            Position = position;
        }
    }

    /// <summary>
    /// 敌人：自动追击玩家，拥有生命值，可被子弹击杀（使用对象池）。
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private int _maxHealth = 1; // 生命值（1 发子弹打死）

        private int _currentHealth;
        private Transform _player;
        private ObjectPool<Enemy> _pool; // 这个敌人属于哪个对象池

        private void Awake()
        {
            // 给敌人补一个「身体」（碰撞体），否则子弹检测不到它
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }

            col.isTrigger = true; // 触发器：只检测「碰到」，不产生物理推挤
            col.radius = 0.5f;

            _currentHealth = _maxHealth; // 初始满血
        }

        private void Start()
        {
            // 找到玩家并缓存其 Transform
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                _player = player.transform;
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            Vector3 direction = (_player.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 出生/复用：设置出生位置、重置生命值、记录归属的对象池。
        /// </summary>
        public void Init(Vector3 spawnPos, ObjectPool<Enemy> pool)
        {
            _pool = pool;
            transform.position = spawnPos;
            _currentHealth = _maxHealth; // 重置生命值（复用的敌人可能残血）
        }

        /// <summary>
        /// 受到伤害，扣血；血量为 0 时死亡。
        /// </summary>
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // 广播「敌人死亡」事件（含死亡位置），谁关心谁订阅——敌人自己不知道有谁在听
            EventBus<EnemyDiedEvent>.Publish(new EnemyDiedEvent(transform.position));

            if (_pool != null)
            {
                _pool.Release(this); // 回池复用
            }
            else
            {
                Destroy(gameObject); // 没有池（场景初始敌人）才真正销毁
            }
        }
    }
}
