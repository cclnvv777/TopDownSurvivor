using System.Collections;
using UnityEngine;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 伤害数字飘字：向上飘 + 淡出，用对象池复用。
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private float _floatDistance = 1.5f; // 上飘距离
        [SerializeField] private float _lifetime = 0.8f;      // 存活时间

        private TextMesh _text;
        private DamagePopupManager _manager;

        private void Awake()
        {
            _text = gameObject.AddComponent<TextMesh>();
            _text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _text.anchor = TextAnchor.MiddleCenter;
            _text.fontSize = 48;
            _text.characterSize = 0.15f;
            _text.fontStyle = FontStyle.Bold;
        }

        /// <summary>初始化飘字：位置、数值、颜色、归属的池管理器。</summary>
        public void Init(Vector3 position, int damage, Color color, DamagePopupManager manager)
        {
            _manager = manager;
            transform.position = position;
            _text.text = damage.ToString();
            _text.color = color;

            StopAllCoroutines();
            StartCoroutine(FloatRoutine());
        }

        private IEnumerator FloatRoutine()
        {
            float t = 0f;
            Vector3 start = transform.position;

            while (t < _lifetime)
            {
                t += Time.deltaTime;
                float progress = t / _lifetime;

                transform.position = start + Vector3.up * (_floatDistance * progress); // 向上飘
                Color c = _text.color;
                c.a = 1f - progress; // 淡出
                _text.color = c;

                yield return null;
            }

            _manager.Release(this);
        }
    }
}
