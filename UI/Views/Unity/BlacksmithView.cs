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
    public class BlacksmithView : View, IBlacksmithView
    {
        [SerializeField] private ItemCategoryTab tabPrefab;
        [SerializeField] private Transform tabContainer;
        [SerializeField] private CraftViewRecipeRow recipeRowPrefab;
        [SerializeField] private CraftViewRecipePanel recipePanel;
        [SerializeField] private Transform recipeRowContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Interactable continueButton;
        [SerializeField] private ItemTooltip resultTooltip;
        [SerializeField] private CraftViewRecipeRow[] precreatedRecipeRows;

        private readonly List<CraftViewRecipeRow> recipeRows = new List<CraftViewRecipeRow>();

        private bool requiresUpdate;
        private MonoBehaviourPool<CraftViewRecipeRow> recipeRowPool;
        private CraftViewRecipeRow selectedRecipe;
        private ItemCategoryTab activeTab;
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

            this.recipeRowPool = MonoBehaviourPool<CraftViewRecipeRow>.Factory(
                this.recipeRowPrefab, this.recipeRowContainer, this.precreatedRecipeRows);

            this.resultTooltip.Initialize();

            this.ingredientInventory.ItemPicked += OnItemPicked;
            this.ingredientInventory.ItemRemoved += OnItemRemoved;
            this.ingredientInventory.ItemStackCountChanged += OnItemStackCountChanged;

            this.recipePanel.Initialize(this.ingredientInventory);
            this.recipePanel.CraftButtonClicked += OnCraftButtonClicked;

            this.searchInput.onValueChanged.AddListener(OnSearchInputChanged);
            this.closeButton.PointerClick += OnCloseButtonPointerClick;

            this.continueButton.PointerClick += OnContinueButtonPointerClick;

            CreateCategoryTabs();
            Refresh(this.recipes);
        }

        protected override void OnTerminate()
        {
            this.recipeRowPool.Clear();

            this.resultTooltip.Terminate();

            this.ingredientInventory.ItemPicked -= OnItemPicked;
            this.ingredientInventory.ItemRemoved -= OnItemRemoved;
            this.ingredientInventory.ItemStackCountChanged -= OnItemStackCountChanged;

            this.recipePanel.Terminate();

            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }
        }

        protected override void OnHidden()
        {
            OnContinueButtonPointerClick();
        }

        private void CreateCategoryTabs()
        {
            var categories = new List<ItemCategory>
            {
                ItemCategory.All,
                ItemCategory.Armor,
                ItemCategory.Weapon,
                ItemCategory.Jewelry,
                ItemCategory.Ingredients,
            };

            foreach (var category in categories)
            {
                var tab = Instantiate(this.tabPrefab, this.tabContainer);
                tab.Clicked += OnTabClicked;
                tab.Construct(category);
            }

            OnTabClicked(this.tabContainer.GetComponentInChildren<ItemCategoryTab>());
        }

        private void OnTabClicked(ItemCategoryTab tab)
        {
            if (this.activeTab != null)
            {
                this.activeTab.Deselect();
            }

            this.activeTab = tab;
            this.activeTab.Select();

            OnSearchInputChanged(this.searchInput.text);
        }

        private void OnSearchInputChanged(string search)
        {
            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.gameObject.SetActive(this.activeTab.Category.Contains(recipeRow.Recipe.Item.Type) &&
                                               (recipeRow.Recipe.Item.Name.LikeIgnoreCase($"%{search}%") ||
                                                recipeRow.Recipe.Item.Type.Name.LikeIgnoreCase($"%{search}%")));
            }
        }

        private void OnContinueButtonPointerClick()
        {
            this.resultTooltip.Hide();
        }

        public void Refresh(List<Recipe> recipes)
        {
            this.selectedRecipe = null;

            DestroyRecipeRows();

            foreach (var recipe in recipes)
            {
                var recipeRow = this.recipeRowPool.Spawn();
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
            this.recipeRowPool.DespawnAll();

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

                AudioManager.Instance.PlayCraft();

                if (item.IsGem || item.IsIngredient)
                {
                    return;
                }

                item.RollSuffix();
                item.RollSockets();

                this.resultTooltip.Show(item);
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