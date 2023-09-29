using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class RuneCraftViewController : CraftViewController<IAlchemyView>
    {
        protected RuneCraftViewController(IAlchemyView view, IRecipeRepository recipeRepository) : base(view, recipeRepository, RecipeCategory.Runeforge)
        {
            IngredientInventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
        }
    }
}