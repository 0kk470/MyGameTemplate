using Saltyfish.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Event
{

    /* 说明
      1.一般在某个类初始化的时候监听事件, 在类的析构时候注销事件
      比如在Monobehaviour的Awake和OnDestroy回调里监听和注销事件，或者在OnEnable和OnDisable里处理

      2.用泛型来处理参数避免值类型的装箱拆箱消耗

    */
    /// <summary>
    /// 全局事件派发器
    /// </summary>
    /// 
    public static class EventManager
    {
        private static Dictionary<GameEventType, List<Delegate>> m_EventListeners = new Dictionary<GameEventType, List<Delegate>>();

        private static void InsertListener(GameEventType eventType, Delegate handler)
        {
            if (!m_EventListeners.TryGetValue(eventType, out List<Delegate> listenerList))
            {
                listenerList = new List<Delegate>();
                m_EventListeners.Add(eventType, listenerList);
            }
            listenerList.Add(handler);
        }

        private static void EraseListener(GameEventType eventType, Delegate handler)
        {
            if (m_EventListeners.TryGetValue(eventType, out List<Delegate> listenerList))
            {
                listenerList.Remove(handler);
            }
        }

        public static void ClearAllListeners()
        {
            m_EventListeners.Clear();
        }

                             
        public static void Dump()
        {
            foreach(var listenerList in m_EventListeners.Values)
            {
                listenerList.PrintCollections();
            }
        }


        #region 订阅事件
        /// <summary>
        /// 无参数事件
        /// </summary>
        public static void AddListener(GameEventType eventType, Action handler)
        {
            InsertListener(eventType, handler);
        }

        /// <summary>
        /// 单参数事件
        /// </summary>
        public static void AddListener<A>(GameEventType eventType, Action<A> handler)
        {
            InsertListener(eventType, handler);
        }

        /// <summary>
        /// 双参数事件
        /// </summary>
        public static void AddListener<A, B>(GameEventType eventType, Action<A, B> handler)
        {
            InsertListener(eventType, handler);
        }

        /// <summary>
        /// 三参数事件
        /// </summary>
        public static void AddListener<A, B, C>(GameEventType eventType, Action<A, B, C> handler)
        {
            InsertListener(eventType, handler);
        }

        /// <summary>
        /// 四参数事件
        /// </summary>
        public static void AddListener<A, B, C, D>(GameEventType eventType, Action<A, B, C, D> handler)
        {
            InsertListener(eventType, handler);
        }
        #endregion

        #region 注销事件

        public static void RemoveListener(GameEventType eventType, Action handler)
        {
            EraseListener(eventType, handler);
        }

        public static void RemoveListener<A>(GameEventType eventType, Action<A> handler)
        {
            EraseListener(eventType, handler);
        }

        public static void RemoveListener<A, B>(GameEventType eventType, Action<A, B> handler)
        {
            EraseListener(eventType, handler);
        }

        public static void RemoveListener<A, B, C>(GameEventType eventType, Action<A, B, C> handler)
        {
            EraseListener(eventType, handler);
        }

        public static void RemoveListener<A, B, C, D>(GameEventType eventType, Action<A, B, C, D> handler)
        {
            EraseListener(eventType, handler);
        }
        #endregion

        #region 派发事件
        private static List<Delegate> GetEventListeners(GameEventType eventType)
        {
            m_EventListeners.TryGetValue(eventType, out List<Delegate> result);
            return result;
        }

        public static void DispatchEvent(GameEventType eventType)
        {
            var listeners = GetEventListeners(eventType);
            if(listeners != null)
            {
                foreach (var handler in listeners)
                {
                    try
                    {
                       ((Action)handler)();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        public static void DispatchEvent<A>(GameEventType eventType, A arg1)
        {
            var listeners = GetEventListeners(eventType);
            if (listeners != null)
            {
                foreach(var handler in listeners)
                {
                    try
                    {
                        ((Action<A>)handler)(arg1);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        public static void DispatchEvent<A, B>(GameEventType eventType, A arg1, B arg2)
        {
            var listeners = GetEventListeners(eventType);
            if (listeners != null)
            {
                foreach (var handler in listeners)
                {
                    try
                    {
                        ((Action<A,B>)handler)(arg1, arg2);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        public static void DispatchEvent<A, B, C>(GameEventType eventType, A arg1, B arg2, C arg3)
        {
            var listeners = GetEventListeners(eventType);
            if (listeners != null)
            {
                foreach (var handler in listeners)
                {
                    try
                    {
                        ((Action<A, B, C>)handler)(arg1, arg2, arg3);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        public static void DispatchEvent<A, B, C, D>(GameEventType eventType, A arg1, B arg2, C arg3, D arg4)
        {
            var listeners = GetEventListeners(eventType);
            if (listeners != null)
            {
                foreach (var handler in listeners)
                {
                    try
                    {
                        ((Action<A, B, C, D>)handler)(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        #endregion
    }
}
