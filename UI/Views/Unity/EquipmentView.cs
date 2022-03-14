using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class EquipmentView : View, IEquipmentView
    {
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private Button closeButton;

        private Character character;
        private InventoryComponent inventory;
        private EquipmentComponent equipment;
        private Item itemToDelete;

        public void Construct(Character character)
        {
            this.character = character;
            this.equipment = this.character.Entity.GetComponent<EquipmentComponent>();
            this.inventory = this.character.Entity.GetComponent<InventoryComponent>();

            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;
            this.inventoryPanel.ItemDoubleClicked += OnInventoryItemRightClicked;

            this.closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnTerminate()
        {
            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
            this.inventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
            this.inventoryPanel.ItemDoubleClicked -= OnInventoryItemRightClicked;

            this.closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        public EquipmentPanel GetEquipmentPanel()
        {
            return this.equipmentPanel;
        }

        public InventoryPanel GetInventoryPanel()
        {
            return this.inventoryPanel;
        }

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            if (this.inventory.MaybeUse(inventoryItem.Item))
            {
                return;
            }

            AudioManager.Instance.PlayItemPlace(inventoryItem.Item);
            this.equipment.Equip(inventoryItem.Item);
        }

        private void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}