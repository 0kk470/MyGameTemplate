using UnityEditor;
using UnityEngine;

namespace SaltyFish.Editor
{
    public class SerializeEditorWindow : EditorWindow
    {
        protected SerializedObject m_SerializedObject;

        protected virtual void Awake()
        {
            m_SerializedObject = new SerializedObject(this);
        }

        protected virtual void OnValidate()
        {
            if (m_SerializedObject == null)
                m_SerializedObject = new SerializedObject(this);
        }
    }
}