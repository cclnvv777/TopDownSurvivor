using System.Collections.Generic;
using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 伤害数字飘字管理器：用队列池复用飘字，避免频繁创建销毁。
    /// </summary>
    public class DamagePopupManager : Singleton<DamagePopupManager>
    {
        [SerializeField] private int _poolSize = 30;
        private readonly Queue<DamagePopup> _pool = new Queue<DamagePopup>();

        private void Awake()
        {
            base.Awake();
            for (int i = 0; i < _poolSize; i++)
            {
                _pool.Enqueue(CreatePopup(false));
            }
        }

        private DamagePopup CreatePopup(bool active)
        {
            GameObject go = new GameObject("DamagePopup");
            DamagePopup popup = go.AddComponent<DamagePopup>();
            go.SetActive(active);
            return popup;
        }

        /// <summary>从池中取一个飘字（池空则新建）。</summary>
        public DamagePopup Get()
        {
            DamagePopup popup = _pool.Count > 0 ? _pool.Dequeue() : CreatePopup(true);
            popup.gameObject.SetActive(true);
            return popup;
        }

        /// <summary>把飘字放回池（隐藏，不销毁）。</summary>
        public void Release(DamagePopup popup)
        {
            popup.gameObject.SetActive(false);
            _pool.Enqueue(popup);
        }

        /// <summary>在指定位置显示一个伤害数字。</summary>
        public void ShowDamage(Vector3 position, int damage, Color color)
        {
            DamagePopup popup = Get();
            popup.Init(position, damage, color, this);
        }
    }
}
