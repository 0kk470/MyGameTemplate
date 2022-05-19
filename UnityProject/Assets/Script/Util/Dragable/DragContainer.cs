
using UnityEngine;
using UnityEngine.EventSystems;

namespace Saltyfish.Util
{
    public class DragContainer : MonoBehaviour,IDropHandler
    {
        [SerializeField]
        private bool m_ResetDropPosition = true;

        public void OnDrop(PointerEventData eventData)
        {
            var dragObj = eventData.pointerDrag;
            if (dragObj == null)
                return;
            var draggable = dragObj.GetComponent<Dragable>();
            if(draggable != null)
            {
                OnDragableDrop(draggable);
            }
        }

        protected virtual void OnDragableDrop(Dragable dragable)
        {
            Debug.Log($"[{dragable.name}] dropped on [{transform.name}]");
            dragable.CurrentDropContainer = transform;
            dragable.transform.SetParent(transform);
            if(m_ResetDropPosition)
            {
                dragable.transform.localPosition = Vector3.zero;
            }
        }
    }
}