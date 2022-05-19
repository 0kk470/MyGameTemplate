using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Saltyfish.ObjectPool;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource :ILoopScrollCache
    {
        [Tooltip("Resource相对目录")]
        public string PrefabPath;

        public int poolSize = 5;

        private bool inited = false;

        public Action<Component> onObjectReturn;

        public virtual GameObject GetObject()
        {
            return GoPoolMgr.CreateGameObject(PrefabPath, null, Vector3.zero, poolSize);
        }

        public virtual void ReturnObject(Transform go)
        {
            onObjectReturn?.Invoke(go);
            GoPoolMgr.RecycleGameObject(go.gameObject, PrefabPath);
        }

        private Dictionary<int, Component> m_ComponentCache = new Dictionary<int, Component>();

        private Type m_ComponentType = typeof(Transform);

        public void SetComponentType(Type t)
        {
            if(!t.IsSubclassOf(typeof(Component)))
                return;
            m_ComponentType = t;
        }

        public void Clear()
        {
            onObjectReturn = null;
            m_ComponentCache.Clear();
        }

        public Component GetCacheComponent(Transform transform)
        {
            Component component = null;
            if (transform != null)
            {
                if (m_ComponentType == typeof(Transform))
                    component = transform;
                else
                {
                    var instanceID = transform.GetInstanceID();
                    if (!m_ComponentCache.TryGetValue(instanceID, out component))
                    {
                        component = transform.GetComponent(m_ComponentType);
                        m_ComponentCache.Add(instanceID, component);
                    }
                }
            }
            return component;
        }
    }
}
