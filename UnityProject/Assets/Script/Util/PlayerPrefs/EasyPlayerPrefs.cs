
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Saltyfish.Util
{

    public static class EasyPlayerPrefs
    {

        private static Dictionary<string, int> m_IntDic = new Dictionary<string, int>();

        private static Dictionary<string, bool> m_BoolDic = new Dictionary<string, bool>();

        private static Dictionary<string, float> m_FloatDic = new Dictionary<string, float>();

        private static Dictionary<string, string> m_StringDic = new Dictionary<string, string>();


        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PlayerPrefs/PlayerPrefs.json");

        private const string INT_DIC = "IntDic";

        private const string BOOL_DIC = "BoolDic";

        private const string FLOAT_DIC = "FloatDic";

        private const string STRING_DIC = "StringDic";


        public static void DeleteAll()
        {
            m_IntDic.Clear();
            m_FloatDic.Clear();
            m_StringDic.Clear();
            m_BoolDic.Clear();
            Save();
        }

        public static void DeleteKey(string key)
        {
            if(m_IntDic.ContainsKey(key))
            {
                m_IntDic.Remove(key);
            }
            if(m_StringDic.ContainsKey(key))
            {
                m_StringDic.Remove(key);
            }
            if(m_BoolDic.ContainsKey(key))
            {
                m_BoolDic.Remove(key);
            }
            if(m_FloatDic.ContainsKey(key))
            {
                m_FloatDic.Remove(key);
            }
            Save();
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            return GetValue(m_FloatDic, key, defaultValue);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return GetValue(m_IntDic, key, defaultValue);
        }


        public static string GetString(string key, string defaultValue = "")
        {
            return GetValue(m_StringDic, key, defaultValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return GetValue(m_BoolDic, key, defaultValue);
        }


        public static bool HasKey(string key)
        {
            return m_StringDic.ContainsKey(key) || m_BoolDic.ContainsKey(key) || m_IntDic.ContainsKey(key) || m_FloatDic.ContainsKey(key);
        }

        public static void Save()
        {
            CheckPrefs();
            var jObject = new JObject();
            jObject.Add(INT_DIC,JsonConvert.SerializeObject(m_IntDic));
            jObject.Add(BOOL_DIC, JsonConvert.SerializeObject(m_BoolDic));
            jObject.Add(FLOAT_DIC, JsonConvert.SerializeObject(m_FloatDic));
            jObject.Add(STRING_DIC, JsonConvert.SerializeObject(m_StringDic));
            try
            {
                File.WriteAllText(SavePath, jObject.ToString());
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("Save Playerprefs to json failed");
            }
        }

        public static void Init()
        {
            CheckPrefs();
            ReLoad();
        }

        public static void ReLoad()
        {
            m_IntDic.Clear();
            m_FloatDic.Clear();
            m_StringDic.Clear();
            m_BoolDic.Clear();
            try
            {
                var jsonData = File.ReadAllText(SavePath);
                JObject obj = JObject.Parse(jsonData);
                if (obj != null)
                {
                    LoadHelper(obj.GetValue(INT_DIC), ref m_IntDic);
                    LoadHelper(obj.GetValue(BOOL_DIC), ref m_BoolDic);
                    LoadHelper(obj.GetValue(FLOAT_DIC), ref m_FloatDic);
                    LoadHelper(obj.GetValue(STRING_DIC), ref m_StringDic);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("Load PlayerPrefs from json failed");
            }
        }

        private static void LoadHelper<T>(JToken token, ref Dictionary<string, T> targetDic)
        {
            if(token == null)
                return;
            var storedDic = JsonConvert.DeserializeObject<Dictionary<string,T>>(token.ToString());
            if(storedDic != null)
            {
                targetDic = storedDic;
            }
        }

        public static void SetFloat(string key, float value)
        {
            SetValue(m_FloatDic, key,value);
        }
        public static void SetInt(string key, int value)
        {
            SetValue(m_IntDic, key,value);  
        }
        public static void SetString(string key, string value)
        {
            SetValue(m_StringDic, key,value);
        }

        public static void SetBool(string key, bool value)
        {
            SetValue(m_BoolDic, key,value);
        }

        private static void CheckPrefs()
        {
            if(!File.Exists(SavePath))
            {
                var dirName = Path.GetDirectoryName(SavePath);
                if(!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                var f = File.Create(SavePath);
                f.Dispose();
                f.Close();
            }
        }

        private static void SetValue<T>(Dictionary<string,T> dic, string key, T val)
        {
            if(dic.ContainsKey(key))
            {
                dic[key] = val;
            }
            else
            {
                dic.Add(key, val);
            }
        }

        private static T GetValue<T>(Dictionary<string,T> dic, string key, T defaultValue)
        {
            if(dic.ContainsKey(key))
                return dic[key];
            return defaultValue;
        }
    }

}