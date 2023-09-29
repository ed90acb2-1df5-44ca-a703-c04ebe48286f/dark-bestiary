using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopupItemReward : LevelupPopupReward
    {
        public Item Item { get; set; }

        [SerializeField] private Interactable m_Hover;

        private void Start()
        {
            if (m_Hover == null)
            {
                return;
            }

            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;
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