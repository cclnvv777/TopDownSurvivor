using System;

namespace TopDownSurvivor.Core
{
    /// <summary>
    /// 轻量类型安全事件总线。按事件类型 T 分离，避免字符串魔法值。
    /// 用法：EventBus&lt;XxxEvent&gt;.Subscribe(...) / Publish(...)。
    /// </summary>
    public static class EventBus<T>
    {
        private static event Action<T> _listeners;

        public static void Subscribe(Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }

            _listeners += handler;
        }

        public static void Unsubscribe(Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }

            _listeners -= handler;
        }

        public static void Publish(T payload)
        {
            _listeners?.Invoke(payload);
        }

        public static void Clear()
        {
            _listeners = null;
        }
    }
}
