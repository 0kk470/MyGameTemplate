using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Saltyfish.Util;
using Saltyfish.Data;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Saltyfish.Language
{
    public static class LanguageMapper
    {
        public static event Action<string> OnLanguageChanged;

        private static Dictionary<string, string> m_LanguageDic = new Dictionary<string, string>();

        private static string m_LanguageType;

        public static string LanguageTypeConfigFilePath => Path.Combine(Application.streamingAssetsPath, "Language/LanguageType.json");

        private static Dictionary<string, string> m_LanguagePathDic = new Dictionary<string, string>();

        public static List<string> AllLanguageKeys => m_LanguagePathDic.Keys.ToList();

        public static void Init()
        {
            if(LoadLanguageTypePathConfig())
            {
                LoadCurrentLanguage();
            }
        }



        public static string GetText(string key)
        {
            if(string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("key is null or empty");
                return "Empty Key";
            }
            string result = string.Empty;
            if (!m_LanguageDic.TryGetValue(key, out result))
            {
                Debug.LogWarningFormat("Cannot Find LanguageID:{0}, LanguageType:{1}", key, m_LanguageType);
                result = string.Format("ERROR_KEY:{0}", key);
            }
            return result;
        }

        public static bool TryGetText(string key, out string res)
        {
            res = string.Empty;
            if (!m_LanguageDic.TryGetValue(key, out res))
            {
                Debug.LogWarningFormat("Cannot Find LanguageID:{0}, LanguageType:{1}", key, m_LanguageType);
                res = string.Format("ERROR_KEY:{0}", key);
                return false;
            }
            return true;
        }

        public static string GetFormatText(string key, params object[] args)
        {
            string result;
            if (TryGetText(key, out result))
            {
                result = string.Format(result, args);
            }
            return result;
        }

        public static bool LoadLanguageTypePathConfig()
        {
            var configPath = LanguageTypeConfigFilePath;
            if (!File.Exists(configPath))
            {
                Debug.LogErrorFormat("File does not exist, path:{0}", configPath);
                return false;
            }
            try
            {
                var json = File.ReadAllText(configPath);
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (result == null)
                    return false;
                m_LanguagePathDic = result;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
            return true;
        }

        public static void LoadCurrentLanguage()
        {
            var _LocalType = EasyPlayerPrefs.GetString("Language", "Chinese");
            SetLanguage(_LocalType);
        }

        public static void SetLanguage(string _languageType)
        {
            if (m_LanguageType == _languageType)
                return;
            if (ReloadLanguageData(_languageType))
            {
                m_LanguageType = _languageType;
                EasyPlayerPrefs.SetString("Language", _languageType);
                OnLanguageChanged?.Invoke(_languageType);
                TableConfig.ConfigData.TranslateText(OnMapLanguage);
            }
        }

        private static string OnMapLanguage(string key, string originText)
        {
            if(TryGetText(key, out string result))
            {
                return result;
            }
            return originText;
        }

        private static bool ReloadLanguageData(string _languageType)
        {
            string filePath = string.Empty;
            if (!m_LanguagePathDic.TryGetValue(_languageType, out filePath))
            {
                Debug.LogErrorFormat("Language Data cannot find: {0}", _languageType);
                return false;
            }
            filePath = Path.Combine(Application.streamingAssetsPath, filePath);
            if (!File.Exists(filePath))
            {
                Debug.LogErrorFormat("File does not exist, path:{0}", filePath);
                return false;
            }
            try
            {
                var json = File.ReadAllText(filePath);
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (result == null)
                    return false;
                m_LanguageDic = result;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
            return true;
        }

#if UNITY_EDITOR
        #region Editor Tool
        [MenuItem("LubanTools/Generate LanguageFiles")]
        public static void GenLanguageFiles()
        {
            TableConfig.Init();

            var properties = typeof(cfg.LanguageData).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => !prop.Name.Contains("LanguageKey") && prop.PropertyType == typeof(string)).ToList();

            var allLanguageDic = new Dictionary<string, Dictionary<string, string>>();
            foreach(var prop in properties)
            {
                var langName = prop.Name;
                if (!allLanguageDic.ContainsKey(langName))
                {
                    allLanguageDic.Add(langName, new Dictionary<string, string>());
                }
                var dic = allLanguageDic[prop.Name];
                foreach (var langData in TableConfig.ConfigData.LanguageConfig.DataList)
                {
                    if(!dic.ContainsKey(langData.LanguageKey))
                    {
                        dic.Add(langData.LanguageKey, prop.GetValue(langData) as string);
                    }
                }
            }

            var languageTypePathDic = new Dictionary<string, string>();
            try
            {
                foreach (var kv in allLanguageDic)
                {
                    var langName = kv.Key;
                    var keyTextDic = kv.Value;
                    var relativePath =$"Language/{langName}.json";
                    languageTypePathDic.Add(langName, relativePath);
                    var jsonText = JsonConvert.SerializeObject(keyTextDic, Formatting.Indented);
                    File.WriteAllText(Path.Combine(Application.streamingAssetsPath, relativePath), jsonText);
                }
                var configJson = JsonConvert.SerializeObject(languageTypePathDic, Formatting.Indented);
                File.WriteAllText(LanguageTypeConfigFilePath, configJson);
                EditorUtility.DisplayDialog("info", "Gen LanguageFiles sucess", "Ok");
            }
            catch(Exception ex)
            {
                EditorUtility.DisplayDialog("error", "Gen LanguageFiles failed", "Ok");
                Debug.LogError(ex);
            }
            TableConfig.DeInit();
        }
        #endregion
#endif
    }
}
