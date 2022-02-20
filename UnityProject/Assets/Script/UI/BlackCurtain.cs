using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace Saltyfish.UI
{
    public class BlackCurtain : UIBase
    {
        private Image m_CurtainImage;

        private Action m_CompleteCallback;

        private Tweener m_twn;

        protected override void Awake()
        {
            base.OnDestroy();
            m_CurtainImage = GetComponent<Image>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_twn?.Kill();
        }

        public void StartFade(float duration, Ease easeType = Ease.Linear, Action onComplete = null)
        {
            if (m_twn == null)
            {
                m_twn = m_CurtainImage.DOFade(1, duration).SetEase(easeType).SetLoops(2, LoopType.Yoyo).SetUpdate(true).SetAutoKill(false);
            }
            else
            {
                m_twn.Restart();
            }
            m_CompleteCallback = onComplete;
            m_twn.onComplete = OnCurtainEnd;
        }

        public static void Display(float duration, Ease easeType = Ease.Linear, Action onComplete = null)
        {
            var ui = UIManager.Instance.OpenUI(UINameConfig.BlackCurtain) as BlackCurtain;
            if(ui != null)
            {
                ui.StartFade(duration, easeType, onComplete);
            }
        }

        void OnCurtainEnd()
        {
            UIManager.Instance.CloseUI(UINameConfig.BlackCurtain);
            m_CompleteCallback?.Invoke();
            m_CompleteCallback = null;
        }
    }
}
