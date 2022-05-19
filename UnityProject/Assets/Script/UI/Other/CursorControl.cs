using Saltyfish.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saltyfish.UI
{

    public enum CursorState
    {
        None = 0,
        CursorSprite1,
        CursorSprite2,
        CursorSprite3,
        Max,
    }

    public class CursorControl:MonoBehaviour
    {
        [SerializeField]
        private CursorState m_CursorState = CursorState.None;

        [SerializeField]
        private Texture2D[] m_CursorSprites = new Texture2D[(int)CursorState.Max];


        [SerializeField]
        private bool m_IsDraggingFood = false;

        [SerializeField]
        private bool m_HoveredFood = false;

        private void Awake()
        {
            //register event here
        }

        private void OnDestroy()
        {
            //unregister event here
        }

        public void SetCursorState(CursorState newState)
        {
            if (m_CursorState == newState)
                return;
            m_CursorState = newState;
            Cursor.SetCursor(m_CursorSprites[(int)m_CursorState], Vector2.zero, CursorMode.Auto);
        }

    }
}
