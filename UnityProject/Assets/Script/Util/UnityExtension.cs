using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Saltyfish.Util
{
    /// <summary>
    /// 常用的Unity逻辑拓展和优化
    /// </summary>
    public static class UnityExtension
    {
        /// <summary>
        /// 避免状态不变的情况下重复跨语言调用C++底层
        /// </summary>
        /// <param name="go"></param>
        /// <param name="bActive"></param>
        public static void BetterSetActive(this GameObject go, bool bActive)
        {
            if (go != null && go.activeSelf != bActive)
            {
                go.SetActive(bActive);
            }
        }

        public static GameObject FindChild(this GameObject pParent, string strFindName)
        {
            if (pParent == null)
                return null;
            var pFind = pParent.transform.Find(strFindName);
            if (pFind != null)
                return pFind.gameObject;
            for (var i = 0; i < pParent.transform.childCount; i++)
            {
                var Child = pParent.transform.GetChild(i);

                GameObject findedGameObject = FindChild(Child.gameObject, strFindName);
                if (findedGameObject)
                    return findedGameObject;
            }
            return null;
        }

        public static T GetComponentAnyway<T>(this Component comp) where T : Component
        {
            var component = comp.GetComponent<T>();
            if (component == null)
                component = comp.gameObject.AddComponent<T>();
            return component;
        }

        public static Component GetComponentAnyWay(this Component comp, Type t)
        {
            var component = comp.GetComponent(t);
            if (component == null)
                component = comp.gameObject.AddComponent(t);
            return component;
        }

        public static T GetComponentAnyway<T>(this GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        public static List<T> GetComponentsInChildren_NoRecursive<T>(this Component comp)
        {
            var results = new List<T>();
            if (comp == null)
            {
                return results;
            }
            var transform = comp.transform;
            for (int i = 0; i < transform.childCount; ++i)
            {
                var c = transform.GetChild(i).GetComponent<T>();
                if (c != null)
                {
                    results.Add(c);
                }
            }
            return results;
        }

        public static T FindChildGetComponentAnyway<T>(this Component comp, string szName) where T : Component
        {
            if (comp == null)
            {
                return null;
            }
            var child = FindChild(comp.gameObject, szName);
            if (child != null)
            {
                var component = child.GetComponent<T>() ?? child.AddComponent<T>();
                return component;
            }
            return null;
        }

        public static bool ContainsLayer(this LayerMask target, int layer)
        {
            return (target.value & layer) == layer;
        }

        public static int GetMaskWithout(params string[] layerNames)
        {
            var excludeLayer = LayerMask.GetMask(layerNames);
            return GetMaskWithout(excludeLayer);
        }

        public static int GetMaskWithout(int excludeLayer)
        {
            var everyLayer = ~0;
            return (everyLayer &= (~excludeLayer));
        }

        #region Random
        public static int InclusiveRange(int start, int end)
        {
            return UnityEngine.Random.Range(start, end + 1);
        }
        #endregion

        #region Color Extension
        public static UnityEngine.Color HtmlString2Color(string colorStr)
        {
            UnityEngine.Color col = UnityEngine.Color.white;
            ColorUtility.TryParseHtmlString(colorStr, out col);
            return col;
        }

        #endregion

        #region UI Extension
        public static void SetBlockingMask(this GraphicRaycaster gRaycaster, int maskLayer)
        {
            if (gRaycaster != null)
            {
                var fieldInfo = gRaycaster.GetType().GetField("m_BlockingMask", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    LayerMask layerMask = new LayerMask();
                    layerMask.value = maskLayer;
                    fieldInfo.SetValue(gRaycaster, layerMask);
                }
            }
        }
        #endregion
    }
}
