using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class ItemUpgradeView : View, IItemUpgradeView, IHideOnEscape
    {
        public event Payload<Item> ItemPlaced;
        public event Payload<Item> ItemRemoved;
        public event Payload UpgradeButtonClicked;

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image buttonIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private InventoryItemSlot itemSlot;
        [SerializeField] private CraftViewIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientContainer;
        [SerializeField] private Interactable upgradeButton;
        [SerializeField] private Interactable closeButton;

        private readonly List<CraftViewIngredient> ingredientViews = new List<CraftViewIngredient>();

        private bool requiresUpdate;
        private Item item;
        private InventoryPanel inventoryPanel;
        private InventoryComponent characterInventory;
        private InventoryComponent ingredientInventory;
        private List<RecipeIngredient> ingredients;

        public void Construct(InventoryPanel inventoryPanel, InventoryComponent characterInventory, InventoryComponent ingredientInventory)
        {
            this.inventoryPanel = inventoryPanel;
            this.characterInventory = characterInventory;
            this.ingredientInventory = ingredientInventory;

            this.itemSlot.ChangeItem(this.characterInventory.CreateEmptyItem());

            this.inventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;

            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut += OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked += OnItemRightClicked;

            this.upgradeButton.PointerClick += OnUpgradeButtonClicked;
            this.closeButton.PointerClick += Hide;

            this.buttonIcon.gameObject.SetActive(false);
            this.buttonText.text = I18N.Instance.Translate("ui_apply");
        }

        protected override void OnTerminate()
        {
            this.inventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
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

        public void ChangeTitle(string title)
        {
            this.title.text = title;
        }

        public void Refresh(Item item, List<RecipeIngredient> ingredients)
        {
            this.item = item;
            this.ingredients = ingredients;
            this.requiresUpdate = true;
        }

        public void RefreshCost(Currency cost)
        {
            if (cost == null)
            {
                this.buttonIcon.gameObject.SetActive(false);
                this.buttonText.text = I18N.Instance.Translate("ui_apply");
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
            this.itemName.text = "";
            this.itemSlot.InventoryItem.Change(this.characterInventory.CreateEmptyItem());

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
                ingredientView.Construct(ingredient, this.ingredientInventory);
                this.ingredientViews.Add(ingredientView);
            }
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            ItemPlaced?.Invoke(inventoryItem.Item);
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