using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TopDownSurvivor.Core;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 菜单管理：开始菜单 + 死亡结算，管理游戏流程（菜单 -> 游戏 -> 结算 -> 重开）。
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        private Canvas _menuCanvas;
        private Canvas _gameOverCanvas;
        private Text _timeResult;
        private Text _killResult;
        private Text _levelResult;

        private void Awake()
        {
            BuildMenuUI();
            BuildGameOverUI();

            _gameOverCanvas.gameObject.SetActive(false);
            _menuCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f; // 启动时暂停，显示开始菜单
        }

        private void OnEnable()
        {
            EventBus<PlayerDiedEvent>.Subscribe(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus<PlayerDiedEvent>.Unsubscribe(OnPlayerDied);
        }

        private void OnStartClicked()
        {
            _menuCanvas.gameObject.SetActive(false);
            Time.timeScale = 1f; // 恢复游戏
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            HUDManager hud = FindObjectOfType<HUDManager>();
            float surviveTime = hud != null ? hud.SurviveTime : 0f;
            int killCount = hud != null ? hud.KillCount : 0;

            SaveSystem.SaveBestTime(surviveTime); // 保存最高分

            PlayerExperience pe = FindObjectOfType<PlayerExperience>();
            int level = pe != null ? pe.Level : 1;

            _timeResult.text = "存活时间：" + FormatTime(surviveTime);
            _killResult.text = "击杀数：" + killCount + "    最高存活：" + FormatTime(SaveSystem.LoadBestTime());
            _levelResult.text = "等级：" + level;

            _gameOverCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f; // 暂停
        }

        private void OnRestartClicked()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private static string FormatTime(float seconds)
        {
            int total = Mathf.FloorToInt(seconds);
            int min = total / 60;
            int sec = total % 60;
            return $"{min:00}:{sec:00}";
        }

        private void BuildMenuUI()
        {
            GameObject canvasObj = new GameObject("MenuCanvas");
            _menuCanvas = canvasObj.AddComponent<Canvas>();
            _menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // 标题
            Text title = CreateText(canvasObj.transform, "Title", new Vector2(0.5f, 0.5f), new Vector2(0f, 150f), 64, TextAnchor.MiddleCenter);
            title.text = "TopDownSurvivor";

            // 开始按钮
            Button startBtn = CreateButton(canvasObj.transform, "StartButton", new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), "开始游戏");
            startBtn.onClick.AddListener(OnStartClicked);
        }

        private void BuildGameOverUI()
        {
            GameObject canvasObj = new GameObject("GameOverCanvas");
            _gameOverCanvas = canvasObj.AddComponent<Canvas>();
            _gameOverCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // 半透明背景
            GameObject bg = new GameObject("BG");
            bg.transform.SetParent(canvasObj.transform, false);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.sprite = CreateWhiteSprite();
            bgImg.color = new Color(0f, 0f, 0f, 0.7f);
            RectTransform bgRt = bg.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            Text title = CreateText(canvasObj.transform, "GameOverTitle", new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), 56, TextAnchor.MiddleCenter);
            title.text = "游戏结束";

            _timeResult = CreateText(canvasObj.transform, "TimeResult", new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), 36, TextAnchor.MiddleCenter);
            _killResult = CreateText(canvasObj.transform, "KillResult", new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), 36, TextAnchor.MiddleCenter);
            _levelResult = CreateText(canvasObj.transform, "LevelResult", new Vector2(0.5f, 0.5f), new Vector2(0f, -100f), 36, TextAnchor.MiddleCenter);

            Button restartBtn = CreateButton(canvasObj.transform, "RestartButton", new Vector2(0.5f, 0.5f), new Vector2(0f, -180f), "重新开始");
            restartBtn.onClick.AddListener(OnRestartClicked);
        }

        private static Button CreateButton(Transform parent, string name, Vector2 anchor, Vector2 anchoredPos, string label)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            Image img = btnObj.AddComponent<Image>();
            img.sprite = CreateWhiteSprite();
            img.color = new Color(0.2f, 0.2f, 0.25f);
            Button btn = btnObj.AddComponent<Button>();

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(300f, 80f);

            Text text = CreateText(btnObj.transform, "Text", new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), 32, TextAnchor.MiddleCenter);
            text.text = label;
            RectTransform trt = text.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            return btn;
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchor, Vector2 anchoredPos, int fontSize, TextAnchor alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            Text text = obj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(600f, 80f);
            return text;
        }

        private static Sprite CreateWhiteSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
    }
}
