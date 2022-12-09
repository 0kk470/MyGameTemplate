using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Saltyfish.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Saltyfish.Resource
{
    using UObject = UnityEngine.Object;

    public class AssetCache
    {

        #region static members

        private static AssetCache Default => GetCache("Default");

        private static Dictionary<string, AssetCache> m_AllCache = new Dictionary<string, AssetCache>();

        private static Dictionary<string, AssetBundle> m_Bundles = new Dictionary<string, AssetBundle>();

        private static Dictionary<string, string> m_AssetPath2ABPath = new Dictionary<string, string>();

        private string m_CacheName;

        public static async UniTask<bool> RegisterBundleAsync(string bundlePath)
        {
            if (m_Bundles.ContainsKey(bundlePath))
                return true;
            if(!File.Exists(bundlePath))
            {
                Debug.LogError("AB包不存在, 路径:" + bundlePath);
                return false;
            }
            var bundle = await AssetBundle.LoadFromFileAsync(bundlePath);
            if (bundle == null)
                return false;
            m_Bundles.Add(bundlePath, bundle);
            var allPaths = bundle.GetAllAssetNames();
            foreach(var assetPath in allPaths)
            {
                m_AssetPath2ABPath[assetPath] = bundlePath;
            }
            return true;
        }

        public static void UnRegisterBundle(string bundlePath)
        {
            if (m_AllCache.ContainsKey(bundlePath))
            {
                m_AllCache[bundlePath].Dispose();
            }
        }


        private static AssetCache CreateCache(string cacheName)
        {
            return new AssetCache(cacheName);
        }

        private static AssetCache CreateBundleCache(string bundlePath)
        {
            var cache = new AssetCache(bundlePath);
            cache.OnCustomLoad += LoadAssetFromBundle;
            cache.OnCustomLoadAsync += LoadAssetFromBundleAsync;
            return cache;
        }


        private static UObject LoadAssetFromBundle(string assetPath, string bundlePath, Type t)
        {
            AssetBundle bundle;
            if (!m_Bundles.ContainsKey(bundlePath))
            {
                bundle = AssetBundle.LoadFromFile(bundlePath);
                m_Bundles.Add(bundlePath, bundle);
            }
            else
            {
                bundle = m_Bundles[bundlePath];
            }
            assetPath = ToBundleAssetPath(assetPath);
            return bundle?.LoadAsset(assetPath, t);
        }

        private static AssetBundleRequest LoadAssetFromBundleAsync(string assetPath, string bundlePath, Type t)
        {
            AssetBundle bundle;
            if (!m_Bundles.ContainsKey(bundlePath))
            {
                bundle = AssetBundle.LoadFromFile(bundlePath);
                m_Bundles.Add(bundlePath, bundle);
            }
            else
            {
                bundle = m_Bundles[bundlePath];
            }
            assetPath = ToBundleAssetPath(assetPath);
            return bundle?.LoadAssetAsync(assetPath);
        }

        private static AssetCache GetBundleCache(string bundlePath)
        {
            if (!m_AllCache.TryGetValue(bundlePath, out AssetCache result))
            {
                result = CreateBundleCache(bundlePath);
                m_AllCache.Add(bundlePath, result);
            }
            return result;
        }


        private static AssetCache GetCache(string cacheName)
        {
            if (!m_AllCache.TryGetValue(cacheName, out AssetCache result))
            {
                result = CreateCache(cacheName);
                m_AllCache.Add(cacheName, result);
            }
            return result;
        }


        private static string ToBundleAssetPath(string assetPath)
        {
            var lowerPath = assetPath.ToLower();
            if (!lowerPath.StartsWith("assets/"))
            {
                assetPath = "assets/" + assetPath;
            }
            return assetPath;
        }

        private static string ToResourcesPath(string assetPath)
        {
            var findStr = "Resources/";
            var idx = assetPath.IndexOf(findStr);
            if (idx != -1)
            {
                assetPath = assetPath.Substring(idx + findStr.Length);
            }
            assetPath = System.IO.Path.ChangeExtension(assetPath, null);
            return assetPath;
        }

        public static void DisposeAllCache()
        {
            var cachesToClear = m_AllCache.Values.ToList();
            foreach (var cache in cachesToClear)
            {
                cache.Dispose();
            }
            m_AllCache.Clear();
            m_Bundles.Clear();
        }

        private static bool TryFindBundlePath(string assetPath, out string bundlePath)
        {
            assetPath = assetPath.ToLower(); //ab所有路径都为小写
            if(!m_AssetPath2ABPath.TryGetValue(assetPath, out bundlePath))
            {
                bundlePath = string.Empty;
                return false;
            }
            return true;
        }


        public static T LoadAsset<T>(string assetPath) where T : UObject
        {
            bool isBunlded = TryFindBundlePath(assetPath, out string bundlePath);
            if(!isBunlded)
            {
                return Default.GetAsset<T>(assetPath);
            }
            else
            {
                return GetBundleCache(bundlePath).GetAsset<T>(assetPath);
            }
        }

        public static async UniTask<T> LoadAssetAsync<T>(string assetPath) where T : UObject
        {
            bool isBunlded = TryFindBundlePath(assetPath, out string bundlePath);
            var result = default(T);
            if (isBunlded)
            {
                result = await GetBundleCache(bundlePath).GetAssetAsync<T>(assetPath);

            }
            else
            {
                result = await Default.GetAssetAsync<T>(assetPath);
            }
            return result;
        }

        #endregion

        #region Self Members

        private bool m_IsDisposed { get; set; }

        private Dictionary<string, UObject> m_AssetDic = new Dictionary<string, UObject>();

        public event Func<string, string, Type, UObject> OnCustomLoad;

        public event Func<string, string, Type, ResourceRequest> OnCustomLoadAsync;

        private AssetCache() { }

        private AssetCache(string cacheName) { m_CacheName = cacheName; }


        private T GetAsset<T>(string assetPath) where T : UObject
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Error assetPath is null");
                return null;
            }

            if (!m_AssetDic.TryGetValue(assetPath, out UObject result))
            {
                result = OnCustomLoad?.Invoke(assetPath, m_CacheName, typeof(T));
                if(result == null)
                    result = Resources.Load<T>(ToResourcesPath(assetPath));
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

        private async UniTask<T> GetAssetAsync<T>(string assetPath) where T : UObject
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Error assetPath is null");
                return null;
            }
            if (!m_AssetDic.TryGetValue(assetPath, out UObject result))
            {
                var operation = OnCustomLoadAsync?.Invoke(assetPath, m_CacheName, typeof(T));
                if (operation == null)
                    operation = Resources.LoadAsync<T>(ToResourcesPath(assetPath));
                result = await operation;
                if (result != null)
                {
                    m_AssetDic[assetPath] = result;
                }
                else
                {
                    Debug.LogErrorFormat("Asset does not exist, path:[{0}], CacheName:[{1}]", assetPath, m_CacheName);
                }
                    
            }
            return result as T;
        }

        public void Dispose()
        {
            m_AllCache.Remove(m_CacheName);
            OnCustomLoad = null;
            OnCustomLoadAsync = null;
            m_AssetDic.Clear();
            UnloadRelatedBundle();
            m_IsDisposed = true;
        }

        private void UnloadRelatedBundle()
        {
            if (m_Bundles.TryGetValue(m_CacheName, out AssetBundle bundle))
            {
                var allPaths = bundle.GetAllAssetNames();
                foreach (var assetPath in allPaths)
                {
                    m_AssetPath2ABPath.Remove(assetPath);
                }
                m_Bundles.Remove(m_CacheName);
                bundle.Unload(true);
            }
        }

        #endregion
    }
}
