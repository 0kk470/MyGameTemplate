using UnityEngine;
using Saltyfish.Util;
using System.Linq;
using System;

namespace Saltyfish
{
    /// <summary>
    /// ��Ϸ�������
    /// </summary>
    public class Main : MonoBehaviour
    {

//#if UNITY_EDITOR || UNITY_STANDALONE
//        [SerializeField]
//        private uint m_SteamAppId;
//#endif

#region Mono Callback
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Init();
        }

        void Start()
        {

        }

        void Update()
        {
            GameManager.Update(Time.deltaTime);
        }

        void OnDestroy()
        {
            DeInit();
        }

        #endregion

        private void Init()
        {
            //���ز߻������������ļ�
            Data.TableConfig.Init();
            Language.LanguageMapper.Init();
            InitManagers();

            UI.UIManager.Instance.OpenUI(UI.UINameConfig.MainMenuUI);

//#if UNITY_EDITOR || UNITY_STANDALONE
//            InitSteam();
//#endif
        }

//        private void InitSteam()
//        {
//            try
//            {
//                Steamworks.SteamClient.Init(m_SteamAppId);
//            }
//            catch (Exception ex)
//            {
//                UI.MessageBox.ShowNotice_i18n("msg/steam/initfail");
//            }
//        }


        void InitManagers()
        {
            var managers = GetComponentsInChildren<ManagerBase>();
            foreach(var mgr in managers)
            {
                mgr.Init();
            }
            GameManager.Init();
        }


        private void DeInit()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Steamworks.SteamClient.Shutdown();
#endif     
            Data.TableConfig.DeInit();
            EasyPlayerPrefs.Save();
        }

#if UNITY_EDITOR


        [ContextMenu("ǿ����Ϸ����")]
        private void GameOver()
        {
            GameManager.GameOver();
        }
#endif
    }
}
