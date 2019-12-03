using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;

namespace DarkBestiary.UI.Views
{
    public interface ICraftView : IView, IHideOnEscape
    {
        void Construct(List<Recipe> recipes, InventoryComponent inventory);

        void RefreshRecipes(List<Recipe> recipes);
    }
}