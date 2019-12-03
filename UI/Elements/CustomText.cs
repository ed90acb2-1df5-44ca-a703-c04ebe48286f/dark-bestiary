using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CustomText : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public FontStyles Style
        {
            get => this.text.fontStyle;
            set => this.text.fontStyle = value;
        }

        public Color Color
        {
            get => this.text.color;
            set => this.text.color = value;
        }

        public TextAlignmentOptions Alignment
        {
            get => this.text.alignment;
            set => this.text.alignment = value;
        }

        public string Text
        {
            get => this.text.text;
            set => this.text.text = value;
        }
    }
}