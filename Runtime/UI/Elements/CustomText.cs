using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CustomText : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;

        public FontStyles Style
        {
            get => m_Text.fontStyle;
            set => m_Text.fontStyle = value;
        }

        public Color Color
        {
            get => m_Text.color;
            set => m_Text.color = value;
        }

        public TextAlignmentOptions Alignment
        {
            get => m_Text.alignment;
            set => m_Text.alignment = value;
        }

        public string Text
        {
            get => m_Text.text;
            set => m_Text.text = value;
        }
    }
}