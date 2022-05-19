using Saltyfish.UI;
using Saltyfish.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saltyfish
{
    public static class GameManager
    {
        public static event Action<bool> OnGamePause;

        public static event Action OnGameOver;

        public static GameFlagObject Flag { get; } = new GameFlagObject();

        private static List<Resolution> m_Resolutions;

        public static List<Resolution> SupportResolutions
        {
            get
            {
                if(m_Resolutions == null)
                {
                    m_Resolutions = new List<Resolution>();
                    m_Resolutions.AddRange(Screen.resolutions);
                }
                return m_Resolutions;
            }
        }

        public static bool IsBackingMainMenu => Flag.HasFlag(GameFlag.IsBackingMainMenu);

        public static bool IsRestartingGame => Flag.HasFlag(GameFlag.IsRestartingGame);

        public static bool IsInGameOver => Flag.HasFlag(GameFlag.IsInGameOver);

        public static bool IsLeavingRestaurant => Flag.HasFlag(GameFlag.IsInGameOver);

        public static bool IsEnteringRestaurant => Flag.HasFlag(GameFlag.IsInGameOver);


        public static bool IsBusy => IsBackingMainMenu || IsRestartingGame;

        public static bool IsPaused => Flag.HasFlag(GameFlag.Paused);

        public static void Init()
        {
            InitPlayerSettings();
        }

        public static void DeInit()
        {
        }

        public static void PauseGame()
        {
            Flag.AddFlag(GameFlag.Paused);
            OnGamePause?.Invoke(true);
        }


        public static void ResumeGame()
        {
            Flag.RemoveFlag(GameFlag.Paused);
            OnGamePause?.Invoke(false);
        }


        public static void StartGame()
        {
            if (IsRestartingGame)
                return;
            Util.CoroutineRunner.Instance.StartCoroutine(StartGame_Process());
        }

        public static void RestartGame()
        {
            if (IsRestartingGame || IsBackingMainMenu)
                return;
            Util.CoroutineRunner.Instance.StartCoroutine(GameRestart_Process());
        }

        public static void GameOver()
        {
            if (IsInGameOver)
                return;
            OnGameOver?.Invoke();
            Util.CoroutineRunner.Instance.StartCoroutine(GameOver_Process(false));
        }

        public static void QuitGame()
        {
            MessageBox.ShowConfirmMessage("退出到桌面?",
                () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                });
        }

        public static void BackToMainMenu()
        {
            if (IsBackingMainMenu)
                return;
            MessageBox.ShowConfirmMessage_i18n("ui/pause/menu_confirm", () => { CoroutineRunner.Instance.StartCoroutine(BackMenu_Process()); });
        }

        private static IEnumerator BackMenu_Process()
        {
            Flag.AddFlag(GameFlag.IsBackingMainMenu);
            BlackCurtain.Display(1.8f, DG.Tweening.Ease.Linear);
            yield return new WaitForSeconds(1.7f);

            //Dispose level here

            UIManager.Instance.CloseAllUI_Except(UINameConfig.BlackCurtain);
            UIManager.Instance.OpenUI(UINameConfig.MainMenuUI);
            Flag.RemoveFlag(GameFlag.IsBackingMainMenu);
        }

        private static IEnumerator GameRestart_Process()
        {
            Flag.AddFlag(GameFlag.IsRestartingGame);
            BlackCurtain.Display(2, DG.Tweening.Ease.OutQuart);
            yield return new WaitForSeconds(1.5f);

            //Dispose level here

            UIManager.Instance.CloseAllUI_Except(UINameConfig.BlackCurtain);
            Audio.AudioManager.Instance.CrossFadeBgm(AudioID.Street);

            //restart level here

            Flag.RemoveFlag(GameFlag.IsRestartingGame);
        }

        private static IEnumerator StartGame_Process()
        {
            Flag.AddFlag(GameFlag.IsRestartingGame);

            BlackCurtain.Display(2, DG.Tweening.Ease.OutQuart);
            yield return new WaitForSeconds(1.5f);
            UIManager.Instance.CloseAllUI_Except(UINameConfig.BlackCurtain);
            Audio.AudioManager.Instance.CrossFadeStopBgm();

            //start level here

            Audio.AudioManager.Instance.CrossFadeBgm(AudioID.Street);
            Flag.RemoveFlag(GameFlag.IsRestartingGame);
        }

        private static IEnumerator GameOver_Process(bool isWin)
        {
            Flag.AddFlag(GameFlag.IsInGameOver);
            yield return new WaitForSeconds(1);
            Audio.AudioManager.Instance.CrossFadeStopBgm();
            UIManager.Instance.OpenUI(UINameConfig.GameOverUI);
        }

        private static void InitPlayerSettings()
        {
            m_Resolutions = null;
            EasyPlayerPrefs.Init();

            Audio.AudioManager.Instance.IsMute = EasyPlayerPrefs.GetBool("IsMute");
            Audio.AudioManager.Instance.GlobalVolume = EasyPlayerPrefs.GetFloat("MasterVolume", 1);
            Audio.AudioManager.Instance.BGMVolume = EasyPlayerPrefs.GetFloat("MusicVolume", 1);
            Audio.AudioManager.Instance.SFXVolume = EasyPlayerPrefs.GetFloat("SfxVolume", 1);

            bool isFullScreen = EasyPlayerPrefs.GetBool("IsFullScreen");
            var resolutionX = EasyPlayerPrefs.GetInt("ResolutionX", 1280);
            var resolutionY = EasyPlayerPrefs.GetInt("ResolutionY", 720);
            var refreshRate = EasyPlayerPrefs.GetInt("RefreshRate", 60);
            var resolutions = SupportResolutions;
            var resolutionIdx = resolutions.FindIndex(r => r.width == resolutionX && r.height == resolutionY && r.refreshRate == refreshRate);
            if (resolutionIdx == -1)
            {
                resolutionIdx = 0;
            }
            var targetResolution = resolutions[resolutionIdx];
            EasyPlayerPrefs.SetInt("ResolutionX", targetResolution.width);
            EasyPlayerPrefs.SetInt("ResolutionY", targetResolution.height);
            EasyPlayerPrefs.SetInt("RefreshRate", targetResolution.refreshRate);
            Screen.SetResolution(targetResolution.width, targetResolution.height, isFullScreen, targetResolution.refreshRate);
        }

        public static void Update(float deltaTime)
        {
            if (IsPaused)
                return;
            TimerManager.Instance.Update(deltaTime);
        }
    }
}
