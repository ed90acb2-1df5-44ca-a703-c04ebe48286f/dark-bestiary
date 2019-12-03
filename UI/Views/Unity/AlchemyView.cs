using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Items.Alchemy;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class AlchemyView : View, IAlchemyView
    {
        public event Payload<Item> AddItem;
        public event Payload<Item, int> AddItemIndex;
        public event Payload<Item, Item> SwapItems;
        public event Payload<Item> RemoveItem;
        public event Payload Combine;

        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
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

        private Character character;
        private InventoryComponent inventory;
        private EquipmentComponent equipment;

        public void Construct(Character character, List<Item> items, List<IAlchemyRecipe> recipes)
        {
            this.character = character;
            this.inventory = character.Entity.GetComponent<InventoryComponent>();
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();

            foreach (var item in items)
            {
                var slot = Instantiate(this.slotPrefab, this.slotContainer);
                slot.Construct(item);
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

            ClearResult();
        }

        protected override void OnInitialize()
        {
            this.openRecipesButton.PointerUp += OnOpenRecipesButtonPointerUp;
            this.closeRecipesButton.PointerUp += OnCloseRecipesButtonPointerUp;
            this.combineButton.PointerUp += OnCombineButtonPointerUp;
            this.closeButton.PointerUp += Hide;

            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;
        }

        protected override void OnTerminate()
        {
            this.openRecipesButton.PointerUp -= OnOpenRecipesButtonPointerUp;
            this.closeRecipesButton.PointerUp -= OnCloseRecipesButtonPointerUp;
            this.combineButton.PointerUp -= OnCombineButtonPointerUp;
            this.closeButton.PointerUp -= Hide;

            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();
            this.inventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
        }

        private void OnOpenRecipesButtonPointerUp()
        {
            this.recipesPanel.gameObject.SetActive(true);
        }

        private void OnCloseRecipesButtonPointerUp()
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
            this.recipeText.text = $"<size=150%><smallcaps>{recipe.Item.Name}</smallcaps></size>\n{recipe.Item.ConsumeDescription}";
        }

        public void ShowUnknownResult()
        {
            this.recipeIcon.color = this.recipeIcon.color.With(a: 1);
            this.recipeIcon.sprite = this.unknownIcon;
            this.recipeText.text = I18N.Instance.Get("ui_unknown_result");
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

        private void OnCombineButtonPointerUp()
        {
            Combine?.Invoke();
        }

        private void OnSlotItemRightClicked(InventoryItem inventoryItem)
        {
            RemoveItem?.Invoke(inventoryItem.Item);
        }

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
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