using UnityEngine;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Enemies
{
    /// <summary>
    /// 敌人子弹：朝玩家方向直线飞行，命中玩家造成伤害后销毁。
    /// </summary>
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _lifeTime = 3f;
        [SerializeField] private int _damage = 10;

        private Vector3 _direction;

        private void Awake()
        {
            // 加碰撞体（触发器），用于检测玩家（玩家有刚体，所以子弹不需要刚体）
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }
            col.isTrigger = true;
            col.radius = 0.2f;
        }

        /// <summary>
        /// 发射：设定飞行方向。
        /// </summary>
        public void Init(Vector3 direction)
        {
            _direction = direction.normalized;
            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
