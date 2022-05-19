using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saltyfish.ObjectPool
{
    /// <summary>
    /// 对象池接口回调
    /// </summary>
    public interface ICollectable
    {
        /// <summary>
        /// 当从对象池取出来使用时
        /// </summary>
        void OnUsage();


        /// <summary>
        /// 当被回收到对象池时
        /// </summary>
        void OnRecycle();
    }
}
