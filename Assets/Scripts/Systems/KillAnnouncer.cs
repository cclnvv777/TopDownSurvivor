using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TopDownSurvivor.Core;
using TopDownSurvivor.Enemies;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 击杀/连杀提示：击杀敌人时屏幕弹出提示，2 秒内连续击杀触发连杀计数。
    /// </summary>
    public class KillAnnouncer : MonoBehaviour
    {
        [SerializeField] private float _comboWindow = 2f;    // 连杀判定窗口（秒）
        [SerializeField] private float _displayDuration = 1.2f; // 提示显示时长

        private int _combo;
        private float _lastKillTime;
        private Text _text;
        private GameObject _textObj;
        private Coroutine _hideRoutine;

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

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            // 2 秒内连续击杀 → 连杀 +1；否则重新计数
            _combo = (Time.time - _lastKillTime <= _comboWindow) ? _combo + 1 : 1;
            _lastKillTime = Time.time;

            string msg = _combo > 1 ? "连杀 x" + _combo + "!" : "击杀 +1";
            Show(msg);
        }

        private void Show(string msg)
        {
            _text.text = msg;
            _textObj.SetActive(true);

            if (_hideRoutine != null)
            {
                StopCoroutine(_hideRoutine);
            }
            _hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displayDuration);
            _textObj.SetActive(false);
        }

        private void BuildUI()
        {
            GameObject canvasObj = new GameObject("KillAnnounceCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50; // 在 HUD 之上、暂停菜单之下
            canvasObj.AddComponent<CanvasScaler>();

            GameObject textObj = new GameObject("KillText");
            textObj.transform.SetParent(canvasObj.transform, false);
            _text = textObj.AddComponent<Text>();
            _text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _text.fontSize = 40;
            _text.fontStyle = FontStyle.Bold;
            _text.color = new Color(1f, 0.85f, 0.3f); // 金色
            _text.alignment = TextAnchor.MiddleCenter;

            RectTransform rt = _text.rectTransform;
            rt.anchoredPosition = new Vector2(0f, 360f);
            rt.sizeDelta = new Vector2(400f, 60f);

            _textObj = textObj;
            _textObj.SetActive(false);
        }
    }
}
