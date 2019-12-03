using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class StashView : View, IStashView
    {
        public event Payload<Item> Deposit;
        public event Payload<Item> Withdraw;

        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private InventoryPanel stashInventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private Interactable closeButton;

        private Character character;
        private EquipmentComponent equipment;
        private InventoryComponent inventory;
        private InventoryComponent stashInventory;

        public void Construct(Character character, InventoryComponent stashInventory)
        {
            this.character = character;
            this.stashInventory = stashInventory;
            this.inventory = character.Entity.GetComponent<InventoryComponent>();
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            this.closeButton.PointerUp += Hide;

            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.stashInventoryPanel.Initialize(this.stashInventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;
            this.stashInventoryPanel.ItemRightClicked += OnStashInventoryItemRightClicked;
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerUp -= Hide;

            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
            this.stashInventoryPanel.Terminate();
            this.inventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
            this.stashInventoryPanel.ItemRightClicked -= OnStashInventoryItemRightClicked;
        }

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            Deposit?.Invoke(inventoryItem.Item);
        }

        private void OnStashInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            Withdraw?.Invoke(inventoryItem.Item);
        }
    }
}