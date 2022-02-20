using Saltyfish.Audio;
using Saltyfish.Event;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Saltyfish.UI
{
    public class GameSettingUI : UIBase
    {
        [SerializeField]
        private Dropdown m_ResolutionDrop;

        [SerializeField]
        private Toggle m_MasterVolTgl;

        [SerializeField]
        private Toggle m_WindowdTgl;

        [SerializeField]
        private Slider m_MusicVolumeSlider;

        [SerializeField]
        private Slider m_SfxVolumeSlider;

        [SerializeField]
        private Slider m_MasterVolumeSlider;

        [SerializeField]
        private Button m_CloseBtn;

        private Resolution[] m_AllResoutions;

        protected override void Awake()
        {
            base.Awake();
            m_AllResoutions = Screen.resolutions;
            m_ResolutionDrop.onValueChanged.AddListener(OnResolutionChanged);
            m_MasterVolTgl.onValueChanged.AddListener(OnMasterValToggleChanged);
            m_MusicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            m_SfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            m_MasterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            m_WindowdTgl.onValueChanged.AddListener(OnWindowTglChanged);
            m_CloseBtn.onClick.AddListener(OnCloseBtnClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_ResolutionDrop.onValueChanged.RemoveListener(OnResolutionChanged);
            m_MasterVolTgl.onValueChanged.RemoveListener(OnMasterValToggleChanged);
            m_MusicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            m_SfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            m_MasterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            m_WindowdTgl.onValueChanged.RemoveListener(OnWindowTglChanged);
            m_CloseBtn.onClick.RemoveListener(OnCloseBtnClick);
            PlayerPrefs.Save();
        }

        public override void Refresh()
        {
            m_ResolutionDrop.AddOptions(m_AllResoutions.Select(r => new Dropdown.OptionData(r.ToString())).ToList());
            m_ResolutionDrop.SetValueWithoutNotify (PlayerPrefs.GetInt("Resolution"));
            m_WindowdTgl.SetIsOnWithoutNotify(PlayerPrefs.GetInt("IsFullScreen", 0) == 1);
            m_MasterVolTgl.SetIsOnWithoutNotify(!AudioManager.Instance.IsMute);
            m_SfxVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
            m_MusicVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.BGMVolume);
            m_MasterVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.GlobalVolume);
        }



        private void OnResolutionChanged(int val)
        {
            if (val < 0 || val >= m_AllResoutions.Length)
                return;
            var resolution = m_AllResoutions[val];
            PlayerPrefs.SetInt("Resolution", val);
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        }

        private void OnWindowTglChanged(bool isOn)
        {
            PlayerPrefs.SetInt("IsFullScreen", isOn ? 1 : 0);
            bool isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 0) == 1;
            var resolution = Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
        }

        private void OnMasterVolumeChanged(float v)
        {
            float volume = Mathf.Clamp01(v);
            PlayerPrefs.SetFloat("MasterVolume", volume);
            AudioManager.Instance.GlobalVolume = volume;
        }

        private void OnSfxVolumeChanged(float v)
        {
            float volume = Mathf.Clamp01(v);
            PlayerPrefs.SetFloat("SfxVolume", volume);
            AudioManager.Instance.SFXVolume = volume;
        }

        private void OnMusicVolumeChanged(float v)
        {
            float volume = Mathf.Clamp01(v);
            PlayerPrefs.SetFloat("MusicVolume", volume);
            AudioManager.Instance.BGMVolume = volume;
        }

        private void OnMasterValToggleChanged(bool isOn)
        {
            PlayerPrefs.SetInt("IsMute", isOn ? 0 : 1);
            AudioManager.Instance.IsMute = !isOn;
        }


        private void OnCloseBtnClick()
        {
            UIManager.Instance.CloseUI(UINameConfig.GameSettingUI);
        }
    }
}
