using cfg;
using cfg.Tutorial;
using Saltyfish.Data;
using Saltyfish.ObjectPool;
using Saltyfish.Resource;
using Saltyfish.Util;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Saltyfish.UI
{
    public class TutorialUI : UIBase
    {
        [SerializeField]
        private Text m_Title;

        [SerializeField]
        private Text m_Description;

        [SerializeField]
        private Button m_PrevBtn;

        [SerializeField]
        private Button m_NextBtn;

        [SerializeField]
        private Button m_ConfirmBtn;

        [SerializeField]
        private SelectButtonGroup m_TutorialStepButtonGroup;

        [SerializeField]
        private LayoutGroup m_StepButtonLayout;

        [SerializeField]
        private Image m_TutorialImg;

        [SerializeField]
        private int m_CurStep = 0;

        private TutorialConfigData m_TutorialConfigData;

        public bool IsLastStep => m_TutorialConfigData != null && m_CurStep == m_TutorialConfigData.StepList.Count - 1;

        public bool IsFirstStep => m_CurStep == 0;

        public bool OnlySingleStep => m_TutorialConfigData != null && m_TutorialConfigData.StepList.Count == 1;


        protected override void Awake()
        {
            base.Awake();
            m_PrevBtn.onClick.AddListener(OnPrevBtnClick);
            m_NextBtn.onClick.AddListener(OnNextBtnClick);
            m_ConfirmBtn.onClick.AddListener(OnCofirmBtnClick);
            m_TutorialStepButtonGroup.OnButtonSelect.AddListener(OnStepSelect);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_PrevBtn.onClick.RemoveListener(OnPrevBtnClick);
            m_NextBtn.onClick.RemoveListener(OnNextBtnClick);
            m_ConfirmBtn.onClick.RemoveListener(OnCofirmBtnClick);
            m_TutorialStepButtonGroup.OnButtonSelect.RemoveListener(OnStepSelect);
        }

        public void SetTutorialConfigData(TutorialConfigData configData)
        {
            m_TutorialConfigData = configData;
            ResetTutorialDisplay();
        }

        private void ResetTutorialDisplay()
        {
            m_Title.text = m_TutorialConfigData?.Title;
            ReloadStepSelectionButtons();
            m_TutorialStepButtonGroup.SelectIndex(0);
        }

        private void ReloadStepSelectionButtons()
        {
            var stepList = m_TutorialConfigData.StepList;
            int i = 0;
            for (; i < stepList.Count; ++i)
            {
                if (i >= m_TutorialStepButtonGroup.Buttons.Count)
                {
                    CreateStepButton();
                }
            }
            while (i < m_TutorialStepButtonGroup.Buttons.Count)
            {
                m_TutorialStepButtonGroup.Buttons[i++].gameObject.BetterSetActive(false);
            }
            m_StepButtonLayout.SetLayoutHorizontal();
        }

        private void CreateStepButton()
        {
            var stepBtn = GoPoolMgr.CreateComponent_NoPool<SelectButton>(PrefabPath.TutorialStepButtonPath, m_TutorialStepButtonGroup.transform);
            m_TutorialStepButtonGroup.RegisterButton(stepBtn);
        }



        private void RefreshStepData(TutorialStep step)
        {
            if (step == null)
                return;
            m_Description.text = step.Description;
            m_TutorialImg.sprite = AssetCache.GetCache(CacheName.TutorialTexture).GetAsset<Sprite>(step.TexturePath);
        }


        private void OnPrevBtnClick()
        {
            if (m_TutorialConfigData == null)
                return;
            int step = Mathf.Clamp(m_CurStep - 1, 0, m_TutorialConfigData.StepList.Count - 1);
            m_TutorialStepButtonGroup.SelectIndex(step);
        }

        private void OnNextBtnClick()
        {
            if (m_TutorialConfigData == null)
                return;
            int step = Mathf.Clamp(m_CurStep + 1, 0, m_TutorialConfigData.StepList.Count - 1);
            m_TutorialStepButtonGroup.SelectIndex(step);
        }

        private void OnCofirmBtnClick()
        {
            Close();
        }

        private void OnStepSelect(int idx)
        {
            if (m_TutorialConfigData == null)
                return;
            if (idx < 0 || idx >= m_TutorialConfigData.StepList.Count)
                return;
            m_CurStep = idx;
            var stepData = m_TutorialConfigData.StepList[idx];
            RefreshStepData(stepData);
            UpdateStepButtonVisibility();
        }

        private void UpdateStepButtonVisibility()
        {
            m_NextBtn.gameObject.BetterSetActive(!IsLastStep);
            m_PrevBtn.gameObject.BetterSetActive(!IsFirstStep);
            m_StepButtonLayout.gameObject.BetterSetActive(!OnlySingleStep);
            m_ConfirmBtn.gameObject.BetterSetActive(IsLastStep);
        }


        public static void OpenTutorial(int tutorialId, bool isForce = false)
        {
#if !UNITY_EDITOR
            if (EasyPlayerPrefs.GetBool("Tutorial" + tutorialId) && !isForce)
                return;
            EasyPlayerPrefs.SetBool("Tutorial" + tutorialId, true);
#endif
            var tutorialData = TableConfig.ConfigData.TutorialConfig.GetOrDefault(tutorialId);
            if (tutorialData == null)
            {
                Debug.LogError("Cannot find tutorial config, Id:" + tutorialId);
                return;
            }
            var tutorialUI = UIManager.Instance.OpenUI(UINameConfig.TutorialUI) as TutorialUI;
            tutorialUI?.SetTutorialConfigData(tutorialData);
        }
    }
}
