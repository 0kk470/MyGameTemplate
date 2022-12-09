using Saltyfish.Logic;
using System;
using System.Collections.Generic;

namespace Saltyfish.Logic.Entity
{
    public abstract class IEntityManager<T1,T2>:ManagerBase<T2> where T1: GameEntity where T2 : IEntityManager<T1, T2>
    {
        protected SortedDictionary<int, T1> m_Entities = new SortedDictionary<int, T1>();

        protected int m_InitEntityID = 0;

        public virtual void RegisterEntity(T1 entity)
        {
            if (entity == null)
                return;
            int iEntityID = AllocateID();
            entity.BindEntityID(iEntityID);
            m_Entities.Add(iEntityID, entity);
        }

        public virtual void DeleteEntity(int iEntityID)
        {
            if (m_Entities.ContainsKey(iEntityID))
            {
                if (m_Entities[iEntityID] != null)
                {
                    m_Entities[iEntityID].DestroyToPool();
                }
                m_Entities.Remove(iEntityID);
            }
        }

        public virtual void UnRegisterEntity(int iEntityID)
        {
            if (m_Entities.ContainsKey(iEntityID))
            {
                m_Entities.Remove(iEntityID);
            }
        }

        public virtual void DeleteAll()
        {
            foreach (var unit in m_Entities.Values)
            {
                unit.DestroyToPool();
            }
            m_Entities.Clear();
            m_InitEntityID = 0;
        }

        public virtual T1 GetEntity(int iEntityID)
        {
            m_Entities.TryGetValue(iEntityID, out T1 entity);
            return entity;
        }

        protected virtual int AllocateID() { return ++m_InitEntityID; }

        public virtual void OnUpdate(float deltaTime)
        {

        }

    }
}
