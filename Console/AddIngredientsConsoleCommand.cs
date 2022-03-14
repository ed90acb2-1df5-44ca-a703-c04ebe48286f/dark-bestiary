using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddIngredientsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly IItemRepository itemRepository;

        public AddIngredientsConsoleCommand(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.characterManager = characterManager;
            this.itemRepository = itemRepository;
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
            this.characterManager.Character.Entity.GetComponent<InventoryComponent>()
                .Pickup(this.itemRepository.Find(
                    item => item.Id == Constants.ItemIdAdamantiteHammer ||
                            item.Id == Constants.ItemIdPincers ||
                            item.Id == Constants.ItemIdDemonHeart ||
                            item.Id == Constants.ItemIdSphereOfAscension ||
                            item.Id == Constants.ItemIdSphereOfAugmentation ||
                            item.Id == Constants.ItemIdSphereOfTransmutation ||
                            item.Id == Constants.ItemIdSphereOfUnveiling ||
                            item.Id == Constants.ItemIdScrap ||
                            item.Id == Constants.ItemIdMagicDust ||
                            item.Id == Constants.ItemIdGlowingEssence ||
                            item.Id == Constants.ItemIdShadowCrystal ||
                            item.Id == Constants.ItemIdCatalyst ||
                            item.Id == Constants.ItemIdInhibitor ||
                            item.Id == Constants.ItemIdStabilizer)
                    .Select(item => item.SetStack(999999)));

            return "Done.";
        }
    }
}