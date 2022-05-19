using System.Collections.Generic;

namespace Saltyfish.UI
{
    public static class UINameConfig
    {
        public const string MainMenuUI = "MainMenuUI";
        public const string MessageBox = "MessageBox";
        public const string PauseUI = "PauseUI";
        public const string GameOverUI = "GameOverUI";
        public const string BlackCurtain = "BlackCurtain";
        public const string MainGameUI = "MainGameUI";
        public const string GameSettingUI = "GameSettingUI";
        public const string TutorialUI = "TutorialUI";
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
            { UINameConfig.MessageBox,              new UIConfigParams(UINameConfig.MessageBox,           UILayer.Tip,          "Prefabs/UI/MessageBox") },
            { UINameConfig.PauseUI,                 new UIConfigParams(UINameConfig.PauseUI,              UILayer.Top,          "Prefabs/UI/PauseUI") },
            { UINameConfig.GameOverUI,              new UIConfigParams(UINameConfig.GameOverUI,           UILayer.Middle,       "Prefabs/UI/GameOverUI") },
            { UINameConfig.BlackCurtain,            new UIConfigParams(UINameConfig.BlackCurtain,         UILayer.Curtain,      "Prefabs/UI/BlackCurtain") },
            { UINameConfig.MainGameUI,              new UIConfigParams(UINameConfig.MainGameUI,           UILayer.Bottom,       "Prefabs/UI/MainGameUI") },
            { UINameConfig.GameSettingUI,           new UIConfigParams(UINameConfig.GameSettingUI,        UILayer.Top,       "Prefabs/UI/GameSettingUI") },
            { UINameConfig.TutorialUI,              new UIConfigParams(UINameConfig.TutorialUI,           UILayer.Tutorial,     "Prefabs/UI/Tutorial/TutorialUI") },
        };
    }
}