using UnityEngine;
using UnityEngine.EventSystems;

namespace Saltyfish.Util
{
    public class Dragable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        protected Collider2D m_Collider2D;

        public Transform CurrentDropContainer;

        public Transform OldDropContainer;

        protected Vector3 m_StartPos;

        protected bool m_isDragging = false;

        [SerializeField]
        private bool m_CanDrag = true;

        [SerializeField]
        private bool m_CanMoveIfNoChange = false;

        public bool CanDrag { get => m_CanDrag; set => m_CanDrag = value; }

        void Awake()
        {
            m_Collider2D = GetComponent<Collider2D>();
        }


        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanDrag)
                return;
            OldDropContainer = transform.parent;
            CurrentDropContainer = OldDropContainer;
            m_StartPos = transform.localPosition;
            m_isDragging = true;
            m_Collider2D.enabled = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {

            if (!m_isDragging)
                return;

            var worldPos = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = -1;
            transform.position = worldPos;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            StopDrag();
        }

        protected virtual void StopDrag()
        {
            if (!m_isDragging)
                return;
            Debug.Log("Stop drag");
            if (CurrentDropContainer != OldDropContainer)
            {
                OnContainerChanged();
            }
            else
            {
                Debug.Log("No Change");
                if(m_CanMoveIfNoChange && IsStillOnOldContainer)
                {
                    Debug.Log("change position on current container");
                }
                else
                {
                    transform.SetParent(OldDropContainer);
                    transform.localPosition = m_StartPos;
                }
            }
            m_isDragging = false;
            m_Collider2D.enabled = true;
            OldDropContainer = CurrentDropContainer;
        }

        void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                if(m_isDragging)
                {
                    ResetToOldContainer();
                    m_isDragging = false;
                    m_Collider2D.enabled = true;
                }
            }
        }

        protected virtual void OnContainerChanged()
        {

        }

        private void ResetToOldContainer()
        {
            Debug.Log("Reset");
            CurrentDropContainer = OldDropContainer;
            transform.SetParent(OldDropContainer);
            transform.localPosition = Vector3.zero;
        }

        private bool IsStillOnOldContainer
        {
            get
            {
                if (OldDropContainer == null)
                    return false;
                var containerCollider = OldDropContainer.GetComponent<Collider2D>();
                if (containerCollider == null)
                    return false;
                Vector2 pos = transform.position; 
                bool ret =  containerCollider.bounds.Contains(pos);
                return ret;
            }
        }
    }
}
