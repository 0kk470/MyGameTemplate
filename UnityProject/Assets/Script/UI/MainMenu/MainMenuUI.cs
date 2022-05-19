using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Saltyfish.UI
{
    public class MainMenuUI:UIBase
    {
        [SerializeField]
        private Button m_StartBtn;

        [SerializeField]
        private Button m_QuitBtn;

        [SerializeField]
        private Button m_SettingBtn;


        protected override void Awake()
        {
            base.Awake();
            m_StartBtn.onClick.AddListener(OnStartBtnClick);
            m_QuitBtn.onClick.AddListener(OnQuitBtnClick);
            m_SettingBtn.onClick.AddListener(OnSettingBtnClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            Audio.AudioManager.Instance.CrossFadeBgm(AudioID.MainMenu, true, -1, 2);
        }

        protected override void OnHide()
        {
            base.OnHide();
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_StartBtn.onClick.RemoveListener(OnStartBtnClick);
            m_QuitBtn.onClick.RemoveListener(OnQuitBtnClick);
            m_SettingBtn.onClick.RemoveListener(OnSettingBtnClick);
        }


        private void OnStartBtnClick()
        {
            if (GameManager.IsRestartingGame)
                return;
            GameManager.StartGame();
        }


        private void OnQuitBtnClick()
        {
            if (GameManager.IsRestartingGame)
                return;
            GameManager.QuitGame();
        }

        private void OnSettingBtnClick()
        {
            UIManager.Instance.OpenUI(UINameConfig.GameSettingUI);
        }
    }
}
