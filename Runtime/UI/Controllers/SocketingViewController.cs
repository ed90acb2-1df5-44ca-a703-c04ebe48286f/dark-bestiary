using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SocketingViewController : ItemUpgradeViewController
    {
        private readonly IItemRepository m_ItemRepository;

        public SocketingViewController(IItemUpgradeView view, IItemRepository itemRepository) : base(I18N.Instance.Get("ui_socketing"), view)
        {
            m_ItemRepository = itemRepository;
        }

        protected override List<RecipeIngredient> GetIngredients()
        {
            return new List<RecipeIngredient>
            {
                new(m_ItemRepository.FindOrFail(Constants.c_ItemIdDemonHeart), 1),
            };
        }

        protected override Currency GetCost()
        {
            return null;
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