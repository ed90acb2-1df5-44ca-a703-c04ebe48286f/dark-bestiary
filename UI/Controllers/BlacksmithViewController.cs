using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class BlacksmithViewController : CraftViewController<IBlacksmithView>
    {
        protected BlacksmithViewController(IBlacksmithView view, CharacterManager characterManager,
            IRecipeRepository recipeRepository) : base(view, characterManager, recipeRepository, RecipeCategory.Blacksmith)
        {
        }
    }
}