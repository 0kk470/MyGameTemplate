using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Saltyfish.Util;
using Saltyfish.Data;

public enum CameraPriority
{
    Highest = 100,
    Lowest = 0,
}

namespace Saltyfish.Logic
{
    public class CameraNameConfig
    {
        public const string BattleCameraName = "BattleCamera";

        public const string BalttlePrepareCameraName = "BattlePrepareCamera";
    }

    public class CameraManager : Singleton<CameraManager>
    {

        private CinemachineBrain m_brain;

        private CinemachineImpulseSource m_ShakeImpulseSource;

        private Camera m_MainCam;

        public Camera MainCamera
        {
            get
            {
                if (m_MainCam == null)
                    m_MainCam = Camera.main;
                return m_MainCam;
            }
        }


        public CinemachineBrain brain
        {
            get
            {
                if (m_brain == null)
                {
                    m_brain = Camera.main.GetComponent<CinemachineBrain>();
                }
                if (m_brain == null)
                    Debug.LogError("Error,Scene has no Main Camera");
                else
                {
                    m_brain.m_IgnoreTimeScale = true;
                    m_brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
                    CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
                }
                return m_brain;
            }
        }

        public CinemachineImpulseSource ShakeImpulseSource
        {
            get
            {
                if (m_ShakeImpulseSource == null)
                {
                    m_ShakeImpulseSource = Camera.main.GetComponent<CinemachineImpulseSource>();
                }
                if (m_ShakeImpulseSource == null)
                    Debug.LogError("Error,MainCamera has no CinemachineImpulseSource");
                return m_ShakeImpulseSource;
            }
        }

        private Dictionary<ICinemachineCamera, Tweener> m_CameraTwns = new Dictionary<ICinemachineCamera, Tweener>();

        public override void DeInit()
        {
            ClearTwns();
        }


        private void ClearTwns()
        {
            foreach(var twn in m_CameraTwns.Values)
            {
                if (twn != null)
                    twn.Kill();
            }
            m_CameraTwns.Clear();
        }


        public void SetFollowTarget(Transform target, string cameraName = "")
        {
            if (brain == null)
                return;
            var machineCamera = GetCamera(cameraName);
            if (machineCamera != null)
            {
                machineCamera.Follow = target;
            }
        }

        public void SetPlanePositon(Vector2 position, string cameraName = "")
        {
            if (brain == null)
                return;
            var machineCamera = GetCamera(cameraName);
            if (machineCamera != null)
            { 
                var targetPosition = machineCamera.VirtualCameraGameObject.transform.position;
                targetPosition.x = position.x;
                targetPosition.y = position.y;
                machineCamera.VirtualCameraGameObject.transform.position = targetPosition;
            }
            MarkMainCameraDirty();
        }


        public void MarkMainCameraDirty()
        {
            var cam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (cam != null)
            {
                cam.PreviousStateIsValid = false;
            }
        }

        public void SetCameraPriority(ICinemachineCamera cinemachineCamera, CameraPriority priority)
        {
            if (cinemachineCamera == null)
                return;
            cinemachineCamera.Priority = priority.GetHashCode();
        }

        public void SetCameraPriority(string cameraName, CameraPriority priority)
        {
            var cam = GetCamera(cameraName);
            if(cam != null)
            {
                cam.Priority = priority.GetHashCode();
            }
        }

        public void SetLookAt(Transform target, string cameraName = "")
        {
            var machineCamera = GetCamera(cameraName);
            if (machineCamera != null)
            {
                machineCamera.LookAt = target;
            }
        }

        public void UnLookAt()
        {
            SetLookAt(null);
        }

        public void UnFollowTarget(string cameraName = "")
        {
            SetFollowTarget(null, cameraName);
        }


        public ICinemachineCamera GetCamera(string cameraName = "")
        {
            if (string.IsNullOrEmpty(cameraName))
                return brain.ActiveVirtualCamera;
            for (int i = 0; i < CinemachineCore.Instance.VirtualCameraCount; ++i)
            {
                var cam = CinemachineCore.Instance.GetVirtualCamera(i);
                if (cam.gameObject.name == cameraName)
                {
                    return cam;
                }
            }
            return null;
        }

        public void SetBoudingShape(PolygonCollider2D shape)
        {
            if (brain == null)
                return;
            for (int i = 0; i < CinemachineCore.Instance.VirtualCameraCount; ++i)
            {
                var cam = CinemachineCore.Instance.GetVirtualCamera(i);
                if (cam != null)
                {
                    var confiner = cam.GetComponent<CinemachineConfiner>();
                    if (confiner != null)
                    {
                        confiner.m_BoundingShape2D = shape;
                    }
                }
            }
        }

        public void CloseBoudingShape()
        {
            SetBoudingShape(null);
        }

        public void SetCameraIgnoreTimeScale(bool ignoreTimeScale)
        {
            if (brain != null)
                brain.m_IgnoreTimeScale = true;
            CinemachineImpulseManager.Instance.IgnoreTimeScale = ignoreTimeScale;
        }


        public void ShakeCurrentCamera(float amplitudeGain = 1f, float duration = 0.5f)
        {
            if (!EasyPlayerPrefs.GetBool("IsShakeCamera", true))
                return;
            if (ShakeImpulseSource != null)
            {
                ShakeImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitudeGain;
                ShakeImpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = duration;
                ShakeImpulseSource.GenerateImpulse();
            }
        }


        public void DoPosition(string cameraName , Vector3 position, float speed = 20, TweenCallback onComplete = null)
        {
            if (string.IsNullOrEmpty(cameraName) || cameraName == CameraNameConfig.BattleCameraName)
                return;
            var cam = GetCamera(cameraName);
            if(cam == null)
            {
                Debug.LogError("Cannot find camera:" + cameraName);
                return;
            }
            var cameraTransform = cam.VirtualCameraGameObject.transform;
            position.z = cameraTransform.position.z;
            if(cameraTransform.position == position)
            {
                onComplete?.Invoke();
                return;
            }
            if (!m_CameraTwns.TryGetValue(cam,out Tweener twn))
            {
                twn = cameraTransform.DOMove(position, speed).SetSpeedBased(true).SetAutoKill(false).SetEase(Ease.Linear);
                m_CameraTwns.Add(cam, twn);
            }
            else
            {
                twn.Pause().ChangeValues(cameraTransform.position, position, speed).Restart();
            }
            twn.onComplete = onComplete;
        }

        public void PauseTwnPosition(string cameraName)
        {
            var cam = GetCamera(cameraName);
            if (cam == null)
                return;
            if(m_CameraTwns.TryGetValue(cam, out Tweener twn))
            {
                twn.Pause();
            }
        }
    }

}