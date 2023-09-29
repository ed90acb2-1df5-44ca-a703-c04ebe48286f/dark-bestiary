using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class CraftViewController<TView> : ViewController<TView> where TView : ICraftView
    {
        private readonly IRecipeRepository m_RecipeRepository;
        private readonly RecipeCategory m_RecipeCategory;

        protected readonly InventoryComponent CharacterInventory;
        protected InventoryComponent IngredientInventory;

        protected CraftViewController(TView view, IRecipeRepository recipeRepository, RecipeCategory recipeCategory) : base(view)
        {
            m_RecipeRepository = recipeRepository;
            m_RecipeCategory = recipeCategory;

            CharacterInventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
            IngredientInventory = Stash.Instance.GetIngredientsInventory();
        }

        protected override void OnInitialize()
        {
            View.Construct(PrepareRecipes(), CharacterInventory, IngredientInventory);
        }

        private IEnumerable<Recipe> GetRecipes()
        {
            return m_RecipeRepository
                .Find(x => x.Category == m_RecipeCategory)
                .OrderBy(x => x.Item.Rarity?.Type)
                .ThenBy(x => x.Item.Type.Type);
        }

        private List<Recipe> PrepareRecipes()
        {
            return GetRecipes()
                .Select(recipe =>
                {
                    recipe.Item.ChangeOwner(Game.Instance.Character.Entity);
                    recipe.Item.Sockets = new List<Item>();
                    recipe.Item.Suffix = null;

                    foreach (var ingredient in recipe.Ingredients)
                    {
                        ingredient.Item.ChangeOwner(Game.Instance.Character.Entity);
                    }

                    return recipe;
                })
                .ToList();
        }
    }
}