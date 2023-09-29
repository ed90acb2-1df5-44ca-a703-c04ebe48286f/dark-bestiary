using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class AlchemyViewController : CraftViewController<IAlchemyView>
    {
        protected AlchemyViewController(IAlchemyView view, IRecipeRepository recipeRepository) : base(view, recipeRepository, RecipeCategory.Alchemy)
        {
        }
    }
}