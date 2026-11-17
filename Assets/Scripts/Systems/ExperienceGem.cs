using UnityEngine;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 经验宝石：敌人死亡时掉落，玩家靠近自动拾取获得经验。
    /// </summary>
    public class ExperienceGem : MonoBehaviour
    {
        [SerializeField] private int _experienceValue = 1; // 这颗宝石提供的经验值

        private void Awake()
        {
            // 加碰撞体（触发器），用于检测玩家靠近
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }

            col.isTrigger = true;
            col.radius = 0.5f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerExperience playerExp = other.GetComponent<PlayerExperience>();
            if (playerExp != null)
            {
                playerExp.AddExperience(_experienceValue);
                Destroy(gameObject); // 拾取后销毁（后续可优化为对象池）
            }
        }
    }
}
