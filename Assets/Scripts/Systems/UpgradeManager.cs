using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 升级管理：升级时暂停游戏，弹出三选一强化面板（UI 用代码动态创建）。
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        private Canvas _canvas;
        private readonly Button[] _buttons = new Button[3];
        private readonly Text[] _labels = new Text[3];
        private readonly UpgradeType[] _options = new UpgradeType[3];

        private void Awake()
        {
            BuildUI();
            _canvas.gameObject.SetActive(false); // 初始隐藏
        }

        /// <summary>
        /// 弹出升级面板：暂停游戏，随机三个强化，显示 UI。
        /// </summary>
        public void ShowUpgrades()
        {
            Time.timeScale = 0f; // 暂停游戏

            RollOptions();
            RefreshLabels();
            _canvas.gameObject.SetActive(true);
        }

        private void RollOptions()
        {
            // 洗牌：把三种强化打乱顺序，保证三个选项不重复
            int[] all = { 0, 1, 2 };
            for (int i = all.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int tmp = all[i];
                all[i] = all[j];
                all[j] = tmp;
            }

            for (int i = 0; i < 3; i++)
            {
                _options[i] = (UpgradeType)all[i];
            }
        }

        private void RefreshLabels()
        {
            for (int i = 0; i < 3; i++)
            {
                _labels[i].text = Describe(_options[i]);
            }
        }

        private static string Describe(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.FireRate: return "攻击速度提升";
                case UpgradeType.Damage: return "攻击力提升";
                case UpgradeType.MoveSpeed: return "移动速度提升";
                default: return string.Empty;
            }
        }

        private void OnOptionChosen(int index)
        {
            ApplyUpgrade(_options[index]);
            _canvas.gameObject.SetActive(false);
            Time.timeScale = 1f; // 恢复游戏
        }

        private static void ApplyUpgrade(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.FireRate:
                    FindObjectOfType<PlayerAutoAttack>().IncreaseFireRate(0.08f);
                    break;
                case UpgradeType.Damage:
                    FindObjectOfType<PlayerAutoAttack>().IncreaseDamage(1);
                    break;
                case UpgradeType.MoveSpeed:
                    FindObjectOfType<PlayerMovement>().IncreaseMoveSpeed(0.5f);
                    break;
            }
        }

        private void BuildUI()
        {
            // Canvas
            GameObject canvasObj = new GameObject("UpgradeCanvas");
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // EventSystem（UI 交互必需，如果没有则创建）
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            // 半透明背景遮罩
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(canvasObj.transform, false);
            Image img = panel.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.7f);
            RectTransform prt = panel.GetComponent<RectTransform>();
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;

            // 3 个按钮，竖排
            for (int i = 0; i < 3; i++)
            {
                _buttons[i] = CreateButton(canvasObj.transform, i);
                _labels[i] = _buttons[i].GetComponentInChildren<Text>();
                int captured = i; // 闭包捕获
                _buttons[i].onClick.AddListener(() => OnOptionChosen(captured));
            }
        }

        private static Button CreateButton(Transform parent, int index)
        {
            GameObject btnObj = new GameObject("Button" + index);
            btnObj.transform.SetParent(parent, false);

            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.2f, 0.25f);
            Button btn = btnObj.AddComponent<Button>();

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(420f, 90f);
            rt.anchoredPosition = new Vector2(0f, 120f - index * 120f); // 竖排间隔

            // 按钮文字
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 32;
            RectTransform trt = textObj.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            return btn;
        }
    }
}
