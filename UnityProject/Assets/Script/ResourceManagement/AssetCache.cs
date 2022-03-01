using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Resource
{
    using UObject = UnityEngine.Object;

    public class AssetCache
    {

        public static AssetCache Default { get; private set; } = new AssetCache("Default");

        private static Dictionary<string, AssetCache> m_AllCache = new Dictionary<string, AssetCache>();


        private string m_CacheName;

        private bool m_IsDisposed { get; set; }

        private Dictionary<string, UObject> m_AssetDic = new Dictionary<string, UObject>();

        public event Func<string, UObject> OnCustomLoad;

        private AssetCache() { }

        private AssetCache(string cacheName) { m_CacheName = cacheName; }

        public static AssetCache CreateCache(string cacheName)
        {
            return new AssetCache(cacheName);
        }

        public static AssetCache Get(string cacheName)
        {
            if(!m_AllCache.TryGetValue(cacheName, out AssetCache result))
            {
                result = CreateCache(cacheName);
                m_AllCache.Add(cacheName, result);
            }
            return result;
        }


        public T GetAsset<T>(string assetPath) where T:UObject
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Error assetPath is null");
                return null;
            }
            if (!m_AssetDic.TryGetValue(assetPath, out UObject result))
            {
                result = OnCustomLoad?.Invoke(assetPath);
                if(result == null)
                    result = Resources.Load<T>(assetPath);
                if (result != null)
                {
                    m_AssetDic.Add(assetPath, result);
                }
                else
                {
                    Debug.LogErrorFormat("Load [{0}] Failed", assetPath);
                }
            }
            return result as T;
        }

        public void GetAssetAsync<T>(string assetPath, Action<T> OnLoadComplete) where T : UObject
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Error assetPath is null");
                return;
            }
            if (!m_AssetDic.TryGetValue(assetPath, out UObject result))
            {
                Util.CoroutineRunner.Instance.StartCoroutine(Co_GetAssetAsync(assetPath, OnLoadComplete));
            }
            else
            {
                OnLoadComplete?.Invoke(result as T);
            }
        }

        private IEnumerator Co_GetAssetAsync<T>(string assetPath, Action<T> onComplete) where T:UObject
        {
            var operation = Resources.LoadAsync<T>(assetPath);
            while (!operation.isDone && !m_IsDisposed)
                yield return null;
            if (!m_IsDisposed)
            {
                var result = operation.asset as T;
                if (result != null)
                {
                    if (!m_AssetDic.ContainsKey(assetPath))
                        m_AssetDic.Add(assetPath, result);
                    onComplete?.Invoke(result);
                }
                else
                {
                    Debug.LogErrorFormat("Load [{0}] Failed", assetPath);
                }
            }
        }


        public void Dispose()
        {
            m_IsDisposed = true;
            m_AssetDic.Clear();
        }

        public static void DisposeAllCache()
        {
            foreach(var cache in m_AllCache.Values)
            {
                cache.Dispose();
            }
        }
    }
}
