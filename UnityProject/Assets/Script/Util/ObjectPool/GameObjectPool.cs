using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saltyfish.ObjectPool
{
 
    public class GameObjectPool
    {
        public const int MAX_CAPACITY = 150;
        public int Size
        {
            get
            {
                return m_Objects.Count;
            }
        }

        public int Capacity { get; private set; }

        public string PoolName { get; private set; }

        public GameObject template { get; private set; }

        public Transform container { get; private set; }

        private Stack<GameObject> m_Objects = new Stack<GameObject>();

        public GameObjectPool(string poolName)
        {
            PoolName = poolName;
        }

        public void Init(GameObject prefabSource, Transform parent, int initSize = 10)
        {
            template = GameObject.Instantiate(prefabSource, parent, false);
            container = parent;
            Capacity = initSize * 2;
            for (int i = 0; i < initSize; i++)
            {
                m_Objects.Push(GameObject.Instantiate(template, container, false));
            }
        }

        public bool Collect(GameObject go)
        {
            if (go == null)
            {
                Debug.LogWarning("Try to collect null GameObject");
                return false;
            }
            if (Size >= MAX_CAPACITY)
            {
                Debug.LogError("GameObject Pool is too large");
                GameObject.Destroy(go);
                return false;
            }
            if (Size >= Capacity)
            {
                Capacity = Math.Min(MAX_CAPACITY, 2 * Capacity);
            }
            go.transform.SetParent(container, false);
            m_Objects.Push(go);
            return true;
        }

        public GameObject GetObject()
        {
            if (Size > 0)
            {
                var obj = m_Objects.Pop();
                return obj;
            }
            else
            {
                return GameObject.Instantiate(template);
            }
        }

        public void Dispose()
        {
            m_Objects.Clear();
            GameObject.Destroy(container.gameObject);
        }
    }
}
