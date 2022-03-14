using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Items.Transmutation.Recipes;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class TransmutationView : View, ITransmutationView
    {
        public event Payload<Item> AddItem;
        public event Payload<Item, int> AddItemIndex;
        public event Payload<Item, Item> SwapItems;
        public event Payload<Item> RemoveItem;
        public event Payload Combine;

        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable combineButton;
        [SerializeField] private InventoryItemSlot slotPrefab;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private Interactable openRecipesButton;
        [SerializeField] private Interactable closeRecipesButton;
        [SerializeField] private GameObject recipesPanel;
        [SerializeField] private AlchemyRecipe alchemyRecipePrefab;
        [SerializeField] private Transform alchemyRecipeContainer;
        [SerializeField] private Sprite crossIcon;
        [SerializeField] private Sprite unknownIcon;
        [SerializeField] private Image recipeIcon;
        [SerializeField] private TextMeshProUGUI recipeText;
        [SerializeField] private GameObject particlesPrefab;

        private readonly List<InventoryItemSlot> slots = new List<InventoryItemSlot>();

        private InventoryPanel inventoryPanel;

        public void Construct(InventoryPanel inventoryPanel, List<Item> items, List<ITransmutationRecipe> recipes)
        {
            this.inventoryPanel = inventoryPanel;

            foreach (var item in items)
            {
                var slot = Instantiate(this.slotPrefab, this.slotContainer);
                slot.ChangeItem(item);
                slot.ItemDroppedIn += OnItemDroppedIn;
                slot.ItemDroppedOut += OnItemDroppedOut;
                slot.InventoryItem.RightClicked += OnSlotItemRightClicked;
                this.slots.Add(slot);
            }

            foreach (var recipe in recipes)
            {
                var recipeView = Instantiate(this.alchemyRecipePrefab, this.alchemyRecipeContainer);
                recipeView.Construct(recipe);
            }

            this.openRecipesButton.PointerClick += OnOpenRecipesButtonPointerClick;
            this.closeRecipesButton.PointerClick += OnCloseRecipesButtonPointerClick;
            this.combineButton.PointerClick += OnCombineButtonPointerClick;
            this.closeButton.PointerClick += Hide;

            ClearResult();
        }

        protected override void OnTerminate()
        {
            this.openRecipesButton.PointerClick -= OnOpenRecipesButtonPointerClick;
            this.closeRecipesButton.PointerClick -= OnCloseRecipesButtonPointerClick;
            this.combineButton.PointerClick -= OnCombineButtonPointerClick;
            this.closeButton.PointerClick -= Hide;
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

        private void OnOpenRecipesButtonPointerClick()
        {
            this.recipesPanel.gameObject.SetActive(true);
        }

        private void OnCloseRecipesButtonPointerClick()
        {
            this.recipesPanel.gameObject.SetActive(false);
        }

        public void OnSuccess()
        {
            AudioManager.Instance.PlayAlchemyCombine();
            Instantiate(this.particlesPrefab, this.slotContainer.parent).DestroyAsVisualEffect();
        }

        public void Block(Item item)
        {
            this.inventoryPanel.Block(item);
        }

        public void Unblock(Item item)
        {
            this.inventoryPanel.Unblock(item);
        }

        public void ShowResult(Recipe recipe)
        {
            this.recipeIcon.color = this.recipeIcon.color.With(a: 1);
            this.recipeIcon.sprite = Resources.Load<Sprite>(recipe.Item.Icon);
            this.recipeText.text = $"<size=125%><smallcaps>{recipe.Item.Name}</smallcaps></size>\n{recipe.Item.ConsumeDescription}";
        }

        public void ShowResult(string name, string description)
        {
            this.recipeIcon.color = this.recipeIcon.color.With(a: 1);
            this.recipeIcon.sprite = this.unknownIcon;
            this.recipeText.text = $"<size=125%><smallcaps>{name}</smallcaps></size>\n{description}";
        }

        public void ShowImpossibleCombination()
        {
            this.recipeIcon.color = this.recipeIcon.color.With(a: 1);
            this.recipeIcon.sprite = this.crossIcon;
            this.recipeText.text = I18N.Instance.Get("ui_impossible_combination");
        }

        public void ClearResult()
        {
            this.recipeIcon.color = this.recipeIcon.color.With(a: 0);
            this.recipeText.text = "";
        }

        public void RefreshItems(List<Item> items)
        {
            var index = 0;

            foreach (var item in items)
            {
                this.slots[index].ChangeItem(item);
                index++;
            }
        }

        private void OnCombineButtonPointerClick()
        {
            Combine?.Invoke();
        }

        private void OnSlotItemRightClicked(InventoryItem inventoryItem)
        {
            RemoveItem?.Invoke(inventoryItem.Item);
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            AddItem?.Invoke(inventoryItem.Item);
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            var containing = this.slots.FirstOrDefault(s => s.InventoryItem.Item == data.InventoryItem.Item);

            if (containing == null)
            {
                AddItemIndex?.Invoke(data.InventoryItem.Item, this.slots.IndexOf(data.InventorySlot));
                return;
            }

            SwapItems?.Invoke(containing.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
        }

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            if (this.slots.Any(s => s == data.InventorySlot))
            {
                return;
            }

            // Note: to prevent Empty item dropping into inventory slot
            Timer.Instance.WaitForFixedUpdate(() => RemoveItem?.Invoke(data.InventoryItem.Item));
        }
    }
}