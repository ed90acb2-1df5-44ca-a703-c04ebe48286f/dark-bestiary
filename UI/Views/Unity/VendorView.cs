using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VendorView : View, IVendorView
    {
        public event Payload SellJunk;
        public event Payload<Item> SellingItem;
        public event Payload<Item> BuyingItem;

        [SerializeField] private VendorPanel vendorPanel;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable sellJunkButton;

        private Item buyingItem;
        private InventoryPanel inventoryPanel;

        public void Construct(InventoryPanel inventoryPanel, List<VendorPanel.Category> categories)
        {
            this.inventoryPanel = inventoryPanel;

            this.vendorPanel.Construct(categories);

            this.vendorPanel.ItemRightClicked += OnVendorItemRightClicked;
            this.vendorPanel.ItemClicked += OnVendorItemClicked;
            this.vendorPanel.ItemDroppedIn += OnItemDroppedIn;

            this.closeButton.PointerClick += Hide;
            this.sellJunkButton.PointerClick += OnSellJunkButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerClick -= Hide;
            this.sellJunkButton.PointerClick -= OnSellJunkButtonPointerClick;
        }

        private void OnEnable()
        {
            if (this.inventoryPanel == null)
            {
                return;
            }

            this.inventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;
        }

        private void OnDisable()
        {
            if (this.inventoryPanel == null)
            {
                return;
            }

            this.inventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
        }

        public void RefreshAssortment(List<Item> assortment)
        {
            this.vendorPanel.RefreshAssortment(assortment);
        }

        public void MarkExpensive(Item item)
        {
            this.vendorPanel.MarkExpensive(item);
        }

        public void MarkAffordable(Item item)
        {
            this.vendorPanel.MarkAffordable(item);
        }

        private void OnSellJunkButtonPointerClick()
        {
            SellJunk?.Invoke();
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            TriggerSelling(inventoryItem.Item);
        }

        private void OnItemDroppedIn(Item item)
        {
            TriggerSelling(item);
        }

        private void OnVendorItemClicked(Item item)
        {
            Dialog.Instance.Clear()
                .AddText(I18N.Instance.Get("ui_buy") + $" {item.ColoredName}?")
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_yes"), () => TriggerBuying(item))
                .AddOption(I18N.Instance.Get("ui_no"))
                .Show();
        }

        private void OnVendorItemRightClicked(Item item)
        {
            TriggerBuying(item);
        }

        private void TriggerSelling(Item item)
        {
            SellingItem?.Invoke(item);
            AudioManager.Instance.PlayItemSell();
        }

        private void TriggerBuying(Item item)
        {
            BuyingItem?.Invoke(item);
            AudioManager.Instance.PlayItemBuy();
        }
    }
}