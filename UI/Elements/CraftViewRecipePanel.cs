using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CraftViewRecipePanel : MonoBehaviour
    {
        public event Payload<CraftViewRecipePanel> CraftButtonClicked;

        [SerializeField] private InventoryItem inventoryItemPrefab;
        [SerializeField] private Transform inventoryItemContainer;
        [SerializeField] private CraftViewIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientRowContainer;
        [SerializeField] private Interactable craftButton;
        [SerializeField] private TextMeshProUGUI itemNameText;

        public Recipe Recipe { get; private set; }

        private readonly List<CraftViewIngredient> ingredientRows = new List<CraftViewIngredient>();
        private InventoryComponent inventory;
        private InventoryItem inventoryItem;

        public void Initialize(InventoryComponent inventory)
        {
            this.inventory = inventory;

            this.inventoryItem = Instantiate(this.inventoryItemPrefab, this.inventoryItemContainer);
            this.inventoryItem.IsDraggable = false;
            this.inventoryItem.Change(this.inventory.CreateEmptyItem());

            this.craftButton.PointerUp += OnCraftButtonPointerUp;
        }

        public void Terminate()
        {
            this.craftButton.PointerUp -= OnCraftButtonPointerUp;
        }

        public void Refresh(Recipe recipe)
        {
            Recipe = recipe;

            this.inventoryItem.Change(recipe.Item);

            this.itemNameText.text = recipe.Item.ColoredName;

            foreach (var recipeRow in this.ingredientRows)
            {
                Destroy(recipeRow.gameObject);
            }

            this.ingredientRows.Clear();

            foreach (var ingredient in recipe.Ingredients)
            {
                var ingredientRow = Instantiate(this.ingredientPrefab, this.ingredientRowContainer);
                ingredientRow.Construct(ingredient, this.inventory);

                this.ingredientRows.Add(ingredientRow);
            }
        }

        private void OnCraftButtonPointerUp()
        {
            CraftButtonClicked?.Invoke(this);
        }
    }
}