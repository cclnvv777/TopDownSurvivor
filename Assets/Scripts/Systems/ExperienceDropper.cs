using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 经验掉落系统：订阅敌人死亡事件，从对象池取出宝石放在死亡位置。
    /// </summary>
    public class ExperienceDropper : MonoBehaviour
    {
        [SerializeField] private ExperienceGem _gemPrefab;
        [SerializeField] private int _poolSize = 30; // 预创建宝石数量

        private ObjectPool<ExperienceGem> _gemPool;

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
        }

        private void Start()
        {
            _gemPool = new ObjectPool<ExperienceGem>(_gemPrefab, _poolSize);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            if (_gemPrefab == null)
            {
                return;
            }

            ExperienceGem gem = _gemPool.Get(); // 从池取（不再 Instantiate）
            gem.Init(e.Position, _gemPool);
        }
    }
}
