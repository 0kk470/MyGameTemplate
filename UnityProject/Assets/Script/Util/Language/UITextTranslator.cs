using UnityEngine;
using UnityEngine.UI;

namespace Saltyfish.Language
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class UITextTranslator:MonoBehaviour
    {
        [SerializeField]
        private string m_LanguageKey;

        private Text m_Text;

        private void Awake()
        {
            m_Text = GetComponent<Text>();
            RefreshText();
            LanguageMapper.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            LanguageMapper.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(string languageName)
        {
            RefreshText();
        }

        private void RefreshText()
        {
            if(string.IsNullOrEmpty(m_LanguageKey))
            {
                Debug.LogWarning("No language key on this object");
                return;
            }
            m_Text.text = LanguageMapper.GetText(m_LanguageKey);
        }
    }
}
