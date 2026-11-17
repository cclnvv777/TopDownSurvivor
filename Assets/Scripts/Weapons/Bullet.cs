using System.Collections;
using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;

namespace TopDownSurvivor.Weapons
{
    /// <summary>
    /// 子弹：朝固定方向直线飞行，命中敌人后造成伤害并回收到对象池。
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 3f;
        [SerializeField] private int _damage = 1; // 伤害值

        private Rigidbody2D _rb;
        private ObjectPool<Bullet> _pool; // 这颗子弹属于哪个对象池
        private Coroutine _lifeTimer;

        private void Awake()
        {
            // 子弹需要一个刚体（让触发器能工作）+ 一个碰撞体
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                _rb = gameObject.AddComponent<Rigidbody2D>();
            }

            _rb.gravityScale = 0f;      // 不受重力
            _rb.freezeRotation = true;  // 不旋转

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }

            col.isTrigger = true; // 触发器：只检测命中，不产生物理推挤
            col.radius = 0.2f;
        }

        /// <summary>
        /// 发射时调用：设定飞行方向，并记录它属于哪个对象池。
        /// </summary>
        public void Init(Vector3 direction, ObjectPool<Bullet> pool, int damage)
        {
            _pool = pool;
            _damage = damage; // 用传入的伤害覆盖默认值
            _rb.velocity = direction.normalized * _speed; // 用刚体速度飞行

            // 启动超时回收计时
            if (_lifeTimer != null)
            {
                StopCoroutine(_lifeTimer);
            }

            _lifeTimer = StartCoroutine(LifeTimer());
        }

        private IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(_lifeTime);
            Recycle(); // 超时回收
        }

        /// <summary>
        /// 触发器检测：当子弹碰到别的碰撞体时，Unity 自动调用此方法。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage); // 命中敌人 → 造成伤害
                Recycle();                 // 子弹回收（不销毁）
            }
        }

        /// <summary>
        /// 回收：停止计时，把自己放回对象池（隐藏，不销毁）。
        /// </summary>
        private void Recycle()
        {
            if (_lifeTimer != null)
            {
                StopCoroutine(_lifeTimer);
                _lifeTimer = null;
            }

            _pool.Release(this);
        }
    }
}
