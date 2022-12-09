using cfg;
using Newtonsoft.Json;
using Saltyfish.Data;
using Saltyfish.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Saltyfish.Archive
{
    public enum CurrencyType
    {
        Gold = 0,
    }

    public class GameData
    {
        public static GameData Current { get; set; }

        public GameData()
        {

        }


        #region Common Data

        [JsonIgnore]
        public bool IsTestMode { get; set; } = false;

        public string FileName { get; set; }

        [JsonIgnore]
        public string SavePath
        {
            get
            {
                return Path.Combine(ArchiveManager.ArchivePath, FileName + ".KBSaveGame");
            }
        }

        public string Version { get; set; }

        public long CreateDateTimeTick { get; set; }

        public long SaveDateTimeTick { get; set; }

        public float PlayTime { get; set; }

        [JsonIgnore]
        public bool IsExist
        {
            get
            {
                return File.Exists(SavePath);
            }
        }

        [JsonIgnore]
        public bool IsCurrentVersion
        {
            get
            {
                return Application.version == Version;
            }
        }

        [JsonProperty("IsNewGame")]
        public bool IsNewGame { get; set; } = true;

        #endregion

        #region GameRunData

        //当前所在地图
        [JsonProperty]
        public int MapId { get; set; }


        //货币
        [JsonProperty("Currency")]
        private CustomKeyValueData<CurrencyType, int> m_Currencies = new CustomKeyValueData<CurrencyType, int>();

        [JsonProperty("IntData")]
        private CustomKeyValueData<string, int> m_IntData = new CustomKeyValueData<string, int>();

        #endregion

        #region Currency API

        public void IncreateCurrency(CurrencyType currencyType, int val)
        {
            SetCurrency(currencyType, GetCurrency(currencyType) + val);
        }

        public void ReduceCurrency(CurrencyType currencyType, int val)
        {
            IncreateCurrency(currencyType, -val);
        }


        public void SetCurrency(CurrencyType currencyType, int newVal)
        {
            newVal = Math.Max(0, newVal);
            int oldVal = GetCurrency(currencyType);
            m_Currencies.SetValue(currencyType, newVal);
            EventManager.DispatchEvent(GameEventType.OnCurrencyChange, currencyType, oldVal, newVal);
        }

        public int GetCurrency(CurrencyType currencyType)
        {
            return m_Currencies.GetValue(currencyType);
        }

        #endregion 

        #region IntData

        public void SetInt(string key, int val)
        {
            m_IntData.SetValue(key, val);
        }

        public int GetInt(string key)
        {
            return m_IntData.GetValue(key);
        }

        #endregion

        #region Common


        public void ResetGameData()
        {
            //TODO
        }

        #endregion

        public bool Save()
        {
            if (IsTestMode)
                return false;
            try
            {
                SaveDateTimeTick = DateTime.Now.Ticks;
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return false;
        }


        public bool Delete()
        {
            if (!IsExist)
                return false;
            try
            {
                File.Delete(SavePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return false;
        }
    }
}
