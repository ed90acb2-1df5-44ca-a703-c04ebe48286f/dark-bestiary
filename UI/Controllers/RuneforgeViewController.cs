using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class RuneforgeViewController : CraftViewController<IAlchemyView>
    {
        protected RuneforgeViewController(IAlchemyView view, CharacterManager characterManager,
            IRecipeRepository recipeRepository) : base(view, characterManager, recipeRepository, RecipeCategory.Runeforge)
        {
            this.IngredientInventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
        }
    }
}