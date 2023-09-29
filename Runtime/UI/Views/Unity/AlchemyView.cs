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
    public class AlchemyView : View, IAlchemyView
    {
        [SerializeField] private CraftViewRecipeRow m_RecipeRowPrefab;
        [SerializeField] private CraftViewRecipePanel m_RecipePanel;
        [SerializeField] private Transform m_RecipeRowContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private TMP_InputField m_SearchInput;

        private readonly List<CraftViewRecipeRow> m_RecipeRows = new();

        private bool m_RequiresUpdate;
        private MonoBehaviourPool<CraftViewRecipeRow> m_RecipePool;
        private CraftViewRecipeRow m_SelectedRecipe;
        private InventoryComponent m_CharacterInventory;
        private InventoryComponent m_IngredientInventory;
        private List<Recipe> m_Recipes;

        public void Construct(List<Recipe> recipes, InventoryComponent characterInventory, InventoryComponent ingredientInventory)
        {
            m_Recipes = recipes;
            m_CharacterInventory = characterInventory;
            m_IngredientInventory = ingredientInventory;

            m_RecipePool = MonoBehaviourPool<CraftViewRecipeRow>.Factory(
                m_RecipeRowPrefab, m_RecipeRowContainer);

            m_IngredientInventory.ItemPicked += OnItemPicked;
            m_IngredientInventory.ItemRemoved += OnItemRemoved;
            m_IngredientInventory.ItemStackCountChanged += OnItemStackCountChanged;

            m_RecipePanel.Initialize(m_IngredientInventory);
            m_RecipePanel.CraftButtonClicked += OnCraftButtonClicked;

            m_SearchInput.onValueChanged.AddListener(OnSearchInputChanged);
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;

            Refresh(m_Recipes);
        }

        protected override void OnTerminate()
        {
            m_RecipePool.Clear();

            m_IngredientInventory.ItemPicked -= OnItemPicked;
            m_IngredientInventory.ItemRemoved -= OnItemRemoved;
            m_IngredientInventory.ItemStackCountChanged -= OnItemStackCountChanged;

            m_RecipePanel.Terminate();

            foreach (var recipeRow in m_RecipeRows)
            {
                recipeRow.Clicked -= OnRecipeRowClicked;
            }
        }

        private void OnSearchInputChanged(string search)
        {
            foreach (var recipeRow in m_RecipeRows)
            {
                recipeRow.gameObject.SetActive(recipeRow.Recipe.Item.Name.Contains(search) ||
                                               recipeRow.Recipe.Item.Type.Name.Contains(search));
            }
        }

        public void Refresh(List<Recipe> recipes)
        {
            m_SelectedRecipe = null;

            DestroyRecipeRows();

            foreach (var recipe in recipes)
            {
                var recipeRow = m_RecipePool.Spawn();
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
            m_RecipePool.DespawnAll();

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

                AudioManager.Instance.PlayAlchemyBrew();
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