using System;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemListRow : PoolableMonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        public event Action<ItemListRow> RightClicked;
        public event Action<ItemListRow> Clicked;

        public Item Item { get; private set; }

        [SerializeField] private Image m_Outline;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private TextMeshProUGUI m_ItemStackCountText;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private Image m_PriceIcon;
        [SerializeField] private Transform m_PriceContainer;

        private RectTransform m_TooltipTransform;

        public void Construct(Item item)
        {
            Item = item;

            m_TooltipTransform = GetComponent<RectTransform>();
            m_ItemIcon.sprite = Resources.Load<Sprite>(item.Icon);
            m_ItemStackCountText.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
            m_ItemNameText.text = item.ColoredName;

            var price = item.GetPrice().FirstOrDefault();

            if (price == null)
            {
                HidePrice();
            }
            else
            {
                ShowPrice(price);
            }

            Deselect();
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void OverwriteStackCount(int stack)
        {
            m_ItemStackCountText.text = stack.ToString();
        }

        private void ShowPrice(Currency price)
        {
            m_PriceText.text = price.Amount.ToString();
            m_PriceIcon.sprite = Resources.Load<Sprite>(price.Icon);
        }

        public void HidePrice()
        {
            m_PriceContainer.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            ItemTooltip.Instance.Show(Item, m_TooltipTransform);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (pointer.button == PointerEventData.InputButton.Right)
            {
                RightClicked?.Invoke(this);
                return;
            }

            if (pointer.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Clicked?.Invoke(this);
        }

        public void MarkDisabled()
        {
            m_PriceText.color = Color.red;
        }

        public void MarkEnabled()
        {
            m_PriceText.color = Color.white;
        }
    }
}