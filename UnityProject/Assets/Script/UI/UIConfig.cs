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
        public const string BattlePrepareUI = "BattlePrepareUI";
        public const string BattleInfoUI = "BattleInfoUI";
        public const string ConsoleUI = "ConsoleUI";
        public const string SkillBattleUI = "SkillBattleUI";
        public const string AlarmUI = "AlarmUI";
        public const string ArchiveGameUI = "ArchiveGameUI";
        public const string ShipRepositoryUI = "ShipRepositoryUI";
        public const string ShipRenameUI = "ShipRenameUI";
        public const string ShipLevelSelectionUI = "ShipLevelSelectionUI";
        public const string ShipUpgradeResultUI = "ShipUpgradeResultUI";
        public const string PopTipUI = "PopTipUI";
        public const string SkillBagUI = "SkillBagUI";
        public const string SupportUpgradeUI = "SupportUpgradeUI";
        public const string ShipSelectOperateUI = "ShipSelectOperateUI";
        public const string BattleSummaryUI = "BattleSummaryUI";
        public const string DialogueUI = "DialogueUI";
        public const string StoryIllustrationUI = "StoryIllustrationUI";
        public const string ShipComposeUI = "ShipComposeUI";
        public const string NewShipDisplayUI = "NewShipDisplayUI";
    }

    public class UIConfig
    {
        public class UIConfigParams
        {
            public string Name { get; set; }
            public UILayer Layer { get; set; }
            public string Path { get; set; }
            public bool FrameUpdate { get; set; }

            public UIConfigParams(string _name, UILayer _layer, string _path, bool _frameUpdate = false)
            {
                Name = _name;
                Layer = _layer;
                Path = _path;
                FrameUpdate = _frameUpdate;
            }
        }

        public static Dictionary<string, UIConfigParams> UIPath { get; } = new Dictionary<string, UIConfigParams>()
        {
            { UINameConfig.MainMenuUI,              new UIConfigParams(UINameConfig.MainMenuUI,           UILayer.Bottom,       "Prefabs/UI/MainMenuUI") },
            { UINameConfig.MessageBox,              new UIConfigParams(UINameConfig.MessageBox,           UILayer.Tip,          "Prefabs/UI/MessageBox") },
            { UINameConfig.PauseUI,                 new UIConfigParams(UINameConfig.PauseUI,              UILayer.Top,          "Prefabs/UI/PauseUI") },
            { UINameConfig.GameOverUI,              new UIConfigParams(UINameConfig.GameOverUI,           UILayer.Middle,       "Prefabs/UI/GameOverUI") },
            { UINameConfig.BattleSummaryUI,         new UIConfigParams(UINameConfig.BattleSummaryUI,      UILayer.Middle,       "Prefabs/UI/Battle/BattleSummaryUI") },
            { UINameConfig.BlackCurtain,            new UIConfigParams(UINameConfig.BlackCurtain,         UILayer.Curtain,      "Prefabs/UI/BlackCurtain") },
            { UINameConfig.MainGameUI,              new UIConfigParams(UINameConfig.MainGameUI,           UILayer.Bottom,       "Prefabs/UI/MainGameUI") },
            { UINameConfig.GameSettingUI,           new UIConfigParams(UINameConfig.GameSettingUI,        UILayer.Top,          "Prefabs/UI/GameSettingUI") },
            { UINameConfig.TutorialUI,              new UIConfigParams(UINameConfig.TutorialUI,           UILayer.Tutorial,     "Prefabs/UI/Tutorial/TutorialUI") },
            { UINameConfig.BattlePrepareUI,         new UIConfigParams(UINameConfig.BattlePrepareUI,      UILayer.Bottom,       "Prefabs/UI/Battle/BattlePrepareUI") },
            { UINameConfig.BattleInfoUI,            new UIConfigParams(UINameConfig.BattleInfoUI,         UILayer.Lowest,       "Prefabs/UI/Battle/BattleInfoUI") },
            { UINameConfig.SkillBattleUI,           new UIConfigParams(UINameConfig.SkillBattleUI,        UILayer.Lowest,       "Prefabs/UI/Skill/SkillBattleUI", true) },
            { UINameConfig.AlarmUI,                 new UIConfigParams(UINameConfig.AlarmUI,              UILayer.Lowest,       "Prefabs/UI/Battle/AlarmUI") },
            { UINameConfig.ConsoleUI,               new UIConfigParams(UINameConfig.ConsoleUI,            UILayer.Top,          "Prefabs/UI/ConsoleUI") },
            { UINameConfig.ArchiveGameUI,           new UIConfigParams(UINameConfig.ArchiveGameUI,        UILayer.Bottom,       "Prefabs/UI/Archive/ArchiveGameUI") },
            { UINameConfig.ShipRepositoryUI,        new UIConfigParams(UINameConfig.ShipRepositoryUI,     UILayer.Bottom,       "Prefabs/UI/Recruit/ShipRepositoryUI") },
            { UINameConfig.ShipRenameUI,            new UIConfigParams(UINameConfig.ShipRenameUI,         UILayer.Middle,       "Prefabs/UI/Recruit/ShipRenameUI") },
            { UINameConfig.ShipLevelSelectionUI,    new UIConfigParams(UINameConfig.ShipLevelSelectionUI, UILayer.Middle,       "Prefabs/UI/Recruit/ShipLevelSelectionUI") },
            { UINameConfig.ShipUpgradeResultUI,     new UIConfigParams(UINameConfig.ShipUpgradeResultUI,  UILayer.Middle,       "Prefabs/UI/Recruit/ShipUpgradeResultUI") },
            { UINameConfig.PopTipUI,                new UIConfigParams(UINameConfig.PopTipUI,             UILayer.Tip,          "Prefabs/UI/Tip/PopTipUI") },
            { UINameConfig.SkillBagUI,              new UIConfigParams(UINameConfig.SkillBagUI,           UILayer.Bottom,       "Prefabs/UI/Skill/SkillBag/SkillBagUI") },
            { UINameConfig.SupportUpgradeUI,        new UIConfigParams(UINameConfig.SupportUpgradeUI,     UILayer.Bottom,       "Prefabs/UI/Upgrade/SupportUpgradeUI") },
            { UINameConfig.ShipSelectOperateUI,     new UIConfigParams(UINameConfig.ShipSelectOperateUI,  UILayer.Middle,       "Prefabs/UI/Recruit/ShipSelectOperateUI") },
            { UINameConfig.DialogueUI,              new UIConfigParams(UINameConfig.DialogueUI,           UILayer.Middle,       "Prefabs/UI/Story/DialogueUI") },
            { UINameConfig.StoryIllustrationUI,     new UIConfigParams(UINameConfig.StoryIllustrationUI,  UILayer.Middle,       "Prefabs/UI/Story/StoryIllustrationUI")},
            { UINameConfig.ShipComposeUI,           new UIConfigParams(UINameConfig.ShipComposeUI,        UILayer.Bottom,       "Prefabs/UI/Recruit/ShipComposeUI")},
            { UINameConfig.NewShipDisplayUI,        new UIConfigParams(UINameConfig.NewShipDisplayUI,     UILayer.Middle,       "Prefabs/UI/Recruit/NewShipDisplayUI")}
        };
    }
}