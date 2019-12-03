using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
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
        public event Payload<ItemListRow> RightClicked;
        public event Payload<ItemListRow> Clicked;

        public Item Item { get; private set; }

        [SerializeField] private Image outline;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemStackCountText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image priceIcon;
        [SerializeField] private Transform priceContainer;

        private RectTransform tooltipTransform;

        public void Construct(Item item)
        {
            Item = item;

            this.tooltipTransform = GetComponent<RectTransform>();
            this.itemIcon.sprite = Resources.Load<Sprite>(item.Icon);
            this.itemStackCountText.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
            this.itemNameText.text = item.ColoredName;

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
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OverwriteStackCount(int stack)
        {
            this.itemStackCountText.text = stack.ToString();
        }

        private void ShowPrice(Currency price)
        {
            this.priceText.text = price.Amount.ToString();
            this.priceIcon.sprite = Resources.Load<Sprite>(price.Icon);
        }

        public void HidePrice()
        {
            this.priceContainer.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            ItemTooltip.Instance.Show(Item, this.tooltipTransform);
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
            this.priceText.color = Color.red;
        }

        public void MarkEnabled()
        {
            this.priceText.color = Color.white;
        }
    }
}