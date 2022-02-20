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
        private Button m_CreditBtn;

        [SerializeField]
        private Button m_QuitBtn;

        [SerializeField]
        private Button m_SettingBtn;

        protected override void Awake()
        {
            base.Awake();
            m_StartBtn.onClick.AddListener(OnStartBtnClick);
            m_CreditBtn.onClick.AddListener(OnCreditBtnClick);
            m_QuitBtn.onClick.AddListener(OnQuitBtnClick);
            m_SettingBtn.onClick.AddListener(OnSettingBtnClick);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_StartBtn.onClick.RemoveListener(OnStartBtnClick);
            m_CreditBtn.onClick.RemoveListener(OnCreditBtnClick);
            m_QuitBtn.onClick.RemoveListener(OnQuitBtnClick);
            m_SettingBtn.onClick.RemoveListener(OnSettingBtnClick);
        }

        private void OnStartBtnClick()
        {
            if (GameManager.IsRestartingGame)
                return;
            Audio.AudioManager.Instance.PlaySFXOneShot("Audio/Sound/click");
            GameManager.StartGame();
        }

        private void OnCreditBtnClick()
        {
            if (GameManager.IsRestartingGame)
                return;
            Audio.AudioManager.Instance.PlaySFXOneShot("Audio/Sound/click");
            MessageBox.ShowMessage("TODO");
        }

        private void OnQuitBtnClick()
        {
            if (GameManager.IsRestartingGame)
                return;
            Audio.AudioManager.Instance.PlaySFXOneShot("Audio/Sound/click");
            MessageBox.ShowMessage("确定要退出游戏吗?",
                () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
        }

        private void OnSettingBtnClick()
        {
            UIManager.Instance.OpenUI(UINameConfig.GameSettingUI);
        }
    }
}
