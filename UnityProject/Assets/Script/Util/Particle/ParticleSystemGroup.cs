using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Util.Particle
{
    public class ParticleSystemGroup : MonoBehaviour
    {
        private ParticleSystem[] m_Particles;

        public ParticleSystem[] Particles
        {
            get
            {
                if (m_Particles == null)
                {
                    m_Particles = GetComponentsInChildren<ParticleSystem>(true);
                }
                return m_Particles;
            }
        }

        public void SetSimulationSpeed(float speed)
        {
            if (Particles == null || Particles.Length == 0)
                return;
            for (int i = 0; i < Particles.Length; ++i)
            {
                var mainModule = Particles[i].main;
                mainModule.simulationSpeed = speed;
            }
        }

        public void SetColor(Color color)
        {
            if (Particles == null || Particles.Length == 0)
                return;
            for (int i = 0; i < Particles.Length; ++i)
            {
                var mainModule = Particles[i].main;
                mainModule.startColor = color;
            }
        }


        public void SetMaxParticleNum(int num)
        {
            if (Particles == null || Particles.Length == 0)
                return;
            for (int i = 0; i < Particles.Length; ++i)
            {
                var mainModule = Particles[i].main;
                mainModule.maxParticles = num;
            }
        }

        public void Stop()
        {
            if (Particles == null || Particles.Length == 0)
                return;
            for (int i = 0; i < Particles.Length; ++i)
            {
                var emission = Particles[i].emission;
                emission.enabled = false;
                Particles[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public void Play()
        {
            if (Particles == null || Particles.Length == 0)
                return;
            for (int i = 0; i < Particles.Length; ++i)
            {
                var emission = Particles[i].emission;
                emission.enabled = true;
                Particles[i].Play();
            }
        }

        internal void SetScaleTime(bool isRealTime)
        {
            if (Particles == null || Particles.Length == 0)
                return;
            for (int i = 0; i < Particles.Length; ++i)
            {
                var mainModule = Particles[i].main;
                mainModule.useUnscaledTime = isRealTime;
            }
        }
    }
}
