using UnityEngine;
using UnityEngine.UI;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// HUD：屏幕常驻显示血条、存活时间、击杀数（UI 用代码动态创建）。
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        private Image _healthFill;
        private Text _timeText;
        private Text _killText;

        private PlayerHealth _playerHealth;
        private int _killCount;
        private float _surviveTime;

        public float SurviveTime => _surviveTime;
        public int KillCount => _killCount;

        private void Awake()
        {
            BuildUI();
        }

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
            _playerHealth = FindObjectOfType<PlayerHealth>();
            if (_playerHealth == null)
            {
                Debug.LogWarning("[HUDManager] 没找到 PlayerHealth，血条无法更新，请重新运行 Setup Scene。");
            }
        }

        private void Update()
        {
            if (_playerHealth == null)
            {
                return;
            }

            // 血条填充比例 = 当前血 / 最大血
            _healthFill.fillAmount = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;

            // 存活时间（游戏暂停时不走）
            if (Time.timeScale > 0f)
            {
                _surviveTime += Time.deltaTime;
                _timeText.text = FormatTime(_surviveTime);
            }
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            _killCount++;
            _killText.text = "击杀 " + _killCount;
        }

        private static string FormatTime(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            int min = total / 60;
            int sec = total % 60;
            return $"{min:00}:{sec:00}";
        }

        /// <summary>
        /// 运行时生成一张 1x1 白色贴图，供血条等 Filled Image 使用。
        /// </summary>
        private static Sprite CreateWhiteSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }

        private void BuildUI()
        {
            // Canvas
            GameObject canvasObj = new GameObject("HUDCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // 血条背景（左上角）
            GameObject barBg = new GameObject("HealthBarBG");
            barBg.transform.SetParent(canvasObj.transform, false);
            Image bgImg = barBg.AddComponent<Image>();
            bgImg.sprite = CreateWhiteSprite();
            bgImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            RectTransform bgRt = barBg.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0f, 1f);
            bgRt.anchorMax = new Vector2(0f, 1f);
            bgRt.pivot = new Vector2(0f, 1f);
            bgRt.anchoredPosition = new Vector2(30f, -30f);
            bgRt.sizeDelta = new Vector2(300f, 24f);

            // 血条填充（红色）
            GameObject barFill = new GameObject("HealthBarFill");
            barFill.transform.SetParent(barBg.transform, false);
            _healthFill = barFill.AddComponent<Image>();
            _healthFill.sprite = CreateWhiteSprite();
            _healthFill.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            _healthFill.type = Image.Type.Filled;
            _healthFill.fillMethod = Image.FillMethod.Horizontal;
            _healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            _healthFill.fillAmount = 1f;
            RectTransform fillRt = barFill.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.offsetMin = new Vector2(2f, 2f);
            fillRt.offsetMax = new Vector2(-2f, -2f);

            // 时间文本（血条下方）
            _timeText = CreateText(canvasObj.transform, "TimeText", new Vector2(0f, 1f), new Vector2(30f, -70f), TextAnchor.MiddleLeft);
            _timeText.text = "00:00";

            // 击杀文本（右上角）
            _killText = CreateText(canvasObj.transform, "KillText", new Vector2(1f, 1f), new Vector2(-30f, -30f), TextAnchor.MiddleRight);
            _killText.text = "击杀 0";
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchor, Vector2 anchoredPos, TextAnchor alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            Text text = obj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.color = Color.white;
            text.alignment = alignment;
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(300f, 40f);
            return text;
        }
    }
}
