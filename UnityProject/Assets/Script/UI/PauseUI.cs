using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Saltyfish.UI
{
    public class PauseUI : UIBase
    {
        [SerializeField]
        private Button m_ResumeBtn;

        [SerializeField]
        private Button m_BackMenuBtn;

        [SerializeField]
        private Button m_SettingBtn;

        [SerializeField]
        private Button m_QuitBtn;

        protected override void Awake()
        {
            base.Awake();
            m_ResumeBtn.onClick.AddListener(OnResumeBtnClick);
            m_BackMenuBtn.onClick.AddListener(OnBackMenuBtnClick);
            m_QuitBtn.onClick.AddListener(OnQuitBtnClick);
            m_SettingBtn.onClick.AddListener(OnSettingBtnClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_ResumeBtn.onClick.RemoveListener(OnResumeBtnClick);
            m_BackMenuBtn.onClick.RemoveListener(OnBackMenuBtnClick);
            m_QuitBtn.onClick.RemoveListener(OnQuitBtnClick);
            m_SettingBtn.onClick.RemoveListener(OnSettingBtnClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            GameManager.PauseGame();
        }

        protected override void OnHide()
        {
            base.OnHide();
            GameManager.ResumeGame();
        }


        private void OnResumeBtnClick()
        {
            UIManager.Instance.CloseUI(UINameConfig.PauseUI);
        }

        private void OnBackMenuBtnClick()
        {
            GameManager.BackToMainMenu();
        }


        private void OnQuitBtnClick()
        {
            GameManager.QuitGame();
        }

        private void OnSettingBtnClick()
        {
            UIManager.Instance.OpenUI(UINameConfig.GameSettingUI);
        }
    }
}
