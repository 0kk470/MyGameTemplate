using Saltyfish.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Saltyfish
{
    public static class GameManager
    {
        public static GameFlagObject Flag { get; } = new GameFlagObject();

        public static bool IsBackingMainMenu => Flag.HasFlag(GameFlag.IsBackingMainMenu);

        public static bool IsRestartingGame => Flag.HasFlag(GameFlag.IsRestartingGame);

        public static bool IsInGameOver => Flag.HasFlag(GameFlag.IsInGameOver);


        public static bool IsBusy
        {
            get
            {
                return  IsBackingMainMenu || IsRestartingGame;
            }
        }

        public static void Init()
        {
            InitPlayerSettings();
        }

        public static void DeInit()
        {

        }

        public static void StartGame()
        {
            if (IsRestartingGame)
                return;
            MessageBox.ShowMessage("TODO");
            //Util.CoroutineRunner.Instance.StartCoroutine(StartGame_Process());
        }

        public static void RestartGame()
        {
            if (IsRestartingGame || IsBackingMainMenu)
                return;
            MessageBox.ShowMessage("TODO");
            //Util.CoroutineRunner.Instance.StartCoroutine(GameRestart_Process());
        }

        public static void GameOver()
        {
            if (IsInGameOver)
                return;
            MessageBox.ShowMessage("TODO");
            //Util.CoroutineRunner.Instance.StartCoroutine(GameOver_Process(false));
        }

        public static void GameWin()
        {
            if (IsInGameOver)
                return;
            Util.CoroutineRunner.Instance.StartCoroutine(GameOver_Process(true));
        }


        public static void BackToMainMenu()
        {
            if (IsBackingMainMenu)
                return;
            Util.CoroutineRunner.Instance.StartCoroutine(BackMenu_Process());
        }

        private static IEnumerator BackMenu_Process()
        {
            Flag.AddFlag(GameFlag.IsBackingMainMenu);
            BlackCurtain.Display(1.8f, DG.Tweening.Ease.Linear);
            yield return new WaitForSeconds(1.7f);
            UIManager.Instance.CloseAllUI_Except(UINameConfig.BlackCurtain);
            UIManager.Instance.OpenUI(UINameConfig.MainMenuUI);
            Flag.RemoveFlag(GameFlag.IsBackingMainMenu);
        }

        private static IEnumerator GameRestart_Process()
        {
            Flag.AddFlag(GameFlag.IsRestartingGame);
            BlackCurtain.Display(2, DG.Tweening.Ease.OutQuart);
            yield return new WaitForSeconds(1.5f);
            UIManager.Instance.CloseAllUI_Except(UINameConfig.BlackCurtain);
            Flag.RemoveFlag(GameFlag.IsRestartingGame);
        }

        private static IEnumerator StartGame_Process()
        {
            Flag.AddFlag(GameFlag.IsRestartingGame);

            BlackCurtain.Display(2, DG.Tweening.Ease.OutQuart);
            yield return new WaitForSeconds(1.5f);
            UIManager.Instance.CloseAllUI_Except(UINameConfig.BlackCurtain);

            Flag.RemoveFlag(GameFlag.IsRestartingGame);
        }

        private static IEnumerator GameOver_Process(bool isWin)
        {
            Flag.AddFlag(GameFlag.IsInGameOver);
            yield return new WaitForSeconds(2f);
            var ui = UIManager.Instance.OpenUI(UINameConfig.GameOverUI) as GameOverUI;
            if (ui != null)
            {
                ui.SetTitle(isWin);
            }
        }

        private static void InitPlayerSettings()
        {
            Audio.AudioManager.Instance.IsMute = PlayerPrefs.GetInt("IsMute", 0) == 1;
            Audio.AudioManager.Instance.GlobalVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
            Audio.AudioManager.Instance.BGMVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
            Audio.AudioManager.Instance.SFXVolume = PlayerPrefs.GetFloat("SfxVolume", 1);

            bool isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 0) == 1;
            var resolutionIdx = PlayerPrefs.GetInt("Resolution", -1);
            var resolutions = Screen.resolutions;
            if (resolutionIdx < 0 || resolutionIdx >= resolutions.Length)
            {
                resolutionIdx = Array.FindIndex(resolutions, r => { return r.width == 1920 && r.height == 1080; }); ;
                if(resolutionIdx == -1)
                {
                    resolutionIdx = 0;
                }
            }
            PlayerPrefs.SetInt("Resolution", resolutionIdx);
            var targetResolution = resolutions[resolutionIdx];
            Screen.SetResolution(targetResolution.width, targetResolution.height, isFullScreen, targetResolution.refreshRate);
        }
    }
}
