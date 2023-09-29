using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddIngredientsConsoleCommand : IConsoleCommand
    {
        private readonly IItemRepository m_ItemRepository;

        public AddIngredientsConsoleCommand(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public string GetSignature()
        {
            return "add_ingredients";
        }

        public string GetDescription()
        {
            return "Give ingredients.";
        }

        public string Execute(string input)
        {
            Game.Instance.Character.Entity.GetComponent<InventoryComponent>()
                .Pickup(m_ItemRepository.Find(
                        item => item.Id == Constants.c_ItemIdDemonHeart ||
                                item.Id == Constants.c_ItemIdSphereOfAscension ||
                                item.Id == Constants.c_ItemIdSphereOfAugmentation ||
                                item.Id == Constants.c_ItemIdSphereOfTransmutation ||
                                item.Id == Constants.c_ItemIdSphereOfUnveiling ||
                                item.Id == Constants.c_ItemIdScrap ||
                                item.Id == Constants.c_ItemIdMagicDust ||
                                item.Id == Constants.c_ItemIdGlowingEssence ||
                                item.Id == Constants.c_ItemIdShadowCrystal
                    )
                    .Select(item => item.SetStack(999999)));

            return "Done.";
        }
    }
}