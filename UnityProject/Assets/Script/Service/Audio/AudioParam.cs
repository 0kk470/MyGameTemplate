using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Audio
{
    public struct AudioParam
    {
        public string AudioClipPath;

        public float VolumeScale;

        public Vector3 Position;

        public float timeToCountMax;

        public int MaxPlayNum;

        public Action OnPlayComplete;

        public bool is3D;
    }
}
