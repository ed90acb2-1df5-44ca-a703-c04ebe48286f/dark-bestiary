using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class RemoveGemsViewController : ItemUpgradeViewController
    {
        private readonly InventoryComponent m_Inventory;

        public RemoveGemsViewController(IItemUpgradeView view) : base(I18N.Instance.Get("ui_remove_gems"), view)
        {
            m_Inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
        }

        protected override List<RecipeIngredient> GetIngredients()
        {
            return new List<RecipeIngredient>();
        }

        protected override Currency? GetCost()
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
                m_Inventory.Pickup(socket);
            }

            AudioManager.Instance.PlayCraftSocket();
        }
    }
}