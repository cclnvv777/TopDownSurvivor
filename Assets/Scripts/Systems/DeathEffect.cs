using System.Collections;
using UnityEngine;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 敌人死亡特效：一个方块快速缩小 + 淡出，模拟「爆炸闪光」，用对象池复用。
    /// </summary>
    public class DeathEffect : MonoBehaviour
    {
        private const float Duration = 0.3f; // 特效持续时间

        private SpriteRenderer _sr;
        private DeathEffectManager _manager;

        private void Awake()
        {
            _sr = gameObject.AddComponent<SpriteRenderer>();
            _sr.sprite = CreateSprite();
        }

        /// <summary>初始化特效：位置、颜色、归属的池管理器。</summary>
        public void Init(Vector3 position, Color color, DeathEffectManager manager)
        {
            _manager = manager;
            transform.position = position;
            _sr.color = color;
            transform.localScale = Vector3.one;

            StopAllCoroutines();
            StartCoroutine(EffectRoutine());
        }

        private IEnumerator EffectRoutine()
        {
            float t = 0f;
            while (t < Duration)
            {
                t += Time.deltaTime;
                float progress = t / Duration;

                transform.localScale = Vector3.one * (1f - progress); // 缩小到 0
                Color c = _sr.color;
                c.a = 1f - progress; // 淡出
                _sr.color = c;

                yield return null;
            }

            _manager.Release(this);
        }

        private static Sprite CreateSprite()
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f); // 1 单位大小
        }
    }
}
