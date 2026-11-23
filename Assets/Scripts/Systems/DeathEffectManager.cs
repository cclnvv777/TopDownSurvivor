using System.Collections.Generic;
using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 死亡特效管理器：用队列池复用特效，避免频繁创建销毁。
    /// </summary>
    public class DeathEffectManager : Singleton<DeathEffectManager>
    {
        [SerializeField] private int _poolSize = 20;
        private readonly Queue<DeathEffect> _pool = new Queue<DeathEffect>();

        private void Awake()
        {
            base.Awake();
            for (int i = 0; i < _poolSize; i++)
            {
                _pool.Enqueue(CreateEffect(false));
            }
        }

        private DeathEffect CreateEffect(bool active)
        {
            GameObject go = new GameObject("DeathEffect");
            DeathEffect effect = go.AddComponent<DeathEffect>();
            go.SetActive(active);
            return effect;
        }

        /// <summary>从池中取一个特效（池空则新建）。</summary>
        public DeathEffect Get()
        {
            DeathEffect effect = _pool.Count > 0 ? _pool.Dequeue() : CreateEffect(true);
            effect.gameObject.SetActive(true);
            return effect;
        }

        /// <summary>把特效放回池（隐藏，不销毁）。</summary>
        public void Release(DeathEffect effect)
        {
            effect.gameObject.SetActive(false);
            _pool.Enqueue(effect);
        }

        /// <summary>在指定位置播放一个死亡特效。</summary>
        public void PlayEffect(Vector3 position, Color color)
        {
            DeathEffect effect = Get();
            effect.Init(position, color, this);
        }
    }
}
