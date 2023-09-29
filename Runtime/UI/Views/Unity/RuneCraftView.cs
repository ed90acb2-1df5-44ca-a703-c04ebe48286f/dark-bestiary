using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class RuneCraftView : View, IRuneCraftView
    {
        [SerializeField] private CraftViewRecipeRow m_RecipeRowPrefab;
        [SerializeField] private CraftViewRecipePanel m_RecipePanel;
        [SerializeField] private Transform m_RecipeRowContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private TMP_InputField m_SearchInput;
        [SerializeField] private Interactable m_ContinueButton;
        [SerializeField] private ItemTooltip m_ResultTooltip;

        private readonly List<CraftViewRecipeRow> m_RecipeRows = new();

        private bool m_RequiresUpdate;
        private MonoBehaviourPool<CraftViewRecipeRow> m_RecipeRowPool;
        private CraftViewRecipeRow m_SelectedRecipe;
        private InventoryComponent m_CharacterInventory;
        private InventoryComponent m_IngredientInventory;
        private List<Recipe> m_Recipes;

        public void Construct(List<Recipe> recipes, InventoryComponent characterInventory, InventoryComponent ingredientInventory)
        {
            m_Recipes = recipes;
            m_CharacterInventory = characterInventory;
            m_IngredientInventory = ingredientInventory;

            if (m_Recipes.Count == 0)
            {
                return;
            }

            m_RecipeRowPool = MonoBehaviourPool<CraftViewRecipeRow>.Factory(m_RecipeRowPrefab, m_RecipeRowContainer);

            m_ResultTooltip.Initialize();

            m_IngredientInventory.ItemPicked += OnItemPicked;
            m_IngredientInventory.ItemRemoved += OnItemRemoved;
            m_IngredientInventory.ItemStackCountChanged += OnItemStackCountChanged;

            m_RecipePanel.Initialize(m_IngredientInventory);
            m_RecipePanel.CraftButtonClicked += OnCraftButtonClicked;

            m_SearchInput.onValueChanged.AddListener(OnSearchInputChanged);
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;

            m_ContinueButton.PointerClick += OnContinueButtonPointerClick;

            Refresh(m_Recipes);
        }

        protected override void OnTerminate()
        {
            m_RecipeRowPool.Clear();

            m_ResultTooltip.Terminate();

            m_IngredientInventory.ItemPicked -= OnItemPicked;
            m_IngredientInventory.ItemRemoved -= OnItemRemoved;
            m_IngredientInventory.ItemStackCountChanged -= OnItemStackCountChanged;

            m_RecipePanel.Terminate();

            foreach (var recipeRow in m_RecipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }
        }

        protected override void OnHidden()
        {
            OnContinueButtonPointerClick();
        }

        private void OnSearchInputChanged(string search)
        {
            foreach (var recipeRow in m_RecipeRows)
            {
                recipeRow.gameObject.SetActive(recipeRow.Recipe.Item.Name.Contains(search) ||
                                               recipeRow.Recipe.Item.PassiveDescription.Contains(search));
            }
        }

        private void OnContinueButtonPointerClick()
        {
            m_ResultTooltip.Hide();
        }

        public void Refresh(List<Recipe> recipes)
        {
            m_SelectedRecipe = null;

            DestroyRecipeRows();

            foreach (var recipe in recipes)
            {
                var recipeRow = m_RecipeRowPool.Spawn();
                recipeRow.Clicked += OnRecipeRowClicked;
                recipeRow.Construct(recipe, m_IngredientInventory);
                m_RecipeRows.Add(recipeRow);
            }

            if (m_RecipeRows.Count > 0)
            {
                OnRecipeRowClicked(m_RecipeRows.First());
            }
        }

        private void DestroyRecipeRows()
        {
            m_RecipeRowPool.DespawnAll();

            foreach (var recipeRow in m_RecipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }

            m_RecipeRows.Clear();
        }

        private void MarkRecipesForRefresh()
        {
            m_RequiresUpdate = true;
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
            if (m_SelectedRecipe != null)
            {
                m_SelectedRecipe.Deselect();
            }

            m_SelectedRecipe = recipeRow;
            m_SelectedRecipe.Select();

            m_RecipePanel.Refresh(m_SelectedRecipe.Recipe);
        }

        private void OnCraftButtonClicked(CraftViewRecipePanel recipePanel)
        {
            try
            {
                var item = recipePanel.Recipe.Item.Clone();

                m_IngredientInventory.WithdrawIngredients(recipePanel.Recipe);
                m_CharacterInventory.Pickup(item);

                AudioManager.Instance.PlayCraft();

                if (item.IsGem || item.IsIngredient)
                {
                    return;
                }

                item.RollSuffix();
                item.RollSockets();

                m_ResultTooltip.Show(item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void Update()
        {
            if (!m_RequiresUpdate)
            {
                return;
            }

            m_RequiresUpdate = false;

            foreach (var recipeRow in m_RecipeRows)
            {
                recipeRow.Refresh(recipeRow.Recipe);
            }

            m_RecipePanel.Refresh(m_SelectedRecipe.Recipe);
        }
    }
}