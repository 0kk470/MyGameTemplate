using System;
using System.Diagnostics;
using UnityEngine;

namespace Saltyfish.Util
{
    public class Timer
    {
        private static long _allocTimerId = 1;
        public static long AllocTimerId
        {
            get { return _allocTimerId++; }
        }

        public long Id { get; private set; }

        public float Duration { get; set; }
        public bool IsLooped { get; set; }
        public bool IsOwnerDestroyed => this.m_IsAttached && this.m_AttachObject == null;
        private Action _onComplete;
        private Action<float> _OnFrame;
        private Action<float> _OnElapsedSecond;
        private float m_BeginTime;
        private float _secondTriggerTime;
        private bool _isScaleTime;
        private bool _isPaused = false;

        private UnityEngine.Object m_AttachObject;
        private bool m_IsAttached;

        private bool m_isDead = false;
        public bool IsDead
        {
            get { return m_isDead; }
            set { m_isDead = value; }
        }


        public static Timer Register(float duration,
            Action onComplete = null,
            Action<float> onUpdateFrame = null,
            Action<float> onUpdateSecond = null,
            bool isLooped = false,
            MonoBehaviour attachedObject = null,
            bool isScaleTime = false)
        {

            var timer = TimerManager.Instance.GetTimer();
            timer.Reset();
            timer.Id = AllocTimerId;

            timer.Duration = duration;
            timer._onComplete = onComplete;
            timer._OnElapsedSecond = onUpdateSecond;
            timer._OnFrame = onUpdateFrame;
            timer.IsLooped = isLooped;
            timer._isScaleTime = isScaleTime;

            timer.m_AttachObject = attachedObject;
            timer.m_IsAttached = attachedObject != null;

            timer.m_BeginTime = timer.NowTime();
            timer._secondTriggerTime = timer.m_BeginTime;
            TimerManager.Instance.Register(timer);

            return timer;
        }


        public static Timer After(float duration, Action onComplete)
        {
            return Register(duration, onComplete);
        }

        public static Timer Every(float duration, Action onComplete)
        {
            return Register(duration, onComplete, null, null, true);
        }

        public static Timer CountDown(float duration, Action<float> onElapsedSecond = null, Action onComplete = null)
        {
            return Register(duration, onComplete, null, onElapsedSecond);
        }

        public static void Cancel(long timerId)
        {
            if (timerId <= 0)
                return;

            TimerManager.Instance.Cancel(timerId);
        }


        public void Reset()
        {
            Id = 0;
            Duration = 0;
            IsLooped = false;
            _onComplete = null;
            _OnFrame = null;
            _OnElapsedSecond = null;
            m_BeginTime = 0;
            _secondTriggerTime = 0;
            m_AttachObject = null;
            m_IsAttached = false;
            _isPaused = false;
            IsDead = false;
        }

        public Timer SetLoop(bool isLoop)
        {
            IsLooped = isLoop;
            return this;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }


        private float GetBeginTime()
        {
            return m_BeginTime + Duration;
        }


        private float NowTime()
        {
            return _isScaleTime ? Time.time : Time.realtimeSinceStartup;
        }


        public float ElapsedSeconds()
        {
            return NowTime() - m_BeginTime;
        }


        public void OnUpdate(float deltaTime)
        {
            if (IsDead) return;

            if (IsOwnerDestroyed)
            {
                Cancel(Id);
                return;
            }

            if (_isPaused)
                return;

            float CurrentTime = NowTime();
            float fElapsedSeconds = ElapsedSeconds();

            if (Duration >= 0 && CurrentTime >= GetBeginTime())
            {
                _onComplete?.Invoke();

                if (IsLooped)
                {
                    m_BeginTime = CurrentTime;
                    _secondTriggerTime = CurrentTime;
                }
                else
                {
                    Cancel(Id);
                    return;
                }
            }
            _OnFrame?.Invoke(deltaTime);

            if (CurrentTime - _secondTriggerTime >= 1)
            {
                _secondTriggerTime = CurrentTime;
                _OnElapsedSecond?.Invoke(fElapsedSeconds);
            }
        }
    }
}