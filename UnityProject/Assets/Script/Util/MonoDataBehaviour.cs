using Saltyfish.Util;
using UnityEngine;

namespace Saltyfish.Util
{
    public class MonoDataBehaviour<T>:MonoBehaviour, IDataContainer<T>
    {
        [SerializeField]
        protected T m_Data;

        public virtual void SetData(T data)
        {
            m_Data = data;
        }
    }
}
