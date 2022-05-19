using UnityEngine.UI;
using System;
using UnityEngine;
using Saltyfish.Util;

namespace Saltyfish.UI
{
    class MessageBox : UIBase
    {
        [SerializeField]
        private Text m_Message;

        [SerializeField]
        private Text m_btn1Name;

        [SerializeField]
        private Text m_btn2Name;

        [SerializeField]
        private Button m_btn1;

        [SerializeField]
        private Button m_btn2;

        [SerializeField]
        private LayoutGroup m_ButtonLayout;


        private Action m_btn1Action;

        private Action m_btn2Action;
        protected override void Awake()
        {
            m_btn1.onClick.AddListener(OnBtn1Click);
            m_btn2.onClick.AddListener(OnBtn2Click);
        }

        protected override void OnDestroy()
        {
            m_btn1Action = null;
            m_btn2Action = null;
            m_btn1.onClick.RemoveListener(OnBtn1Click);
            m_btn2.onClick.RemoveListener(OnBtn2Click);
        }

        protected override void OnHide()
        {
            base.OnHide();
            m_btn2Action = null;
            m_btn2Action = null;
        }


        public static void ShowConfirmMessage(string message, Action btn1Callback = null, Action btn2Callback = null, string btn1Name = "", string btn2Name = "")
        {
            MessageBox mb = UIManager.Instance.OpenUI(UINameConfig.MessageBox) as MessageBox;
            if (mb != null)
            {
                mb.m_btn2.gameObject.BetterSetActive(true);
                mb.m_ButtonLayout.SetLayoutHorizontal();
                mb.m_btn1Action = btn1Callback;
                mb.m_btn2Action = btn2Callback;
                mb.SetText(message, btn1Name, btn2Name);
            }
        }

        public static void ShowConfirmMessage_i18n(string key, Action btn1Callback = null, Action btn2Callback = null, string btn1Name = "", string btn2Name = "")
        {
            var message = Language.LanguageMapper.GetText(key);
            ShowConfirmMessage(message, btn1Callback, btn2Callback, btn1Name, btn2Name);
        }


        public static void ShowNotice_i18n(string localizationKey, Action btn1Callback = null, string btn1Name = "")
        {
            var message = Language.LanguageMapper.GetText(localizationKey);
            ShowNotice(message, btn1Callback, btn1Name);
        }


        public static void ShowNotice(string message, Action btn1Callback = null, string btn1Name = "")
        {
            MessageBox mb = UIManager.Instance.OpenUI(UINameConfig.MessageBox) as MessageBox;
            if (mb != null)
            {
                mb.m_btn2.gameObject.BetterSetActive(false);
                mb.m_ButtonLayout.SetLayoutHorizontal();
                mb.m_btn1Action = btn1Callback;
                mb.m_btn2Action = null;
                mb.SetText(message, btn1Name, string.Empty);
            }
        }


        private void SetText(string message, string btn1Name, string btn2Name)
        {
            if (string.IsNullOrEmpty(btn1Name))
                btn1Name = "确定";
            if (string.IsNullOrEmpty(btn2Name))
                btn2Name = "取消";
            m_Message.text = message;
            m_btn1Name.text = btn1Name;
            m_btn2Name.text = btn2Name;
        }

        private void OnBtn1Click()
        {
            Hide();
            m_btn1Action?.Invoke();
        }

        private void OnBtn2Click()
        {
            Hide();
            m_btn2Action?.Invoke();
        }
    }
}