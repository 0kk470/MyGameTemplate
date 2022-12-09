using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Audio
{
    [Serializable]
    public class SoundEffect
    {

        public AudioSource audioSource;

        public AudioClip Clip;
        public float PlayTime;
    }
}
