using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    public virtual void Init() { }
}

public abstract class ManagerBase<T> : ManagerBase where T:ManagerBase<T>
{
    // Instance Indexer 
    public static T Instance { get; private set; }

    /// <summary>
    /// Initialize manager
    /// </summary>
    public override void Init()
    {
        Instance = this as T;
    }
}