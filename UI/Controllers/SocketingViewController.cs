using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SocketingViewController : ItemUpgradeViewController
    {
        private readonly IItemRepository itemRepository;

        public SocketingViewController(IItemUpgradeView view, IItemRepository itemRepository,
            CharacterManager characterManager) : base(I18N.Instance.Get("ui_socketing"), view, characterManager)
        {
            this.itemRepository = itemRepository;
        }

        protected override List<RecipeIngredient> GetIngredients()
        {
            return new List<RecipeIngredient>
            {
                new RecipeIngredient(this.itemRepository.FindOrFail(Constants.ItemIdDemonHeart), 1),
                new RecipeIngredient(this.itemRepository.FindOrFail(Constants.ItemIdAdamantiteHammer), 1),
            };
        }

        protected override void Check(Item item)
        {
            if (item.IsMaximumSocketCount)
            {
                throw new MaxSocketCountException();
            }
        }

        protected override void Upgrade(Item item)
        {
            item.AddSocket();
            AudioManager.Instance.PlayCraftSocket();
        }
    }
}