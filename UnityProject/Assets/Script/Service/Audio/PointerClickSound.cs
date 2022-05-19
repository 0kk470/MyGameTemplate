using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Saltyfish.Audio
{
    [DisallowMultipleComponent]
    public class PointerClickSound : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private string m_SoundPath = "Audio/Sound/click";

        public void OnPointerClick(PointerEventData eventData)
        {
            Saltyfish.Audio.AudioManager.Instance?.PlayOneShot(m_SoundPath);
        }
    }
}
