using Saltyfish.Data;
using Saltyfish.Resource;
using Saltyfish.Util;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Saltyfish.UI
{
    public class GameOverUI : UIBase
    {
        [SerializeField]
        private Text m_Title;

        [SerializeField]
        private Button m_RestartBtn;

        [SerializeField]
        private Button m_BackMenuBtn;


        protected override void Awake()
        {
            base.Awake();
            m_RestartBtn.onClick.AddListener(OnRestartBtnClick);
            m_BackMenuBtn.onClick.AddListener(OnBackMenuBtnClick);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_RestartBtn.onClick.RemoveListener(OnRestartBtnClick);
            m_BackMenuBtn.onClick.RemoveListener(OnBackMenuBtnClick);
        }

        protected override void OnShow()
        {
            GameManager.Flag.AddFlag(GameFlag.IsInGameOver);
            base.OnShow();
        }

        protected override void OnHide()
        {
            GameManager.Flag.RemoveFlag(GameFlag.IsInGameOver);
            base.OnHide();
        }

        public override void Refresh()
        {
            base.Refresh();
        }


        private void OnRestartBtnClick()
        {
            m_RestartBtn.enabled = false;
            m_BackMenuBtn.enabled = false;
            GameManager.RestartGame();
        }

        private void OnBackMenuBtnClick()
        {
            m_RestartBtn.enabled = false;
            m_BackMenuBtn.enabled = false;
            GameManager.BackToMainMenu();
        }
    }
}
