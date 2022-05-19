using cfg;
using System;
using System.IO;
using UnityEngine;

namespace Saltyfish.Data
{
    public static class TableConfig
    {
        private static bool m_isLoaded;

        private static Tables m_Config;

        public static Tables ConfigData
        {
            get
            {
                if(m_Config == null)
                {
                    LoadConfig();
                }
                return m_Config;
            }
        }

        private static readonly string ConfigPath = Path.Combine(Application.streamingAssetsPath, "ExcelData/Json");

        public static void Init()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            if (m_isLoaded)
                return;
            try
            {
                m_Config = new cfg.Tables(file => SimpleJSON.JSON.Parse(System.IO.File.ReadAllText(ConfigPath + "/" + file + ".json")));
                m_isLoaded = true;
            }
            catch(Exception ex)
            {
                Debug.LogError("Load ExcelConfig failed");
                Debug.LogError(ex);
                m_isLoaded = false;
            }
        }


        public static void DeInit()
        {
            m_Config = null;
            m_isLoaded = false;
        }
    }
}
