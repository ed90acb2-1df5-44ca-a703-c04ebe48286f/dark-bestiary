using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TextWithIcon : PoolableMonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;

        public Image Icon => this.icon;

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