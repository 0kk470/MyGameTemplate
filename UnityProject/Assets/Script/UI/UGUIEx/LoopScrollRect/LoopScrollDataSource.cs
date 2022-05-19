using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{

    public interface ILoopScrollCache
    {
        void SetComponentType(Type t);

        void Clear();

        Component GetCacheComponent(Transform transform);
    }

    public abstract class LoopScrollDataSource:ILoopScrollCache
    {
        public abstract void ProvideData(Transform transform, int idx);
        public Action<Component, int> onData;

        public abstract void SetComponentType(Type t);

        public abstract void Clear();

        public abstract Component GetCacheComponent(Transform transform);
    }

	public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
		public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

        private Dictionary<int, Component> m_ComponentCache = new Dictionary<int, Component>();

        private Type m_ComponentType = typeof(Transform);

		public LoopScrollSendIndexSource(){}

        public override void ProvideData(Transform transform, int idx)
        {
            if(transform == null)
            {
                Debug.LogError("transform is null, idx :" + idx);
                return;
            }
            Component component = GetCacheComponent(transform);
            onData?.Invoke(component, idx);
        }

        

        public override void SetComponentType(Type t)
        {
            if(!t.IsSubclassOf(typeof(Component)))
                return;
            m_ComponentType = t;
        }

        public override void Clear()
        {
            onData = null;
            m_ComponentCache.Clear();
        }

        public override Component GetCacheComponent(Transform transform)
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

	public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

		public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellContent", objectsToFill[idx]);
        }

        public override void SetComponentType(Type t)
        {
            
        }

        public override Component GetCacheComponent(Transform transform)
        {
            return null;
        }

        public override void Clear()
        {
            this.objectsToFill = null;
        }
    }
}