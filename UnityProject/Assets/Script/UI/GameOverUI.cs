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

        public void SetTitle(bool isWin)
        {
            if(isWin)
            {
                m_Title.text = "你赢了";
                Saltyfish.Audio.AudioManager.Instance.PlayBGM("Audio/Music/Win-Mud-girl");
            }
            else
            {
                m_Title.text = "你挂了";
                Saltyfish.Audio.AudioManager.Instance.PlayBGM("Audio/Music/GameOver-Enchanted-Forest");
            }
        }
    }
}
