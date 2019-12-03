using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class DismantlingView : View, IDismantlingView
    {
        public event Payload OkayButtonClicked;
        public event Payload DismantleButtonClicked;
        public event Payload ClearButtonClicked;
        public event Payload<Item> ItemPlacing;
        public event Payload<Item> ItemRemoving;

        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private Button okayButton;
        [SerializeField] private Button dismantleButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private ItemListRow itemRowPrefab;
        [SerializeField] private Transform itemRowContainer;
        [SerializeField] private ItemDropArea itemDropArea;

        private Character character;
        private InventoryComponent inventory;
        private EquipmentComponent equipment;

        public void Construct(Character character)
        {
            this.character = character;
            this.inventory = character.Entity.GetComponent<InventoryComponent>();
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;

            this.itemDropArea.ItemDroppedIn += OnItemDroppedIn;
            this.okayButton.onClick.AddListener(() => OkayButtonClicked?.Invoke());
            this.dismantleButton.onClick.AddListener(() => DismantleButtonClicked?.Invoke());
            this.clearButton.onClick.AddListener(() => ClearButtonClicked?.Invoke());
            this.closeButton.onClick.AddListener(Hide);
        }

        protected override void OnTerminate()
        {
            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
        }

        public void DisplayDismantlingItems(IEnumerable<Item> items)
        {
            this.okayButton.gameObject.SetActive(false);
            this.dismantleButton.gameObject.SetActive(true);
            this.clearButton.gameObject.SetActive(true);

            ClearItems();
            CreateDismantlingItems(items);
        }

        protected override void OnHidden()
        {
            ClearButtonClicked?.Invoke();
        }

        public void DisplayDismantlingResult(IEnumerable<Item> items)
        {
            this.okayButton.gameObject.SetActive(true);
            this.dismantleButton.gameObject.SetActive(false);
            this.clearButton.gameObject.SetActive(false);

            ClearItems();
            CreateDismantledItems(items);
        }

        private void CreateDismantledItems(IEnumerable<Item> items)
        {
            foreach (var group in items.GroupBy(item => item.Id))
            {
                var row = Instantiate(this.itemRowPrefab, this.itemRowContainer);
                row.Construct(group.First());
                row.OverwriteStackCount(group.Sum(item => item.StackCount));
                row.HidePrice();
            }
        }

        private void CreateDismantlingItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                var row = Instantiate(this.itemRowPrefab, this.itemRowContainer);
                row.Clicked += OnRowClicked;
                row.Construct(item);
                row.HidePrice();

                this.inventoryPanel
                    .Slots
                    .FirstOrDefault(slot => slot.InventoryItem.Item.Equals(item))?
                    .InventoryItem
                    .Block();
            }
        }

        private void ClearItems()
        {
            foreach (var row in this.itemRowContainer.GetComponentsInChildren<ItemListRow>())
            {
                foreach (var slot in this.inventoryPanel.Slots)
                {
                    slot.InventoryItem.Unblock();
                }

                row.Clicked -= OnRowClicked;
                Destroy(row.gameObject);
            }
        }

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            ItemPlacing?.Invoke(inventoryItem.Item);
        }


        private void OnItemDroppedIn(Item item)
        {
            ItemPlacing?.Invoke(item);
        }

        private void OnRowClicked(ItemListRow row)
        {
            ItemRemoving?.Invoke(row.Item);
        }
    }
}