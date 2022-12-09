using Cysharp.Threading.Tasks;
using Saltyfish.Util;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Saltyfish.UI
{

    public class UIBase : MonoBehaviour
    {

        public bool IsOpen { get; protected set; }

        public string uiName { get; set; }

        public UILayer Layer { get; set; }

        public bool UpdateEveryFrame { get; set; } = false;

        public int Sibling
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }


        protected UIBase[] m_Childs;

        //private IUIPanelInput m_UIPanelInput;


        protected virtual void Awake()
        {
           // m_UIPanelInput = GetComponent<IUIPanelInput>();
        }

        protected virtual void Start()
        {

        }


        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 如果数据能够通过全局获取的话，可以重载此函数在UI打开时刷新
        /// </summary>
        public virtual void Refresh()
        {

        }

        protected virtual void OnShow()
        {
            RegisterUIInput();
        }

        public void Show(bool needRefresh = true)
        {
            transform.SetAsLastSibling();
            gameObject.BetterSetActive(true);
            if (needRefresh)
                Refresh();
            IsOpen = true;
            OnShow();
        }

        protected virtual void OnHide()
        {
            UnRegisterUIInput();
        }

        private Coroutine m_Co_DelayHide;
        protected void DelayHide(float duration = 0.1f, bool isDestroy = false)
        {
            StopDelayHide();
            m_Co_DelayHide = StartCoroutine(Co_DelayHide(duration, isDestroy));
        }

        protected void StopDelayHide()
        {
            if (m_Co_DelayHide != null)
            {
                StopCoroutine(m_Co_DelayHide);
                m_Co_DelayHide = null;
            }
        }

        protected IEnumerator Co_DelayHide(float duration, bool isDestroy)
        {
            yield return new WaitForSeconds(duration);
            if (isDestroy)
            {
                Close();
            }
            else
            {
                Hide();
            }
        }


        public void Hide()
        {
            gameObject.BetterSetActive(false);
            IsOpen = false;
            OnHide();
        }

        public virtual void OnUpdate(float deltaTime)
        {
            
        }


        public void Close()
        {
            Hide();
            Destroy(gameObject);
            UIManager.Instance.RemoveUI(uiName);
        }

        private void RegisterUIInput()
        {
           //UIPanel_InputManager.Instance.AddInputControl(m_UIPanelInput);
        }

        private void UnRegisterUIInput()
        {
            //UIPanel_InputManager.Instance.RemoveInputControl(m_UIPanelInput);
        }
    }
}