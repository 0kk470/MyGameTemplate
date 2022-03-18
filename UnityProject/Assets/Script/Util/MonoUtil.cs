using System;
using System.Collections.Generic;
using Saltyfish.ObjectPool;
using UnityEngine;

namespace Saltyfish.Util
{

    public interface IDataContainer<T>
    {
        void SetData(T data);
    }


    public static class MonoUtil
    {
        public static void GenerateMonoElements<T, data>(GameObject template, IList<data> dataList, Transform parent = null, Action<int, T, data> CreateElementCallback = null) where T : Component, IDataContainer<data>
        {
            if (template == null)
                return;
            for (int i = 0; i < dataList.Count; i++)
            {
                var go = GameObject.Instantiate(template, parent, false);
                var component = go.GetComponentAnyway<T>();
                component?.SetData(dataList[i]);
                CreateElementCallback?.Invoke(i, component, dataList[i]);
            }
        }


        public static void GenerateMonoElements<T, data>(string templatePath, IList<data> dataList, Transform parent = null, Action<int, T, data> CreateElementCallback = null) where T : Component, IDataContainer<data>
        {
            if (string.IsNullOrEmpty(templatePath))
                return;
            var prefab = Resources.Load<GameObject>(templatePath);
            GenerateMonoElements(prefab, dataList, parent, CreateElementCallback);
        }

        public static void GenerateMonoElementsWithPool<T, data>(string templatePath, IList<data> dataList, Transform parent = null, Action<int, T, data> CreateElementCallback = null) where T : Component, ICollectable, IDataContainer<data>
        {
            if (string.IsNullOrEmpty(templatePath))
                return;
            for (int i = 0; i < dataList.Count; i++)
            {
                var component = GoPoolMgr.CreateComponent<T>(templatePath, parent);
                component?.OnUsage();
                component?.SetData(dataList[i]);
                CreateElementCallback?.Invoke(i, component, dataList[i]);
            }
        }

        public static System.Collections.IEnumerator GenerateMonoElementsAsync<T, data>(string templatePath, IList<data> dataList, Transform parent = null, Action<int, T, data> CreateElementCallback = null, Action onComplete = null, float delta = 0f)
        where T : Component,IDataContainer<data>
        {
            var prefab = Resources.Load<GameObject>(templatePath);
            var comp = prefab.GetComponent<T>();
            return GenerateMonoElementsAsync(comp, dataList, parent, CreateElementCallback, onComplete, delta);
        }


        public static System.Collections.IEnumerator GenerateMonoElementsAsync<T, data>(T template, IList<data> dataList, Transform parent = null, Action<int, T, data> CreateElementCallback = null, Action onComplete = null, float delta = 0f)
        where T : Component, IDataContainer<data>
        {
            bool isValid = dataList != null && template != null;
            int index = 0;
            var waitTime = delta > 0 ? new WaitForSecondsRealtime(delta) : null;
            while (isValid && index < dataList.Count)
            {
                var go = GameObject.Instantiate(template.gameObject, parent, false);
                var component = go.GetComponentAnyway<T>();
                CreateElementCallback?.Invoke(index, component, dataList[index]);
                component?.SetData(dataList[index]);
                ++index;
                yield return waitTime;
            }
            onComplete?.Invoke();
        }
    }
}