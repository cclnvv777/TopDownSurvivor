using System.Collections.Generic;
using UnityEngine;

namespace TopDownSurvivor.Core
{
    /// <summary>
    /// 通用对象池：预创建一批对象，用的时候取出，用完放回，避免频繁创建/销毁。
    /// 内部用「队列（Queue，先进先出）」管理空闲对象。
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Queue<T> _pool = new Queue<T>();

        /// <param name="prefab">要复用的预制体</param>
        /// <param name="initialSize">预创建的数量</param>
        public ObjectPool(T prefab, int initialSize)
        {
            _prefab = prefab;
            for (int i = 0; i < initialSize; i++)
            {
                T obj = CreateNew();
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        private T CreateNew()
        {
            return Object.Instantiate(_prefab);
        }

        /// <summary>
        /// 从池里取出一个可用对象（池空时会临时新建）。
        /// </summary>
        public T Get()
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 把对象放回池里（不销毁，只是隐藏）。
        /// </summary>
        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
