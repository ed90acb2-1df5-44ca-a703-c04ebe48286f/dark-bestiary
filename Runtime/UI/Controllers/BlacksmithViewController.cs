using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class BlacksmithViewController : CraftViewController<IBlacksmithView>
    {
        protected BlacksmithViewController(IBlacksmithView view, IRecipeRepository recipeRepository) : base(view, recipeRepository, RecipeCategory.Blacksmith)
        {
        }
    }
}