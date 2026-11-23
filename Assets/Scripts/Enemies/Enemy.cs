using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Player;
using TopDownSurvivor.Systems;

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
        [SerializeField] protected float _moveSpeed = 2f;
        [SerializeField] private int _maxHealth = 1; // 普通怪生命值
        [SerializeField] private int _contactDamage = 10; // 普通怪接触伤害
        [SerializeField] private int _eliteMaxHealth = 5; // 精英怪生命值
        [SerializeField] private int _eliteContactDamage = 30; // 精英怪接触伤害

        private int _currentHealth;
        private int _currentContactDamage; // 当前接触伤害（普通或精英）
        protected Transform _player;
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
            _currentContactDamage = _contactDamage; // 初始普通伤害
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

            Move();
        }

        /// <summary>
        /// 移动行为（子类可重写，如远程怪保持距离）。
        /// </summary>
        protected virtual void Move()
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 出生/复用：设置出生位置、重置生命值、记录归属的对象池，并设置移速（随难度递增）。
        /// </summary>
        public virtual void Init(Vector3 spawnPos, ObjectPool<Enemy> pool, float moveSpeed, bool isElite)
        {
            _pool = pool;
            transform.position = spawnPos;
            _moveSpeed = moveSpeed; // 设置移速

            if (isElite)
            {
                _currentHealth = _eliteMaxHealth;
                _currentContactDamage = _eliteContactDamage;
                _moveSpeed *= 0.7f; // 精英稍慢
                GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.4f, 1f); // 紫色
                transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            }
            else
            {
                _currentHealth = _maxHealth;
                _currentContactDamage = _contactDamage;
                GetComponent<SpriteRenderer>().color = new Color(1f, 0.35f, 0.35f); // 红色
                transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 受到伤害，扣血；血量为 0 时死亡。
        /// </summary>
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (DamagePopupManager.Instance != null)
            {
                DamagePopupManager.Instance.ShowDamage(transform.position, damage, Color.white);
            }
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // 播放死亡特效（同色闪光）
            if (DeathEffectManager.Instance != null)
            {
                Color color = Color.yellow;
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    color = sr.color;
                }
                DeathEffectManager.Instance.PlayEffect(transform.position, color);
            }

            // 广播「敌人死亡」事件（含死亡位置），谁关心谁订阅——敌人自己不知道有谁在听
            EventBus<EnemyDiedEvent>.Publish(new EnemyDiedEvent(transform.position));

            Recycle();
        }

        /// <summary>
        /// 触发器检测：碰到玩家 → 玩家掉血，敌人消失（不掉经验）。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_currentContactDamage);
                Recycle(); // 碰撞消失（不广播死亡事件，不掉经验）
            }
        }

        /// <summary>
        /// 回收到对象池（或销毁）。
        /// </summary>
        private void Recycle()
        {
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
