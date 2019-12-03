using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopupItemReward : LevelupPopupReward
    {
        public Item Item { get; set; }

        [SerializeField] private Interactable hover;

        private void Start()
        {
            if (this.hover == null)
            {
                return;
            }

            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;
        }

        private void OnPointerEnter()
        {
            if (Item == null)
            {
                return;
            }

            ItemTooltip.Instance.Show(Item, GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            if (Item == null)
            {
                return;
            }

            ItemTooltip.Instance.Hide();
        }
    }
}