using System.Collections;
using UnityEngine;
using TopDownSurvivor.Core;

namespace TopDownSurvivor.Systems
{
    /// <summary>
    /// 屏幕震动：相机短促抖动，幅度随时间衰减，增强打击感。
    /// </summary>
    public class CameraShake : Singleton<CameraShake>
    {
        [SerializeField] private float _defaultDuration = 0.2f;  // 默认震动时长
        [SerializeField] private float _defaultMagnitude = 0.3f; // 默认震动幅度

        private Vector3 _originalPos;

        private void Awake()
        {
            base.Awake();
            _originalPos = transform.localPosition;
        }

        /// <summary>触发一次屏幕震动。</summary>
        public void Shake(float duration, float magnitude)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        /// <summary>用默认参数触发震动。</summary>
        public void Shake()
        {
            Shake(_defaultDuration, _defaultMagnitude);
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float damping = 1f - t / duration; // 幅度随时间衰减到 0
                Vector2 offset = Random.insideUnitCircle * magnitude * damping;
                transform.localPosition = _originalPos + new Vector3(offset.x, offset.y, 0f);
                yield return null;
            }

            transform.localPosition = _originalPos; // 恢复原位
        }
    }
}
