using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 经验宝石：敌人死亡时掉落，玩家靠近自动吸附并拾取（使用对象池）。
    /// </summary>
    public class ExperienceGem : MonoBehaviour
    {
        [SerializeField] private int _experienceValue = 1;   // 提供的经验值
        [SerializeField] private float _pickupRadius = 1.5f; // 吸附半径
        [SerializeField] private float _magnetSpeed = 8f;    // 吸附飞行速度

        private ObjectPool<ExperienceGem> _pool;
        private Transform _player;

        private void Awake()
        {
            // 加碰撞体（触发器），用于检测玩家
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }

            col.isTrigger = true;
            col.radius = 0.5f;
        }

        /// <summary>
        /// 掉落/复用：设置位置、记录归属的池、缓存玩家。
        /// </summary>
        public void Init(Vector3 spawnPos, ObjectPool<ExperienceGem> pool, int experienceValue = 1)
        {
            _pool = pool;
            _experienceValue = experienceValue;
            transform.position = spawnPos;

            // 大经验宝石更大更亮
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (experienceValue >= 5)
                {
                    transform.localScale = new Vector3(1.6f, 1.6f, 1f);
                    sr.color = new Color(1f, 0.95f, 0.5f); // 亮金色
                }
                else
                {
                    transform.localScale = Vector3.one;
                    sr.color = new Color(1f, 0.85f, 0.3f); // 金色
                }
            }

            if (_player == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null)
                {
                    _player = player.transform;
                }
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            // 自动吸附：玩家靠近到吸附半径内，宝石飞向玩家
            float distance = Vector3.Distance(transform.position, _player.position);
            if (distance < _pickupRadius)
            {
                Vector3 dir = (_player.position - transform.position).normalized;
                transform.position += dir * _magnetSpeed * Time.deltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerExperience playerExp = other.GetComponent<PlayerExperience>();
            if (playerExp != null)
            {
                playerExp.AddExperience(_experienceValue);
                AudioManager.Instance.PlayPickupSound();

                if (_pool != null)
                {
                    _pool.Release(this); // 拾取后回池（不销毁）
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
