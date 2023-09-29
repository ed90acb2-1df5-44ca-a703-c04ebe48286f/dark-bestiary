using System;
using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VendorView : View, IVendorView
    {
        public event Action SellJunk;
        public event Action<Item> SellingItem;
        public event Action<Item> BuyingItem;

        [SerializeField] private VendorPanel m_VendorPanel;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_SellJunkButton;

        private Item m_BuyingItem;
        private InventoryPanel m_InventoryPanel;

        public void Construct(InventoryPanel inventoryPanel, List<VendorPanel.Category> categories)
        {
            m_InventoryPanel = inventoryPanel;

            m_VendorPanel.Construct(categories);

            m_VendorPanel.ItemRightClicked += OnVendorItemRightClicked;
            m_VendorPanel.ItemClicked += OnVendorItemClicked;
            m_VendorPanel.ItemDroppedIn += OnItemDroppedIn;

            m_CloseButton.PointerClick += Hide;
            m_SellJunkButton.PointerClick += OnSellJunkButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            m_CloseButton.PointerClick -= Hide;
            m_SellJunkButton.PointerClick -= OnSellJunkButtonPointerClick;
        }

        private void OnEnable()
        {
            if (m_InventoryPanel == null)
            {
                return;
            }

            m_InventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;
        }

        private void OnDisable()
        {
            if (m_InventoryPanel == null)
            {
                return;
            }

            m_InventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
        }

        public void RefreshAssortment(List<Item> assortment)
        {
            m_VendorPanel.RefreshAssortment(assortment);
        }

        public void MarkExpensive(Item item)
        {
            m_VendorPanel.MarkExpensive(item);
        }

        public void MarkAffordable(Item item)
        {
            m_VendorPanel.MarkAffordable(item);
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