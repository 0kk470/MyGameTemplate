//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg
{

public sealed partial class LanguageData :  Bright.Config.BeanBase 
{
    public LanguageData(JSONNode _json) 
    {
        { if(!_json["languageKey"].IsString) { throw new SerializationException(); }  LanguageKey = _json["languageKey"]; }
        { if(!_json["Chinese"].IsString) { throw new SerializationException(); }  Chinese = _json["Chinese"]; }
        { if(!_json["English"].IsString) { throw new SerializationException(); }  English = _json["English"]; }
        PostInit();
    }

    public LanguageData(string languageKey, string Chinese, string English ) 
    {
        this.LanguageKey = languageKey;
        this.Chinese = Chinese;
        this.English = English;
        PostInit();
    }

    public static LanguageData DeserializeLanguageData(JSONNode _json)
    {
        return new LanguageData(_json);
    }

    /// <summary>
    /// 唯一id
    /// </summary>
    public string LanguageKey { get; private set; }
    /// <summary>
    /// 中文
    /// </summary>
    public string Chinese { get; private set; }
    /// <summary>
    /// 英文
    /// </summary>
    public string English { get; private set; }

    public const int __ID__ = -1928011966;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "LanguageKey:" + LanguageKey + ","
        + "Chinese:" + Chinese + ","
        + "English:" + English + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
