using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 暂停菜单：按 ESC 暂停/恢复，可继续或重新开始。
    /// </summary>
    public class PauseMenuManager : MonoBehaviour
    {
        private GameObject _pausePanel;
        private bool _isPaused;

        private void Awake()
        {
            BuildUI();
        }

        private void Update()
        {
            // 游戏中（或已暂停）按 ESC 切换暂停
            if (Input.GetKeyDown(KeyCode.Escape) && (Time.timeScale > 0f || _isPaused))
            {
                TogglePause();
            }
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            _pausePanel.SetActive(_isPaused);
        }

        /// <summary>继续游戏。</summary>
        public void ResumeGame()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            _pausePanel.SetActive(false);
        }

        /// <summary>重新开始（重载当前场景）。</summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void BuildUI()
        {
            GameObject canvasObj = new GameObject("PauseCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // 确保盖在 HUD 之上
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // 半透明背景
            GameObject bg = new GameObject("PauseBg");
            bg.transform.SetParent(canvasObj.transform, false);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0f, 0f, 0f, 0.6f);
            RectTransform bgRt = bgImg.rectTransform;
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            // 标题
            CreateText(canvasObj.transform, "已暂停", new Vector2(0f, 140f), 44);

            // 继续按钮
            CreateButton(canvasObj.transform, "继续游戏", new Vector2(0f, 30f), ResumeGame);

            // 重新开始按钮
            CreateButton(canvasObj.transform, "重新开始", new Vector2(0f, -50f), RestartGame);

            _pausePanel = canvasObj;
            _pausePanel.SetActive(false);
        }

        private static void CreateText(Transform parent, string content, Vector2 pos, int fontSize)
        {
            GameObject textObj = new GameObject("Text_" + content);
            textObj.transform.SetParent(parent, false);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = content;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            RectTransform rt = text.rectTransform;
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(400f, 60f);
        }

        private static void CreateButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject("Btn_" + label);
            btnObj.transform.SetParent(parent, false);
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.25f, 0.25f, 0.35f);
            RectTransform rt = img.rectTransform;
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(300f, 60f);

            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = label;
            text.fontSize = 32;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            RectTransform textRt = text.rectTransform;
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
        }
    }
}
