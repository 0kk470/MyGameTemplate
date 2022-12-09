using Saltyfish.Audio;
using System;
using UnityEngine;

namespace Saltyfish.Audio
{

    public class AudioShot : MonoBehaviour
    {
        [SerializeField]
        private bool m_PlayOnEnable = false;

        [SerializeField]
        private string m_AudioPath;

        [SerializeField]
        private bool m_Is3D = false;

        private void OnEnable()
        {
            if (m_PlayOnEnable)
                PlayOneShot();
        }
        public void PlayOneShot()
        {
            AudioManager.Instance?.PlayOneShot(m_AudioPath, transform.position, m_Is3D);
        }
    }
}
