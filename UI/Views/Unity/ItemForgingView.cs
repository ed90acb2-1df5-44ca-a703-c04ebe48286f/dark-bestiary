using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class ItemForgingView : View, IItemForgingView, IHideOnEscape
    {
        public event Payload<Item> ItemPlaced;
        public event Payload<Item> ItemRemoved;
        public event Payload UpgradeButtonClicked;

        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image buttonIcon;
        [SerializeField] private InventoryItemSlot itemSlot;
        [SerializeField] private ItemForgingRow rowPrefab;
        [SerializeField] private Transform rowContainer;
        [SerializeField] private CraftViewIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientContainer;
        [SerializeField] private Interactable upgradeButton;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private ForgingStar[] stars;

        private InventoryPanel inventoryPanel;
        private MonoBehaviourPool<CraftViewIngredient> ingredientPool;
        private MonoBehaviourPool<ItemForgingRow> rowPool;
        private InventoryComponent characterInventory;
        private InventoryComponent ingredientInventory;

        public void Construct(InventoryPanel inventoryPanel, InventoryComponent characterInventory, InventoryComponent ingredientInventory)
        {
            this.inventoryPanel = inventoryPanel;
            this.characterInventory = characterInventory;
            this.ingredientInventory = ingredientInventory;

            this.ingredientPool = MonoBehaviourPool<CraftViewIngredient>.Factory(this.ingredientPrefab, this.ingredientContainer);
            this.rowPool = MonoBehaviourPool<ItemForgingRow>.Factory(this.rowPrefab, this.rowContainer);

            this.itemSlot.ChangeItem(this.characterInventory.CreateEmptyItem());
            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut += OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked += OnItemRightClicked;
            this.itemSlot.InventoryItem.IsDraggable = false;

            this.upgradeButton.PointerClick += OnUpgradeButtonClicked;
            this.closeButton.PointerClick += Hide;
        }

        protected override void OnTerminate()
        {
            this.ingredientPool.Clear();
            this.rowPool.Clear();
        }

        protected override void OnHidden()
        {
            OnItemRightClicked(this.itemSlot.InventoryItem);
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

        public void Refresh(Item current, Item upgraded, List<RecipeIngredient> ingredients)
        {
            Cleanup();

            this.itemName.text = current.ColoredName;
            this.itemSlot.InventoryItem.Change(current);

            for (var i = 0; i < Item.MaxForgeLevel; i++)
            {
                if (current.ForgeLevel >= i + 1)
                {
                    this.stars[i].Activate();
                }
                else
                {
                    this.stars[i].Deactivate();
                }
            }

            CreateStats(current, upgraded);
            CreateIngredients(ingredients);
        }

        public void RefreshCost(Currency cost)
        {
            if (cost == null)
            {
                this.buttonIcon.gameObject.SetActive(false);
                this.buttonText.text = I18N.Instance.Translate("ui_upgrade");
                this.buttonText.margin = Vector4.zero;
            }
            else
            {
                this.buttonIcon.gameObject.SetActive(true);
                this.buttonIcon.sprite = Resources.Load<Sprite>(cost.Icon);
                this.buttonText.text = cost.Amount.ToString();
                this.buttonText.margin = new Vector4(42, 0, 0, 0);
            }
        }

        public void Cleanup()
        {
            for (var i = 0; i < Item.MaxForgeLevel; i++)
            {
                this.stars[i].Deactivate();
            }

            this.itemName.text = "";
            this.itemSlot.InventoryItem.Change(this.characterInventory.CreateEmptyItem());

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
                        $"{Mathf.Ceil(modifier.GetAmount()):F0}",
                        modifier.Attribute.Name + (difference.Value > 0 ? $" <color=green>(+{difference.Value:F0})</color>" : ""));
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
                this.ingredientPool.Spawn().Construct(ingredient, this.ingredientInventory);
            }
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            ItemPlaced?.Invoke(inventoryItem.Item);
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