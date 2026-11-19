using UnityEngine;

namespace TopDownSurvivor.Weapons
{
    /// <summary>
    /// 环绕刀刃武器：刀片绕玩家旋转，碰到敌人造成伤害。升级选「环绕刀刃」加一把刀。
    /// </summary>
    public class OrbitingWeapon : MonoBehaviour
    {
        [SerializeField] private float _orbitRadius = 1.5f; // 旋转半径
        [SerializeField] private float _orbitSpeed = 120f;  // 旋转速度（度/秒）
        [SerializeField] private int _damage = 1;           // 每刀伤害

        private Transform _root;

        private void Awake()
        {
            _root = new GameObject("OrbitRoot").transform;
            _root.SetParent(transform);
            _root.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 增加一把刀，并重新均匀分布所有刀。
        /// </summary>
        public void AddBlade()
        {
            GameObject blade = new GameObject("Blade" + _root.childCount);
            blade.transform.SetParent(_root);

            SpriteRenderer sr = blade.AddComponent<SpriteRenderer>();
            sr.sprite = CreateWhiteSprite();
            sr.color = new Color(0.6f, 0.8f, 1f); // 蓝色

            CircleCollider2D col = blade.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;

            BladeHit hit = blade.AddComponent<BladeHit>();
            hit.Init(_damage);

            RepositionBlades();
        }

        private void RepositionBlades()
        {
            int count = _root.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform blade = _root.GetChild(i);
                float angle = i * (360f / count) * Mathf.Deg2Rad;
                blade.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _orbitRadius;
            }
        }

        private void Update()
        {
            if (_root.childCount > 0)
            {
                _root.Rotate(0f, 0f, _orbitSpeed * Time.deltaTime);
            }
        }

        private static Sprite CreateWhiteSprite()
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
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f); // 约 0.5 单位大小
        }
    }
}
