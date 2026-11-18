using UnityEngine;

namespace TopDownSurvivor.Core
{
    /// <summary>
    /// MonoBehaviour 单例基类。
    /// 子类通过 Instance 访问；场景中重复挂载时自动销毁多余实例。
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        // 场景里没有则自动创建，避免空引用
                        _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }
    }
}
