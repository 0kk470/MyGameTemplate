using System.Collections.Generic;

namespace Saltyfish.UI
{
    public static class UINameConfig
    {
        public const string MainMenuUI = "MainMenuUI";
        public const string MessageBox = "MessageBox";
        public const string PlayerUI = "PlayerUI";
        public const string GameOverUI = "GameOverUI";
        public const string BlackCurtain = "BlackCurtain";
        public const string WeaponBuffSelectUI = "WeaponBuffSelectUI";
        public const string GameSettingUI = "GameSettingUI";
    }

    public class UIConfig
    {
        public class UIConfigParams
        {
            public string Name { get; set; }
            public UILayer Layer { get; set; }
            public string Path { get; set; }

            public UIConfigParams(string _name, UILayer _layer, string _path)
            {
                Name = _name;
                Layer = _layer;
                Path = _path;
            }
        }

        public static Dictionary<string, UIConfigParams> UIPath { get; } = new Dictionary<string, UIConfigParams>()
        {
            { UINameConfig.MainMenuUI,              new UIConfigParams(UINameConfig.MainMenuUI,           UILayer.Bottom,       "Prefabs/UI/MainMenuUI") },
            { UINameConfig.MessageBox,              new UIConfigParams(UINameConfig.MessageBox,           UILayer.Top,          "Prefabs/UI/MessageBox") },
            { UINameConfig.PlayerUI,                new UIConfigParams(UINameConfig.PlayerUI,             UILayer.Lowest,       "Prefabs/UI/PlayerUI") },
            { UINameConfig.GameOverUI,              new UIConfigParams(UINameConfig.GameOverUI,           UILayer.Middle,       "Prefabs/UI/GameOverUI") },
            { UINameConfig.BlackCurtain,            new UIConfigParams(UINameConfig.BlackCurtain,         UILayer.Middle,       "Prefabs/UI/BlackCurtain") },
            { UINameConfig.WeaponBuffSelectUI,      new UIConfigParams(UINameConfig.WeaponBuffSelectUI,   UILayer.Bottom,       "Prefabs/UI/WeaponBuffSelectUI") },
            { UINameConfig.GameSettingUI,           new UIConfigParams(UINameConfig.GameSettingUI,        UILayer.Bottom,       "Prefabs/UI/GameSettingUI") },
        };
    }
}