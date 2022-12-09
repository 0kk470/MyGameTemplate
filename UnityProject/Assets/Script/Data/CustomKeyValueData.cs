using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saltyfish.Data
{
    [Serializable]
    public class CustomKeyValueData<TKey, TValue>
    {
        [JsonProperty("Data")]
        private Dictionary<TKey, TValue> m_DataDic = new Dictionary<TKey, TValue>();

        public void Clear()
        {
            m_DataDic.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return m_DataDic.ContainsKey(key);
        }

        public void SetValue(TKey key, TValue val)
        {
            if(m_DataDic.ContainsKey(key))
            {
                m_DataDic[key] = val;
            }
            else
            {
                m_DataDic.Add(key, val);
            }
        }

        public TValue GetValue(TKey key)
        {
            m_DataDic.TryGetValue(key, out TValue val);
            return val;
        }
    }
}
