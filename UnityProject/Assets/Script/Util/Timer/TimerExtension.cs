using System;
using UnityEngine;

namespace Saltyfish.Util
{
    public static class TimerExtension
    {
        public static Timer After(this MonoBehaviour mono, float duration, Action onComplete)
        {
            return Timer.Register(duration, onComplete, null, null, false, mono);
        }

        public static Timer Every(this MonoBehaviour mono, float duration, Action onComplete)
        {
            return Timer.Register(duration, onComplete, null, null, true, mono);
        }


        public static Timer CountDown(this MonoBehaviour mono, float duration, Action<float> onUpdateSecond = null, Action onComplete = null)
        {
            return Timer.Register(duration, onComplete, null, onUpdateSecond, false, mono);
        }
    }
}