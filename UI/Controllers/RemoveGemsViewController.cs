using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class RemoveGemsViewController : ItemUpgradeViewController
    {
        private readonly IItemRepository itemRepository;
        private readonly InventoryComponent inventory;

        public RemoveGemsViewController(IItemUpgradeView view, IItemRepository itemRepository,
            CharacterManager characterManager) : base(I18N.Instance.Get("ui_remove_gems"), view, characterManager)
        {
            this.itemRepository = itemRepository;
            this.inventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
        }

        protected override List<RecipeIngredient> GetIngredients()
        {
            return new List<RecipeIngredient>
            {
                new RecipeIngredient(this.itemRepository.FindOrFail(Constants.ItemIdPincers), 1),
                new RecipeIngredient(this.itemRepository.FindOrFail(Constants.ItemIdInhibitor), 1),
            };
        }

        protected override Currency GetCost()
        {
            return null;
        }

        protected override void Check(Item item)
        {
            if (item.Sockets.Count == 0 || item.Sockets.All(socket => socket.IsEmpty))
            {
                throw new GameplayException("");
            }
        }

        protected override void Upgrade(Item item)
        {
            foreach (var socket in item.Sockets.ToList())
            {
                if (socket.IsEmpty)
                {
                    continue;
                }

                item.RemoveSocket(socket);
                this.inventory.Pickup(socket);
            }

            AudioManager.Instance.PlayCraftSocket();
        }
    }
}