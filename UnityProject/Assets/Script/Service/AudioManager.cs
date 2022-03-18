using System;
using System.Collections;
using System.Collections.Generic;
using Saltyfish.Resource;
using UnityEngine;
using UnityEngine.Events;

namespace Saltyfish.Audio
{
    public class AudioManager : ManagerBase<AudioManager>
    {
        public static event Action<float> OnMasterVolumeChange;

        public static event Action<float> OnMusicVolumeChange; 

        public static event Action<float> OnSfxVolumeChange; 

        public static event Action<bool> OnMasterVolumeMute;  

        private const string AudioCacheName = "AudioCache";

        [SerializeField]
        private string m_AudioSourcePrefabPath = "Prefabs/Audio/SoundEffect";

        [SerializeField]
        private AudioSource bgmAudioSource;

        // All activated SFX audio sources in scene
        private List<AudioSource> sfxAudioSources = new List<AudioSource>();

        #region Volume and audio source control
        [SerializeField]
        [Range(0, 1)]
        //[OnValueChanged("UpdateAllAudioSources")]
        private float globalVolume;

        [SerializeField]
        [Range(0, 1)]
        private float bgmVolume;

        [SerializeField]
        [Range(0, 1)]
        private float sfxVolume;

        [SerializeField]
        private bool isMute = false;

        [SerializeField]
        private bool isLoop = true;

        private bool isPause = false;

        public float GlobalVolume
        {
            get => globalVolume;
            set
            {
                if (globalVolume == value) return;
                globalVolume = value;
                OnMasterVolumeChange?.Invoke(globalVolume);
                UpdateAllAudioSources();
            }
        }

        public float BGMVolume
        {
            get => bgmVolume;
            set
            {
                if (bgmVolume == value) return;
                bgmVolume = value;
                OnMusicVolumeChange?.Invoke(bgmVolume);
                UpdateBGMAudioSource();
            }
        }

        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                if (sfxVolume == value) return;
                sfxVolume = value;
                OnSfxVolumeChange?.Invoke(sfxVolume);
                UpdateSFXAudioSources();
            }
        }

        public bool IsMute
        {
            get => isMute;
            set
            {
                if (isMute == value) return;
                isMute = value;
                OnMasterVolumeMute?.Invoke(isMute);
                UpdateMute();
            }
        }

        public bool IsLoop
        {
            get => isLoop;
            set
            {
                if (isLoop == value) return;
                isLoop = value;
                UpdateLoop();
            }
        }

        public bool IsPause
        {
            get => isPause;
            set
            {
                if (isPause == value) return;
                isPause = value;
                if (isPause)
                {
                    bgmAudioSource.Pause();
                }
                else
                {
                    bgmAudioSource.UnPause();
                }
                UpdateSFXAudioSources();
            }
        }


#if UNITY_EDITOR
        void OnValidate()
        {
            UpdateAllAudioSources();
            UpdateMute();
            UpdateLoop();
        }
#endif
        private void UpdateAllAudioSources()
        {
            UpdateBGMAudioSource();
            UpdateSFXAudioSources();
        }

        private void UpdateBGMAudioSource()
        {
            if(bgmAudioSource != null)
                bgmAudioSource.volume = bgmVolume * globalVolume;
        }

        private void UpdateSFXAudioSources()
        {
            for (int i = sfxAudioSources.Count - 1; i >= 0; i--)
            {
                if (sfxAudioSources[i] != null)
                {
                    SetSFX(sfxAudioSources[i]);
                }
                else
                {
                    sfxAudioSources.RemoveAt(i);
                }
            }
        }

        private void SetSFX(AudioSource audioSource, float spatial = -1)
        {
            if(audioSource == null)
            {
               Debug.LogError("SetSfx failed, audioSource is null");
               return;
            }
            audioSource.mute = isMute;
            audioSource.volume = sfxVolume * globalVolume;
            if (spatial != -1)
            {
                audioSource.spatialBlend = spatial;
            }
            if (isPause)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }

        private void UpdateMute()
        {
            if(bgmAudioSource != null)
               bgmAudioSource.mute = isMute;
            UpdateSFXAudioSources();
        }

        private void UpdateLoop()
        {
            if(bgmAudioSource != null)
               bgmAudioSource.loop = isLoop;
        }

        #endregion

        #region BGM control
        public void PlayBGM(AudioClip clip, bool loop = true, float volume = -1)
        {
            bgmAudioSource.clip = clip;
            IsLoop = loop;
            if (volume != -1)
            {
                BGMVolume = volume;
            }
            bgmAudioSource.Play();
        }
        public void PlayBGM(string path, bool loop = true, float volume = -1)
        {
            var clip = AssetCache.GetCache(AudioCacheName).GetAsset<AudioClip>(path);
            if (clip != null)
            {
                PlayBGM(clip, loop, volume);
            }
        }
        #endregion

        #region SFX control
        /// <summary>
        /// Get SFX audio source
        /// </summary>
        /// <param name="is3D">Is 3D audio or not</param>
        /// <returns></returns>
        private AudioSource GetSFXAudioSource(bool is3D = true)
        {
            AudioSource audioSource = ObjectPool.GoPoolMgr.CreateComponent<AudioSource>(m_AudioSourcePrefabPath);
            SetSFX(audioSource, is3D ? 1f : 0f);
            sfxAudioSources.Add(audioSource);
            return audioSource;
        }
        /// <summary>
        /// Recycle SFX audio source
        /// </summary>
        private void RecycleSFXAudioSource(AudioSource audioSource, AudioClip clip, UnityAction callback, float callbackWaitSec)
        {
            StartCoroutine(DoRecycleSFXAudioSource(audioSource, clip, callback, callbackWaitSec));
        }

        private IEnumerator DoRecycleSFXAudioSource(AudioSource audioSource, AudioClip clip, UnityAction callback, float callbackWaitSec)
        {
            yield return new WaitForSeconds(clip.length);
            if (audioSource != null)
            {
                ObjectPool.GoPoolMgr.RecycleComponent(audioSource, m_AudioSourcePrefabPath);
                yield return new WaitForSeconds(callbackWaitSec);
                callback?.Invoke();
            }
        }

        public void PlayOneShot(AudioClip clip, Vector3 position = default, Transform parent = null, float volume = 1, bool is3D = false, UnityAction callback = null, float callbackWaitSec = 0)
        {
            // Initialize audio source
            AudioSource audioSource = GetSFXAudioSource(is3D);
            if(audioSource != null)
            {
                audioSource.transform.SetParent(parent);
                audioSource.transform.localPosition = position;

                // Call actual PlayOneShot function on SFX audio source
                audioSource.PlayOneShot(clip, volume);

                // Recycle audio source and callback
                RecycleSFXAudioSource(audioSource, clip, callback, callbackWaitSec);
            }
        }

        public void PlayOneShot(string path, Vector3 position = default, Transform parent = null, float volume = 1, bool is3D = false, UnityAction callback = null, float callbackWaitSec = 0)
        {
            var clip = AssetCache.GetCache(AudioCacheName).GetAsset<AudioClip>(path);
            if(clip != null)
            {
                PlayOneShot(clip, position, parent, volume, is3D, callback, callbackWaitSec);
            }
        }
        #endregion
    }
}