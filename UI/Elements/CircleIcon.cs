using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CircleIcon : Interactable
    {
        [SerializeField] private Image image;

        private string tooltip;

        public void Construct(Sprite sprite, string tooltip = null)
        {
            this.image.sprite = sprite;
            this.tooltip = tooltip;
        }

        protected override void OnPointerEnter()
        {
            if (string.IsNullOrEmpty(this.tooltip))
            {
                return;
            }

            Tooltip.Instance.Show(this.tooltip, GetComponent<RectTransform>());
        }

        protected override void OnPointerExit()
        {
            if (string.IsNullOrEmpty(this.tooltip))
            {
                return;
            }

            Tooltip.Instance.Hide();
        }
    }
}