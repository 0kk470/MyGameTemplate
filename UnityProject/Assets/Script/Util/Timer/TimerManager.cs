using Saltyfish.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saltyfish.Util
{
    
    public class TimerManager : Singleton<TimerManager>
    {
        private readonly List<Timer> _timers = new List<Timer>();
        private readonly ObjPool<Timer> _timerPool = new ObjPool<Timer>(30);
        
        public Timer GetTimer()
        {
            return _timerPool.Get();
        }

        
        public void RecycleTimer(Timer timer)
        {
            if (timer != null)
            {
                _timerPool.Release(timer);
            }
        }
        

        public void Register(Timer timer)
        {
            if (timer == null) return;
            
            _timers.Add(timer);
        }

        
        public bool TryGetTimer(long id, out Timer result)
        {
            result = _timers.Find(toFind => toFind.Id == id);
            return result != null;
        }


        public void Cancel(long timerId)
        {
            if (TryGetTimer(timerId, out Timer timer))
            {
                timer.Reset();
                timer.IsDead = true;
            }
        }

        public void Pause(long timerID)
        {
            if (TryGetTimer(timerID, out Timer timer))
            {
                timer.Pause();
            }
        }

        public void Resume(long timerID)
        {
            if (TryGetTimer(timerID, out Timer timer))
            {
                timer.Resume();
            }
        }

        public void PauseAll()
        {
            foreach(var timer in _timers)
            {
                timer.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var timer in _timers)
            {
                timer.Resume();
            }
        }


        public void CancelAll()
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                Cancel(timer.Id);
                RecycleTimer(timer);
            }
            _timers.Clear();
        }
        

        public void Update(float deltaTime)
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                _timers[i].OnUpdate(deltaTime);
            }
            RemoveDeadTimer();
        }

        private void RemoveDeadTimer()
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                if (_timers[i].IsDead)
                {
                    RecycleTimer(_timers[i]);
                    _timers.RemoveAt(i);
                }
            }
        }
    }
}