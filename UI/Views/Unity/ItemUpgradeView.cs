using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class ItemUpgradeView : View, IItemUpgradeView, IHideOnEscape
    {
        public event Payload<Item> ItemPlaced;
        public event Payload<Item> ItemRemoved;
        public event Payload UpgradeButtonClicked;

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private InventoryItemSlot itemSlot;
        [SerializeField] private CraftViewIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientContainer;
        [SerializeField] private Interactable upgradeButton;
        [SerializeField] private Interactable closeButton;

        private readonly List<CraftViewIngredient> ingredientViews = new List<CraftViewIngredient>();

        private bool requiresUpdate;
        private Item item;
        private Character character;
        private EquipmentComponent equipment;
        private InventoryComponent inventory;
        private List<RecipeIngredient> ingredients;

        public void Construct(Character character)
        {
            this.character = character;
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();
            this.inventory = character.Entity.GetComponent<InventoryComponent>();

            this.itemSlot.Construct(this.inventory.CreateEmptyItem());
        }

        protected override void OnInitialize()
        {
            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;

            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut += OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked += OnItemRightClicked;

            this.upgradeButton.PointerUp += OnUpgradeButtonClicked;
            this.closeButton.PointerUp += Hide;
        }

        protected override void OnTerminate()
        {
            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
            this.inventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
        }

        protected override void OnHidden()
        {
            OnItemRightClicked(this.itemSlot.InventoryItem);
        }

        public void ChanceTitle(string title)
        {
            this.title.text = title;
        }

        public void Refresh(Item item, List<RecipeIngredient> ingredients)
        {
            this.item = item;
            this.ingredients = ingredients;
            this.requiresUpdate = true;
        }

        public void Cleanup()
        {
            this.itemName.text = "";
            this.itemSlot.InventoryItem.Change(this.inventory.CreateEmptyItem());

            foreach (var element in this.ingredientContainer.GetComponentsInChildren<InventoryItem>())
            {
                Destroy(element.gameObject);
            }
        }

        private void CreateIngredients(List<RecipeIngredient> ingredients)
        {
            foreach (var ingredientView in this.ingredientViews)
            {
                Destroy(ingredientView.gameObject);
            }

            this.ingredientViews.Clear();

            foreach (var ingredient in ingredients)
            {
                var ingredientView = Instantiate(this.ingredientPrefab, this.ingredientContainer);
                ingredientView.Construct(ingredient, this.inventory);
                this.ingredientViews.Add(ingredientView);
            }
        }

        private void OnInventoryItemRightClicked(InventoryItem item)
        {
            ItemPlaced?.Invoke(item.Item);
        }

        private void OnItemRightClicked(InventoryItem item)
        {
            if (item.Item.IsEmpty)
            {
                return;
            }

            ItemRemoved?.Invoke(item.Item);
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            ItemPlaced?.Invoke(data.InventoryItem.Item);
        }

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            ItemRemoved?.Invoke(data.InventoryItem.Item);
        }

        private void OnUpgradeButtonClicked()
        {
            UpgradeButtonClicked?.Invoke();
        }

        private void Update()
        {
            if (!this.requiresUpdate)
            {
                return;
            }

            this.requiresUpdate = false;

            Cleanup();

            this.itemName.text = this.item.IsEmpty ? "" : this.item.ColoredName;
            this.itemSlot.InventoryItem.Change(this.item);

            CreateIngredients(this.ingredients);
        }
    }
}