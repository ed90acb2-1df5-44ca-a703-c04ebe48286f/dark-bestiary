using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CharacterInfoRow : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Interactable hover;

        private string tooltip;

        private void Start()
        {
            if (this.hover == null)
            {
                return;
            }

            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;
        }

        public void Construct(string label, string value)
        {
            SetLabel(label);
            SetValue(value);
        }

        public void SetLabel(string label)
        {
            this.labelText.text = label;
        }

        public void SetValue(string value)
        {
            this.valueText.text = value;
        }

        public void SetValueColor(Color color)
        {
            this.valueText.color = color;
        }

        public void SetTooltip(string tooltip)
        {
            this.tooltip = tooltip;
        }

        private void OnPointerEnter()
        {
            if (string.IsNullOrEmpty(this.tooltip))
            {
                return;
            }

            Tooltip.Instance.Show(this.tooltip, GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            if (string.IsNullOrEmpty(this.tooltip))
            {
                return;
            }

            Tooltip.Instance.Hide();
        }
    }
}