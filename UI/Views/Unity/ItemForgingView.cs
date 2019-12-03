using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class ItemForgingView : View, IItemForgingView, IHideOnEscape
    {
        public event Payload<Item> ItemPlaced;
        public event Payload<Item> ItemRemoved;
        public event Payload UpgradeButtonClicked;

        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private InventoryItemSlot itemSlot;
        [SerializeField] private ItemForgingRow rowPrefab;
        [SerializeField] private Transform rowContainer;
        [SerializeField] private CraftViewIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientContainer;
        [SerializeField] private Interactable upgradeButton;
        [SerializeField] private Interactable closeButton;

        private MonoBehaviourPool<CraftViewIngredient> ingredientPool;
        private MonoBehaviourPool<ItemForgingRow> rowPool;
        private Character character;
        private EquipmentComponent equipment;
        private InventoryComponent inventory;

        public void Construct(Character character)
        {
            this.character = character;
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();
            this.inventory = character.Entity.GetComponent<InventoryComponent>();
        }

        protected override void OnInitialize()
        {
            this.ingredientPool = MonoBehaviourPool<CraftViewIngredient>.Factory(this.ingredientPrefab, this.ingredientContainer);
            this.rowPool = MonoBehaviourPool<ItemForgingRow>.Factory(this.rowPrefab, this.rowContainer);

            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;

            this.itemSlot.Construct(this.inventory.CreateEmptyItem());
            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut += OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked += OnItemRightClicked;

            this.upgradeButton.PointerUp += OnUpgradeButtonClicked;
            this.closeButton.PointerUp += Hide;
        }

        protected override void OnTerminate()
        {
            this.ingredientPool.Clear();
            this.rowPool.Clear();

            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
            this.inventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
        }

        protected override void OnHidden()
        {
            OnItemRightClicked(this.itemSlot.InventoryItem);
        }

        public void Refresh(Item current, Item upgraded, List<RecipeIngredient> ingredients)
        {
            Cleanup();

            this.itemName.text = current.ColoredName;
            this.itemSlot.InventoryItem.Change(current);

            CreateStats(current, upgraded);
            CreateIngredients(ingredients);
        }

        public void Cleanup()
        {
            this.itemName.text = "";
            this.itemSlot.InventoryItem.Change(this.inventory.CreateEmptyItem());

            this.ingredientPool.DespawnAll();
            this.rowPool.DespawnAll();
        }

        private void CreateStats(Item current, Item upgraded)
        {
            var attributeDifference = current.GetAttributeDifference(upgraded);
            var propertyDifference = current.GetPropertyDifference(upgraded);

            foreach (var modifier in current.GetAttributeModifiers(false).OrderBy(key => key.Attribute.Index))
            {
                var difference = attributeDifference.FirstOrDefault(pair => pair.Key.Type == modifier.Attribute.Type);

                this.rowPool.Spawn()
                    .Construct(
                        $"{modifier.GetAmount():F0}",
                        modifier.Attribute.Name + $" <color=green>(+{difference.Value:F0})</color>");
            }

            foreach (var modifier in current.GetPropertyModifiers(false).OrderBy(key => key.Property.Index))
            {
                var difference = propertyDifference.FirstOrDefault(pair => pair.Key.Type == modifier.Property.Type);

                this.rowPool.Spawn()
                    .Construct(
                        Property.ValueString(modifier.Property.Type, modifier.GetAmount()),
                        modifier.Property.Name + $" <color=green>(+{Property.ValueString(modifier.Property.Type, difference.Value)})</color>");
            }
        }

        private void CreateIngredients(List<RecipeIngredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                this.ingredientPool.Spawn().Construct(ingredient, this.inventory);
            }
        }

        private void OnInventoryItemRightClicked(InventoryItem item)
        {
            ItemPlaced?.Invoke(item.Item);
        }

        private void OnItemRightClicked(InventoryItem item)
        {
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
    }
}