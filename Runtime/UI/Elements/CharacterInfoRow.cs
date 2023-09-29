using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CharacterInfoRow : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_LabelText;
        [SerializeField] private TextMeshProUGUI m_ValueText;
        [SerializeField] private Interactable m_Hover;

        private string m_Tooltip;

        private void Start()
        {
            if (m_Hover == null)
            {
                return;
            }

            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;
        }

        public void Construct(string label, string value)
        {
            SetLabel(label);
            SetValue(value);
        }

        public void SetLabel(string label)
        {
            m_LabelText.text = label;
        }

        public void SetValue(string value)
        {
            m_ValueText.text = value;
        }

        public void SetValueColor(Color color)
        {
            m_ValueText.color = color;
        }

        public void SetTooltip(string tooltip)
        {
            m_Tooltip = tooltip;
        }

        private void OnPointerEnter()
        {
            if (string.IsNullOrEmpty(m_Tooltip))
            {
                return;
            }

            Tooltip.Instance.Show(m_Tooltip, GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            if (string.IsNullOrEmpty(m_Tooltip))
            {
                return;
            }

            Tooltip.Instance.Hide();
        }
    }
}