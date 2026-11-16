using UnityEngine;

namespace TopDownSurvivor.Enemies
{
    /// <summary>
    /// 敌人：自动追击玩家，拥有生命值，可被子弹击杀。
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private int _maxHealth = 1; // 生命值（1 发子弹打死）

        private int _currentHealth;
        private Transform _player;

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

            _currentHealth = _maxHealth;
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
            Destroy(gameObject); // 先简单销毁，下一步改成「对象池回收」
        }
    }
}
