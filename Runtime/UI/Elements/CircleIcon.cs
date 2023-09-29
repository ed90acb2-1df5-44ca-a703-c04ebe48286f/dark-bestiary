using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CircleIcon : Interactable
    {
        [SerializeField] private Image m_Image;

        private string m_Tooltip;

        public void Construct(Sprite sprite, string tooltip = null)
        {
            m_Image.sprite = sprite;
            m_Tooltip = tooltip;
        }

        protected override void OnPointerEnter()
        {
            if (string.IsNullOrEmpty(m_Tooltip))
            {
                return;
            }

            Tooltip.Instance.Show(m_Tooltip, GetComponent<RectTransform>());
        }

        protected override void OnPointerExit()
        {
            if (string.IsNullOrEmpty(m_Tooltip))
            {
                return;
            }

            Tooltip.Instance.Hide();
        }
    }
}