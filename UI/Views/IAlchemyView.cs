using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Items.Alchemy;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IAlchemyView : IView, IHideOnEscape
    {
        event Payload Combine;
        event Payload<Item> AddItem;
        event Payload<Item, int> AddItemIndex;
        event Payload<Item> RemoveItem;
        event Payload<Item, Item> SwapItems;

        void RefreshItems(List<Item> items);

        void ShowResult(Recipe recipe);

        void ClearResult();

        void ShowUnknownResult();

        void ShowImpossibleCombination();

        void OnSuccess();

        void Construct(Character character, List<Item> items, List<IAlchemyRecipe> recipes);

        void Block(Item item);

        void Unblock(Item item);
    }
}