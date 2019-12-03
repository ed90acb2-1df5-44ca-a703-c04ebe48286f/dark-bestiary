using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IItemUpgradeView : IView
    {
        event Payload<Item> ItemPlaced;
        event Payload<Item> ItemRemoved;
        event Payload UpgradeButtonClicked;

        void ChanceTitle(string title);

        void Construct(Character character);

        void Refresh(Item item, List<RecipeIngredient> ingredients);

        void Cleanup();
    }
}