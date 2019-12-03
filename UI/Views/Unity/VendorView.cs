using System.Collections.Generic;
using DarkBestiary.Components;
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

        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private VendorPanel vendorPanel;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable sellJunkButton;

        private Item buyingItem;
        private Character character;
        private InventoryComponent inventory;
        private EquipmentComponent equipment;

        public void Construct(Character character)
        {
            this.character = character;
            this.inventory = character.Entity.GetComponent<InventoryComponent>();
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();

            this.vendorPanel.Construct();
        }

        protected override void OnInitialize()
        {
            this.vendorPanel.ItemRightClicked += OnVendorItemRightClicked;
            this.vendorPanel.ItemClicked += OnVendorItemClicked;
            this.vendorPanel.ItemDroppedIn += OnItemDroppedIn;

            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;

            this.closeButton.PointerUp += OnCloseButtonPointerUp;
            this.sellJunkButton.PointerUp += OnSellJunkButtonPointerUp;
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerUp -= OnCloseButtonPointerUp;
            this.sellJunkButton.PointerUp -= OnSellJunkButtonPointerUp;

            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
            this.inventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
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

        private void OnCloseButtonPointerUp()
        {
            Hide();
        }

        private void OnSellJunkButtonPointerUp()
        {
            SellJunk?.Invoke();
        }

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
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
            AudioManager.Instance.PlayItemSell();
            SellingItem?.Invoke(item);
        }

        private void TriggerBuying(Item item)
        {
            AudioManager.Instance.PlayItemBuy();
            BuyingItem?.Invoke(item);
        }
    }
}