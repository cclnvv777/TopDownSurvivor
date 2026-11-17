using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 经验掉落系统：订阅「敌人死亡」事件，在死亡位置生成经验宝石。
    /// 这是 EventBus 的「订阅方」，与敌人完全解耦。
    /// </summary>
    public class ExperienceDropper : MonoBehaviour
    {
        [SerializeField] private ExperienceGem _gemPrefab; // 经验宝石预制体

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            if (_gemPrefab == null)
            {
                return;
            }

            Instantiate(_gemPrefab, e.Position, Quaternion.identity);
        }
    }
}
