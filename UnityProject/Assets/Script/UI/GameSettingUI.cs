using Saltyfish.Audio;
using Saltyfish.Event;
using Saltyfish.Util;
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

        protected override void Awake()
        {
            base.Awake();
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
            EasyPlayerPrefs.Save();
        }

        public override void Refresh()
        {
            RefreshResolutionSelections();
            m_WindowdTgl.SetIsOnWithoutNotify(EasyPlayerPrefs.GetBool("IsFullScreen", false));
            m_MasterVolTgl.SetIsOnWithoutNotify(!AudioManager.Instance.IsMute);
            m_SfxVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
            m_MusicVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.BGMVolume);
            m_MasterVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.GlobalVolume);
        }

        private void RefreshResolutionSelections()
        {
            var resolutions = GameManager.SupportResolutions;
            m_ResolutionDrop.AddOptions(resolutions.Select(r => new Dropdown.OptionData(r.ToString())).ToList());
            var width = EasyPlayerPrefs.GetInt("ResolutionX");
            var height = EasyPlayerPrefs.GetInt("ResolutionY");
            var refreshRate = EasyPlayerPrefs.GetInt("RefreshRate");
            var idx = resolutions.FindIndex(r => r.width == width && r.height == height && r.refreshRate == refreshRate);
            if(idx != -1)
                m_ResolutionDrop.SetValueWithoutNotify(idx);
        }



        private void OnResolutionChanged(int val)
        {
            var allResolutions = GameManager.SupportResolutions; 
            if (val < 0 || val >= allResolutions.Count)
                return;
            var resolution = allResolutions[val];
            EasyPlayerPrefs.SetInt("ResolutionX", resolution.width);
            EasyPlayerPrefs.SetInt("ResolutionY", resolution.height);
            EasyPlayerPrefs.SetInt("RefreshRate", resolution.refreshRate);
            var isFullScreen = EasyPlayerPrefs.GetBool("IsFullScreen");
            Screen.SetResolution(resolution.width, resolution.height, isFullScreen, resolution.refreshRate);
        }

        private void OnWindowTglChanged(bool isFullScreen)
        {
            EasyPlayerPrefs.SetBool("IsFullScreen", isFullScreen);
            var resolution = Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
        }

        private void OnMasterVolumeChanged(float v)
        {
            float volume = Mathf.Clamp01(v);
            EasyPlayerPrefs.SetFloat("MasterVolume", volume);
            AudioManager.Instance.GlobalVolume = volume;
        }

        private void OnSfxVolumeChanged(float v)
        {
            float volume = Mathf.Clamp01(v);
            EasyPlayerPrefs.SetFloat("SfxVolume", volume);
            AudioManager.Instance.SFXVolume = volume;
        }

        private void OnMusicVolumeChanged(float v)
        {
            float volume = Mathf.Clamp01(v);
            EasyPlayerPrefs.SetFloat("MusicVolume", volume);
            AudioManager.Instance.BGMVolume = volume;
        }

        private void OnMasterValToggleChanged(bool isOn)
        {
            EasyPlayerPrefs.SetBool("IsMute", !isOn);
            AudioManager.Instance.IsMute = !isOn;
        }


        private void OnCloseBtnClick()
        {
            UIManager.Instance.CloseUI(UINameConfig.GameSettingUI);
        }
    }
}
