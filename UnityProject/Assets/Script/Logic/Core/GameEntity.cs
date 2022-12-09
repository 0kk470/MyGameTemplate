using Saltyfish.Data;
using Saltyfish.ObjectPool;
using UnityEngine;

namespace Saltyfish.Logic
{
    public class GameEntity : MonoBehaviour,ICollectable,IPoolBinder
    {
        public int iEntityID { get; private set; } = GameConfig.NO_ENTITY_ID;

        public string PoolName { get; set; }

        public GameEntity BindEntityID(int iEntityID)
        {
            this.iEntityID = iEntityID;
            return this;
        }


        public void ClearEntityID()
        {
            iEntityID = GameConfig.NO_ENTITY_ID;
        }

        public virtual void DestroyToPool()
        {
            GoPoolMgr.RecycleComponent(this, PoolName);
        }


        public virtual void OnRecycle()
        {
            
        }

        public virtual void OnUsage()
        {

        }
    }
}
