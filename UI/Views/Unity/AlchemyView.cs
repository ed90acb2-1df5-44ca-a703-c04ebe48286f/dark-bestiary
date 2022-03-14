using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class AlchemyView : View, IAlchemyView
    {
        [SerializeField] private CraftViewRecipeRow recipeRowPrefab;
        [SerializeField] private CraftViewRecipePanel recipePanel;
        [SerializeField] private Transform recipeRowContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private TMP_InputField searchInput;

        private readonly List<CraftViewRecipeRow> recipeRows = new List<CraftViewRecipeRow>();

        private bool requiresUpdate;
        private MonoBehaviourPool<CraftViewRecipeRow> recipePool;
        private CraftViewRecipeRow selectedRecipe;
        private InventoryComponent characterInventory;
        private InventoryComponent ingredientInventory;
        private List<Recipe> recipes;

        public void Construct(List<Recipe> recipes, InventoryComponent characterInventory, InventoryComponent ingredientInventory)
        {
            this.recipes = recipes;
            this.characterInventory = characterInventory;
            this.ingredientInventory = ingredientInventory;

            if (this.recipes.Count == 0)
            {
                return;
            }

            this.recipePool = MonoBehaviourPool<CraftViewRecipeRow>.Factory(
                this.recipeRowPrefab, this.recipeRowContainer);

            this.ingredientInventory.ItemPicked += OnItemPicked;
            this.ingredientInventory.ItemRemoved += OnItemRemoved;
            this.ingredientInventory.ItemStackCountChanged += OnItemStackCountChanged;

            this.recipePanel.Initialize(this.ingredientInventory);
            this.recipePanel.CraftButtonClicked += OnCraftButtonClicked;

            this.searchInput.onValueChanged.AddListener(OnSearchInputChanged);
            this.closeButton.PointerClick += OnCloseButtonPointerClick;

            Refresh(this.recipes);
        }

        protected override void OnTerminate()
        {
            this.recipePool.Clear();

            this.ingredientInventory.ItemPicked -= OnItemPicked;
            this.ingredientInventory.ItemRemoved -= OnItemRemoved;
            this.ingredientInventory.ItemStackCountChanged -= OnItemStackCountChanged;

            this.recipePanel.Terminate();

            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }
        }

        private void OnSearchInputChanged(string search)
        {
            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.gameObject.SetActive(recipeRow.Recipe.Item.Name.LikeIgnoreCase($"%{search}%") ||
                                               recipeRow.Recipe.Item.Type.Name.LikeIgnoreCase($"%{search}%"));
            }
        }

        public void Refresh(List<Recipe> recipes)
        {
            this.selectedRecipe = null;

            DestroyRecipeRows();

            foreach (var recipe in recipes)
            {
                var recipeRow = this.recipePool.Spawn();
                recipeRow.Clicked += OnRecipeRowClicked;
                recipeRow.Construct(recipe, this.ingredientInventory);
                this.recipeRows.Add(recipeRow);
            }

            if (this.recipeRows.Count > 0)
            {
                OnRecipeRowClicked(this.recipeRows.First());
            }
        }

        private void DestroyRecipeRows()
        {
            this.recipePool.DespawnAll();

            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }

            this.recipeRows.Clear();
        }

        private void MarkRecipesForRefresh()
        {
            this.requiresUpdate = true;
        }

        private void OnItemStackCountChanged(ItemStackCountChangedEventData data)
        {
            MarkRecipesForRefresh();
        }

        private void OnItemPicked(ItemPickupEventData data)
        {
            MarkRecipesForRefresh();
        }

        private void OnItemRemoved(ItemRemovedEventData data)
        {
            MarkRecipesForRefresh();
        }

        private void OnCloseButtonPointerClick()
        {
            Hide();
        }

        private void OnRecipeRowClicked(CraftViewRecipeRow recipeRow)
        {
            if (this.selectedRecipe != null)
            {
                this.selectedRecipe.Deselect();
            }

            this.selectedRecipe = recipeRow;
            this.selectedRecipe.Select();

            this.recipePanel.Refresh(this.selectedRecipe.Recipe);
        }

        private void OnCraftButtonClicked(CraftViewRecipePanel recipePanel)
        {
            try
            {
                var item = recipePanel.Recipe.Item.Clone();

                this.ingredientInventory.WithdrawIngredients(recipePanel.Recipe);
                this.characterInventory.Pickup(item);

                AudioManager.Instance.PlayAlchemyBrew();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void Update()
        {
            if (!this.requiresUpdate)
            {
                return;
            }

            this.requiresUpdate = false;

            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.Refresh(recipeRow.Recipe);
            }

            this.recipePanel.Refresh(this.selectedRecipe.Recipe);
        }
    }
}