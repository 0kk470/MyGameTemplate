using Saltyfish.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Logic
{
    public enum ScrollDirection
    {
        None = 0,
        Left,
        Right,
        Top,
        Down,
        LeftTop,
        LeftDown,
        RightTop,
        RightDown,
    }

    public class CameraScroller:MonoBehaviour
    {
        [SerializeField]
        [Range(0.1f, 50)]
        private float m_ScrollSpeed = 30;

        private Vector2 m_ScrollPadding = new Vector2(20, 20);

        public ScrollDirection scrollDirection { get; private set; }

        public void OnUpdate()
        {
            UpdateCameraScroll();
        }

        private void UpdateCameraScroll()
        {
            var scrollDirection = GetScrollDirectionType();
            if (scrollDirection == ScrollDirection.None)
                return;
            var cam = CameraManager.Instance.MainCamera;
            if (cam == null)
                return;
            var direction = GetScrollDirection(scrollDirection);
            var deltaPosition = direction * m_ScrollSpeed * Time.deltaTime;
            var camPosition = cam.transform.position;
            var nextPosition = new Vector2(camPosition.x, camPosition.y) + deltaPosition;
            CameraManager.Instance.SetPlanePositon(nextPosition);
            //Debug.Log(direction);
        }

        private ScrollDirection GetScrollDirectionType()
        {
            var mousePosition = UnityEngine.Input.mousePosition;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            if(mousePosition.x <= m_ScrollPadding.x)
            {
                if (mousePosition.y <= m_ScrollPadding.y)
                    return ScrollDirection.LeftDown;
                if (mousePosition.y >= screenHeight - m_ScrollPadding.y)
                    return ScrollDirection.LeftTop;
                return ScrollDirection.Left;
            }

            if(mousePosition.x >= screenWidth - m_ScrollPadding.x)
            {
                if (mousePosition.y <= m_ScrollPadding.y)
                    return ScrollDirection.RightDown;
                if (mousePosition.y >= screenHeight - m_ScrollPadding.y)
                    return ScrollDirection.RightTop;
                return ScrollDirection.Right;
            }

            if(mousePosition.y <= m_ScrollPadding.y)
                return ScrollDirection.Down;

            if (mousePosition.y >= screenHeight - m_ScrollPadding.y)
                return ScrollDirection.Top;
            return ScrollDirection.None;
        }

        private Vector2 GetScrollDirection(ScrollDirection dirType)
        {
            switch (dirType)
            {
                case ScrollDirection.Left:
                    return Vector2.left;
                case ScrollDirection.Right:
                    return Vector2.right;
                case ScrollDirection.Top:
                    return Vector2.up;
                case ScrollDirection.Down:
                    return Vector2.down;
                case ScrollDirection.LeftTop:
                    return new Vector2(-1, 1).normalized;
                case ScrollDirection.LeftDown:
                    return new Vector2(-1, -1).normalized;
                case ScrollDirection.RightTop:
                    return new Vector2(1, 1).normalized;
                case ScrollDirection.RightDown:
                    return new Vector2(1, -1).normalized;
                default:
                    return Vector2.zero;
            }
        }
    }
}
