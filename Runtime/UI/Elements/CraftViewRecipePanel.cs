using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CraftViewRecipePanel : MonoBehaviour
    {
        public event Action<CraftViewRecipePanel> CraftButtonClicked;

        [SerializeField] private InventoryItem m_InventoryItemPrefab;
        [SerializeField] private Transform m_InventoryItemContainer;
        [SerializeField] private CraftViewIngredient m_IngredientPrefab;
        [SerializeField] private Transform m_IngredientRowContainer;
        [SerializeField] private Interactable m_CraftButton;
        [SerializeField] private TextMeshProUGUI m_ItemNameText;

        public Recipe Recipe { get; private set; }

        private readonly List<CraftViewIngredient> m_IngredientRows = new();
        private InventoryComponent m_Inventory;
        private InventoryItem m_InventoryItem;

        public void Initialize(InventoryComponent inventory)
        {
            m_Inventory = inventory;

            m_InventoryItem = Instantiate(m_InventoryItemPrefab, m_InventoryItemContainer);
            m_InventoryItem.IsDraggable = false;
            m_InventoryItem.Change(m_Inventory.CreateEmptyItem());

            m_CraftButton.PointerClick += OnCraftButtonPointerClick;
        }

        public void Terminate()
        {
            m_CraftButton.PointerClick -= OnCraftButtonPointerClick;
        }

        public void Refresh(Recipe recipe)
        {
            Recipe = recipe;

            m_InventoryItem.Change(recipe.Item);

            m_ItemNameText.text = recipe.Item.ColoredName;

            foreach (var recipeRow in m_IngredientRows)
            {
                Destroy(recipeRow.gameObject);
            }

            m_IngredientRows.Clear();

            foreach (var ingredient in recipe.Ingredients)
            {
                var ingredientRow = Instantiate(m_IngredientPrefab, m_IngredientRowContainer);
                ingredientRow.Construct(ingredient, m_Inventory);

                m_IngredientRows.Add(ingredientRow);
            }
        }

        private void OnCraftButtonPointerClick()
        {
            CraftButtonClicked?.Invoke(this);
        }
    }
}