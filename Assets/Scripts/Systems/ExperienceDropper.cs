using UnityEngine;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 掉落系统：订阅敌人死亡事件，按概率掉落普通经验 / 血包 / 大经验（均用对象池）。
    /// </summary>
    public class ExperienceDropper : MonoBehaviour
    {
        [SerializeField] private ExperienceGem _gemPrefab;         // 经验宝石预制体
        [SerializeField] private HealthPack _healthPackPrefab;     // 血包预制体
        [SerializeField] private int _poolSize = 30;               // 宝石预创建数量
        [SerializeField] private float _healthPackChance = 0.3f;   // 血包概率 30%
        [SerializeField] private float _bigExpChance = 0.05f;      // 大经验概率 5%

        private ObjectPool<ExperienceGem> _gemPool;
        private ObjectPool<HealthPack> _healthPackPool;

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
            if (_healthPackPrefab != null)
            {
                _healthPackPool = new ObjectPool<HealthPack>(_healthPackPrefab, 10);
            }
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            float roll = Random.value;

            if (roll < _bigExpChance)
            {
                // 5% 大经验（5 经验，更大更亮）
                if (_gemPrefab == null) return;
                ExperienceGem gem = _gemPool.Get();
                gem.Init(e.Position, _gemPool, 5);
            }
            else if (roll < _bigExpChance + _healthPackChance)
            {
                // 10% 血包
                if (_healthPackPrefab == null || _healthPackPool == null) return;
                HealthPack pack = _healthPackPool.Get();
                pack.Init(e.Position, _healthPackPool);
            }
            else
            {
                // 其余普通经验（1 经验）
                if (_gemPrefab == null) return;
                ExperienceGem gem = _gemPool.Get();
                gem.Init(e.Position, _gemPool, 1);
            }
        }
    }
}
