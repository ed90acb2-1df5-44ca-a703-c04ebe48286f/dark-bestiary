using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class CraftViewController : ViewController<ICraftView>
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly CharacterManager characterManager;

        public CraftViewController(ICraftView view, IRecipeRepository recipeRepository,
            CharacterManager characterManager) : base(view)
        {
            this.recipeRepository = recipeRepository;
            this.characterManager = characterManager;
        }

        protected override void OnInitialize()
        {
            Character.RecipeUnlocked += OnRecipeUnlocked;
            View.Construct(GetRecipes(), this.characterManager.Character.Entity.GetComponent<InventoryComponent>());
        }

        protected override void OnTerminate()
        {
            Character.RecipeUnlocked -= OnRecipeUnlocked;
        }

        private void OnRecipeUnlocked(Recipe recipe)
        {
            View.RefreshRecipes(GetRecipes());
        }

        private List<Recipe> GetRecipes()
        {
            return this.recipeRepository
                .Find(recipe => recipe.Category == RecipeCategory.Blacksmith &&
                                (recipe.IsUnlocked || this.characterManager.Character.Data.UnlockedRecipes.Contains(recipe.Id)))
                .Select(recipe =>
                {
                    recipe.Item.ChangeOwner(this.characterManager.Character.Entity);

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