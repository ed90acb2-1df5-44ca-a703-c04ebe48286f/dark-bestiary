using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IItemForgingView : IView
    {
        event Payload<Item> ItemPlaced;
        event Payload<Item> ItemRemoved;
        event Payload UpgradeButtonClicked;

        void Construct(Character character);

        void Refresh(Item current, Item upgraded, List<RecipeIngredient> ingredients);

        void Cleanup();
    }
}