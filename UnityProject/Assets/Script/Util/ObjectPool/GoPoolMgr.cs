using Saltyfish.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Saltyfish.ObjectPool
{
    /// <summary>
    /// 对象池管理器 
    /// 一般来说直接从Resources目录直接加载，默认对象池名为Reosurces目录下的加载路径
    /// 也可以直接手动创建
    /// </summary>
    public class GoPoolMgr : MonoSingleton<GoPoolMgr>
    {
        private Dictionary<string, GameObjectPool> m_Pools = new Dictionary<string, GameObjectPool>();

        public GameObjectPool CreatePool(string poolName, GameObject prefab, int initSize = 5)
        {
            if (m_Pools.ContainsKey(poolName))
            {
#if UNITY_EDITOR
                Debug.LogWarning("Pool is already existed");
#endif
                return m_Pools[poolName];
            }
            var newPoolObject = new GameObject(poolName);
            var pool = new GameObjectPool(poolName);
            newPoolObject.gameObject.SetActive(false);
            newPoolObject.transform.SetParent(transform, false);
            pool.Init(prefab, newPoolObject.transform, initSize);
            m_Pools.Add(poolName, pool);
            return pool;
        }

        public GameObjectPool CreatePool<T>(string poolName, T monoObject, int initSize = 5) where T:Component
        {
            return CreatePool(poolName, monoObject.gameObject, initSize);
        }

        public void DeletePool(string poolName)
        {
            if (m_Pools.ContainsKey(poolName))
            {
                var pool = m_Pools[poolName];
                m_Pools.Remove(poolName);
                if (pool != null)
                {
                    pool.Dispose();
                }
            }
        }

        [ContextMenu("销毁所有缓存对象")]
        public void DeleteAllPools()
        {
            if (m_Pools.Count == 0)
                return;
            var allPoolName = m_Pools.Keys.ToList();
            foreach (var poolName in allPoolName)
            {
                DeletePool(poolName);
            }
        }

        public GameObjectPool GetPool(string poolName)
        {
            GameObjectPool pool = null;
            m_Pools.TryGetValue(poolName, out pool);
            return pool;
        }

        public GameObjectPool GetPoolByResourcePath(string path, int cacheNum)
        {
            if (!IsValidResourcePath(path))
                return null;
            var pool = GetPool(path);
            if (pool == null)
            {
                var prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogErrorFormat("Load GameObject Failed, path Name:{0}", path);
                    return null;
                }
                pool = CreatePool(path, prefab, cacheNum);
            }
            return pool;
        }

        private GameObject GetGameObject(string poolName)
        {
            var pool = GetPool(poolName);
            if (pool != null)
            {
                return pool.GetObject();
            }
            else
            {
                Debug.LogErrorFormat("Error,Pool {0} is null", poolName);
                return null;
            }
        }

        private void CollectGameObject(string poolName, GameObject go)
        {
            if (go != null)
            {
                if (string.IsNullOrEmpty(poolName))
                {
                    Debug.LogError("PoolName is null,Destroy this Object:" + go.name);
                    Destroy(go);
                    return;
                }
                var pool = GetPool(poolName);
                if (pool != null)
                {
                    pool.Collect(go);
                }
                else
                {
                    Debug.LogWarningFormat("<color=red>Cannot Find Pool【{0}】,Destroy this Object:{1}</color>", poolName, go.name);
                    Destroy(go);
                }
            }
        }

        private T GetComponentFromPool<T>(string poolName) where T : Component
        {
            var go = GetGameObject(poolName);
            if (go != null)
            {
                var comp = go.GetComponentAnyway<T>();
                if(comp is ICollectable collectable)
                {
                    collectable.OnUsage();
                }
                return comp;
            }
            return null;
        }

        private void CollectComponentToPool<T>(string poolName, T comp) where T : Component
        {
            if (comp != null)
            {
                if (string.IsNullOrEmpty(poolName))
                {
                    Debug.LogError("PoolName is null,Destroy this Object:" + comp.gameObject.name);
                    Destroy(comp.gameObject);
                    return;
                }
                if(comp is ICollectable collectable)
                {
                    collectable.OnRecycle();
                }
                CollectGameObject(poolName, comp.gameObject);
            }
        }

        private static bool IsValidResourcePath(string _path)
        {
            if (string.IsNullOrEmpty(_path))
            {
                Debug.LogError("Path is null or empty");
                return false;
            }
            return true;
        }

        public GameObject InstantiateFromPool(string path, Transform parent = null, Vector3 localPosition = default(Vector3), int cacheNum = 1)
        {
            var pool = GetPoolByResourcePath(path, cacheNum);
            if (pool == null)
                return null;
            var go = pool.GetObject();
            if (go != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = localPosition;
            }
            return go;
        }

        public GameObject Instantiate_NoPool(string path, Transform parent = null, Vector3 localPosition = default(Vector3))
        {
            if (!IsValidResourcePath(path))
                return null;
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogErrorFormat("Load GameObject Failed, path Name:{0}", path);
                return null;
            }
            var go = Instantiate(prefab);
            if (go != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = localPosition;
            }
            return go;
        }

        #region 静态接口（免得每次调用多写个Instance）

        /// <summary>
        /// 可以理解为直接从Resource目录加载对应的Prefab缓存，然后Instantiate
        /// </summary>
        /// <param name="path">相对于Resources</param>
        /// <param name="parent">父节点</param>
        /// <param name="localPosition">相对坐标</param>
        /// <param name="cacheNum">缓存数量</param>
        /// <returns></returns>
        public static GameObject CreateGameObject(string path, Transform parent = null, Vector3 localPosition = default(Vector3), int cacheNum = 1)
        {
            return Instance?.InstantiateFromPool(path, parent, localPosition, cacheNum);
        }

        public static GameObject CreateGameObject_NoPool(string path, Transform parent = null, Vector3 localPosition = default(Vector3))
        {
            return Instance?.Instantiate_NoPool(path, parent, localPosition);
        }

        public static void RecycleGameObject(GameObject target, string path)
        {
            Instance?.CollectGameObject(path, target);
        }


        public static T CreateComponent<T>(string path, bool isActive = true, Transform parent = null, Vector3 localPosition = default(Vector3), int cacheNum = 1) where T : Component
        {
            var pool = Instance.GetPoolByResourcePath(path, cacheNum);
            if (pool == null)
                return null;
            var comp = Instance.GetComponentFromPool<T>(path);
            if (comp != null)
            {
                comp.gameObject.BetterSetActive(isActive);
                comp.transform.SetParent(parent, false);
                comp.transform.localPosition = localPosition;
            }
            return comp;
        }

        public static void RecycleComponent<T>(T comp, string path) where T:Component
        {
            Instance?.CollectComponentToPool(path, comp);
        }



        #endregion


        [ContextMenu("查看对象池情况")]
        public void Dump()
        {
            Func<GameObjectPool, string> OnPrint = pool =>
            {
                return string.Format($"对象池[{pool.PoolName}],缓存数量:{pool.Size}\n");
            };
            m_Pools.Values.PrintCollections(OnPrint);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_Pools.Clear();
        }
    }
}
