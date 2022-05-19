using TMPro;
using UnityEngine;

namespace Saltyfish.Language
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextTranslator : MonoBehaviour
    {
        [SerializeField]
        private string m_LanguageKey;

        private TMP_Text m_Text;

        private void Awake()
        {
            m_Text = GetComponent<TMP_Text>();
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
            if (!Application.isPlaying)
                return;
            if (string.IsNullOrEmpty(m_LanguageKey))
            {
                Debug.LogWarning("No language key on this object");
                return;
            }
            m_Text.text = LanguageMapper.GetText(m_LanguageKey);
        }
    }
}
