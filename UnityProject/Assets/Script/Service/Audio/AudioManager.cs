using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using Saltyfish.Resource;
using UnityEngine;
using UnityEngine.Events;

namespace Saltyfish.Audio
{
    public class AudioManager : ManagerBase<AudioManager>
    {
#if UNITY_EDITOR
        private static List<string> m_AudioConfigList;

        public static List<string> AudioConfigList
        {
            get
            {
                if(m_AudioConfigList == null)
                {
                    var filePath = Path.Combine(Application.streamingAssetsPath, "AudioConfig.json");
                    string json = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
                    var configDic = JsonUtility.FromJson<Dictionary<string, string>>(json);
                    if(configDic != null)
                    {
                        m_AudioConfigList = new List<string>(configDic.Values);
                    }
                    else
                    {
                        m_AudioConfigList = new List<string>();
                    }
                }
                return m_AudioConfigList;
            }
        }
#endif
        public static event Action<float> OnMasterVolumeChange;

        public static event Action<float> OnMusicVolumeChange; 

        public static event Action<float> OnSfxVolumeChange; 

        public static event Action<bool> OnMasterVolumeMute;  

        [SerializeField]
        private string m_AudioSourcePrefabPath = "Prefabs/Audio/SoundEffect";

        [SerializeField]
        private AudioSource bgmAudioSource;

        [SerializeField]
        // All activated SFX audio sources in scene
        private List<SoundEffect> m_Sfxs = new List<SoundEffect>();

        private ObjectPool.ObjPool<SoundEffect> m_SoundPool = new ObjectPool.ObjPool<SoundEffect>(100);

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
            for (int i = m_Sfxs.Count - 1; i >= 0; i--)
            {
                if (m_Sfxs[i].audioSource != null)
                {
                    SetSFX(m_Sfxs[i].audioSource);
                }
                else
                {
                    m_Sfxs.RemoveAt(i);
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
            UpdateBGMAudioSource();
            bgmAudioSource.Play();
        }

        public void StopBgm()
        {
            bgmAudioSource?.Stop();
        }

        public void CrossFadeStopBgm(float crossFadeDuration = 1, Ease ease = Ease.Linear)
        {
            TweenCallback onComplete = () => StopBgm();
            DOVirtual.Float(1, 0, crossFadeDuration,
                (vol) => bgmAudioSource.volume = bgmVolume * globalVolume * vol)
                .SetEase(ease).onComplete = onComplete;
        }


        public void PlayBGM(string path, bool loop = true, float volume = -1)
        {
            if (string.IsNullOrEmpty(path))
                return;
            var clip = AssetCache.LoadAsset<AudioClip>(path);
            if (clip != null)
            {
                PlayBGM(clip, loop, volume);
            }
        }

        public void CrossFadeBgm(string path="", bool loop = true, float volume = -1, float crossFadeDuration = 1f, Ease ease = Ease.Linear)
        {
            TweenCallback onComplete = () => PlayBGM(path, loop, volume);
            DOVirtual.Float(1, 0, crossFadeDuration,
                (vol) => bgmAudioSource.volume = bgmVolume * globalVolume * vol)
                .SetEase(ease).SetUpdate(true).onComplete = onComplete;
        }
        #endregion

        #region SFX control
        /// <summary>
        /// Get SFX audio source
        /// </summary>
        /// <param name="is3D">Is 3D audio or not</param>
        /// <returns></returns>
        private SoundEffect GetNewSoundEffect(bool is3D = true)
        {
            var sfx = m_SoundPool.Get();
            sfx.audioSource = ObjectPool.GoPoolMgr.CreateComponent<AudioSource>(m_AudioSourcePrefabPath);
            SetSFX(sfx.audioSource, is3D ? 1f : 0f);
            m_Sfxs.Add(sfx);
            return sfx;
        }
        /// <summary>
        /// Recycle SFX audio source
        /// </summary>
        private void RecycleSoundEffect(SoundEffect soundEffect, Action onComplete)
        {
            StartCoroutine(Co_RecycleSoundEffect(soundEffect, onComplete));
        }

        private IEnumerator Co_RecycleSoundEffect(SoundEffect soundEffect, Action onComplete)
        {
            yield return new WaitForSeconds(soundEffect.Clip.length);
            DoRecyleSfx(soundEffect);
            onComplete?.Invoke();
        }

        private void DoRecyleSfx(SoundEffect sfx)
        {
            if (sfx.audioSource != null)
            {
                ObjectPool.GoPoolMgr.RecycleComponent(sfx.audioSource, m_AudioSourcePrefabPath);
            }
            sfx.audioSource = null;
            sfx.Clip = null;
            sfx.PlayTime = 0;
            m_Sfxs.Remove(sfx);
            m_SoundPool.Release(sfx);
        }

        private void PlayOneShot(AudioClip clip, AudioParam param)
        {
            float NowTime = Time.time;
            int sameCount = m_Sfxs.Count(sfx =>
            {
                return sfx.Clip == clip && (NowTime - sfx.PlayTime) <= param.timeToCountMax;
            });
            if (sameCount >= param.MaxPlayNum)
                return;
            // Initialize audio source
            var soundEffect = GetNewSoundEffect(param.is3D);
            soundEffect.Clip = clip;
            if (soundEffect.audioSource != null)
            {
                soundEffect.audioSource.transform.localPosition = param.Position ;

                // Call actual PlayOneShot function on SFX audio source
                soundEffect.audioSource.PlayOneShot(clip, param.VolumeScale);

            }
            soundEffect.PlayTime = NowTime;
            RecycleSoundEffect(soundEffect, param.OnPlayComplete);
        }

        public void PlayOneShot(string path, Vector3 position = default, bool is3d = false, Action onComplete = null, int max_num = 2, float timeToCount = 0.1f)
        {
            if (string.IsNullOrEmpty(path))
                return;
            var clip = AssetCache.LoadAsset<AudioClip>(path);
            if(clip != null)
            {
                AudioParam param = new AudioParam();
                param.VolumeScale = 1;
                param.AudioClipPath = path;
                param.Position = position;
                param.MaxPlayNum = max_num;
                param.timeToCountMax = timeToCount;
                param.is3D = is3d;
                param.OnPlayComplete = onComplete;
                PlayOneShot(clip, param);
            }
        }

        public void PlayOneShot(AudioParam param)
        {
            if (string.IsNullOrEmpty(param.AudioClipPath))
                return;
            var clip = AssetCache.LoadAsset<AudioClip>(param.AudioClipPath);
            if (clip != null)
            {
                PlayOneShot(clip, param);
            }
        }


        #endregion
    }
}