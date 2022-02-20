using Saltyfish.Util;
using System.Collections;
using UnityEngine;

namespace Saltyfish.UI
{

    public class UIBase : MonoBehaviour
    {

        public bool IsOpen { get; protected set; }

        public string uiName { get; set; }

        public UILayer Layer { get; set; }

        public int Sibling
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }


        protected UIBase[] m_Childs;

        protected virtual void Awake()
        {

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

        }

        public void Show(bool needRefresh = true)
        {
            gameObject.BetterSetActive(true);
            if (needRefresh)
                Refresh();
            IsOpen = true;
            OnShow();
        }

        protected virtual void OnHide()
        {

        }

        public void Hide()
        {
            gameObject.BetterSetActive(false);
            IsOpen = false;
            OnHide();
        }

        public void Close()
        {
            Hide();
            Destroy(gameObject);
            UIManager.Instance.RemoveUI(uiName);
        }
    }
}