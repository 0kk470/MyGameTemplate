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
        /// ��������ܹ�ͨ��ȫ�ֻ�ȡ�Ļ����������ش˺�����UI��ʱˢ��
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