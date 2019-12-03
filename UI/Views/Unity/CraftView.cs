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
    public class CraftView : View, ICraftView
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

        private readonly List<CraftViewRecipeRow> recipeRows = new List<CraftViewRecipeRow>();

        private bool requiresUpdate;
        private MonoBehaviourPool<CraftViewRecipeRow> recipePool;
        private CraftViewRecipeRow selectedRecipe;
        private ItemCategoryTab activeTab;
        private InventoryComponent inventory;
        private List<Recipe> recipes;

        public void Construct(List<Recipe> recipes, InventoryComponent inventory)
        {
            this.recipes = recipes;
            this.inventory = inventory;
        }

        protected override void OnInitialize()
        {
            if (this.recipes.Count == 0)
            {
                return;
            }

            this.recipePool = MonoBehaviourPool<CraftViewRecipeRow>.Factory(
                this.recipeRowPrefab, this.recipeRowContainer);

            this.resultTooltip.Initialize();

            this.inventory.ItemPicked += OnItemPicked;
            this.inventory.ItemRemoved += OnItemRemoved;
            this.inventory.ItemStackCountChanged += OnItemStackCountChanged;

            this.recipePanel.Initialize(this.inventory);
            this.recipePanel.CraftButtonClicked += OnCraftButtonClicked;

            this.searchInput.onValueChanged.AddListener(OnSearchInputChanged);
            this.closeButton.PointerUp += OnCloseButtonPointerUp;

            this.continueButton.PointerUp += OnContinueButtonPointerUp;

            CreateCategoryTabs();
            RefreshRecipes(this.recipes);
        }

        protected override void OnTerminate()
        {
            this.recipePool.Clear();

            this.resultTooltip.Terminate();

            this.inventory.ItemPicked -= OnItemPicked;
            this.inventory.ItemRemoved -= OnItemRemoved;
            this.inventory.ItemStackCountChanged -= OnItemStackCountChanged;

            this.recipePanel.Terminate();

            foreach (var recipeRow in this.recipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }
        }

        protected override void OnHidden()
        {
            OnContinueButtonPointerUp();
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
                ItemCategory.Gems,
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

        private void OnContinueButtonPointerUp()
        {
            this.resultTooltip.Hide();
        }

        public void RefreshRecipes(List<Recipe> recipes)
        {
            this.selectedRecipe = null;

            DestroyRecipeRows();

            foreach (var recipe in recipes)
            {
                var recipeRow = this.recipePool.Spawn();
                recipeRow.Clicked += OnRecipeRowClicked;
                recipeRow.Construct(recipe, this.inventory);
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

        private void OnCloseButtonPointerUp()
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

                this.inventory.WithdrawIngredients(recipePanel.Recipe);
                this.inventory.Pickup(item);

                AudioManager.Instance.PlayCraft();

                if (item.IsGem || item.IsIngredient)
                {
                    return;
                }

                this.resultTooltip.Show(item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
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