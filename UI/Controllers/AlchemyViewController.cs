using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class AlchemyViewController : CraftViewController<IAlchemyView>
    {
        protected AlchemyViewController(IAlchemyView view, CharacterManager characterManager,
            IRecipeRepository recipeRepository) : base(view, characterManager, recipeRepository, RecipeCategory.Alchemy)
        {
        }
    }
}