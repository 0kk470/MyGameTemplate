using Saltyfish.Audio;
using System;
using UnityEngine;

namespace Saltyfish.Audio
{

    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceControl:MonoBehaviour
    {
        [SerializeField]
        private bool m_PlayOnEnable = false;

        [SerializeField]
        private AudioSource m_AudioSource;

        public bool IsPlaying => m_AudioSource?.isPlaying ?? false;


        private void Awake()
        {
            if(m_AudioSource == null)
                m_AudioSource = GetComponent<AudioSource>();

            m_AudioSource.volume = AudioManager.Instance.SFXVolume;
            m_AudioSource.mute = AudioManager.Instance.IsMute;

            AudioManager.OnMasterVolumeMute += OnMasterVolumeMute;
            AudioManager.OnSfxVolumeChange += OnSoundFxVolumeChange;
        }

        private void OnEnable()
        {
            if (m_PlayOnEnable)
                Play();
        }

        private void OnDisable()
        {
            
        }


        private void OnDestroy()
        {
            AudioManager.OnMasterVolumeMute -= OnMasterVolumeMute;
            AudioManager.OnSfxVolumeChange -= OnSoundFxVolumeChange;
        }

        private void OnMasterVolumeMute(bool isMute)
        {
            m_AudioSource.mute = isMute;
        }

        private void OnSoundFxVolumeChange(float volume)
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.volume = volume;
            }
        }

        public void SetLoop(bool isLoop)
        {
            m_AudioSource.loop = isLoop;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            m_AudioSource?.Play();
        }
    }
}
