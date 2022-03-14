using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class CraftViewController<TView> : ViewController<TView> where TView : ICraftView
    {
        private readonly CharacterManager characterManager;
        private readonly IRecipeRepository recipeRepository;
        private readonly RecipeCategory recipeCategory;

        protected readonly InventoryComponent CharacterInventory;
        protected InventoryComponent IngredientInventory;

        protected CraftViewController(TView view, CharacterManager characterManager,
            IRecipeRepository recipeRepository, RecipeCategory recipeCategory) : base(view)
        {
            this.characterManager = characterManager;
            this.recipeRepository = recipeRepository;
            this.recipeCategory = recipeCategory;

            this.CharacterInventory = this.characterManager.Character.Entity.GetComponent<InventoryComponent>();
            this.IngredientInventory = Stash.Instance.GetIngredientsInventory();
        }

        protected override void OnInitialize()
        {
            Character.RecipeUnlocked += OnRecipeUnlocked;
            View.Construct(PrepareRecipes(), this.CharacterInventory, this.IngredientInventory);
        }

        protected override void OnTerminate()
        {
            Character.RecipeUnlocked -= OnRecipeUnlocked;
        }

        private void OnRecipeUnlocked(Recipe recipe)
        {
            if (recipe.Category != this.recipeCategory)
            {
                return;
            }

            View.Refresh(PrepareRecipes());
        }

        private IEnumerable<Recipe> GetRecipes()
        {
            return this.recipeRepository
                .Find(recipe => recipe.Category == this.recipeCategory &&
                                (recipe.IsUnlocked || this.characterManager.Character.Data.UnlockedRecipes.Contains(recipe.Id)));
        }

        private List<Recipe> PrepareRecipes()
        {
            return GetRecipes()
                .Select(recipe =>
                {
                    recipe.Item.ChangeOwner(this.characterManager.Character.Entity);
                    recipe.Item.Sockets = new List<Item>();
                    recipe.Item.Suffix = null;

                    foreach (var ingredient in recipe.Ingredients)
                    {
                        ingredient.Item.ChangeOwner(this.characterManager.Character.Entity);
                    }

                    return recipe;
                })
                .OrderBy(recipe => recipe.Item.Level)
                .ThenBy(recipe => recipe.Item.Type.Type)
                .ToList();
        }
    }
}