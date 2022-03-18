using System.Collections.Generic;


namespace Saltyfish.ObjectPool
{
    public class ObjPool<T> where T : new()
    {
        Queue<T> m_Objs = new Queue<T>();

        public ObjPool(int nInitCapacity)
        {
            for (int i = 0; i < nInitCapacity; ++i)
            {
                m_Objs.Enqueue(new T());
            }
        }

        public T Get()
        {
            if (m_Objs.Count == 0)
            {
                T o = new T();
                return o;
            }
            else
            {
                return m_Objs.Dequeue();
            }
        }

        public void Release(T o)
        {
            m_Objs.Enqueue(o);
        }
    }
}