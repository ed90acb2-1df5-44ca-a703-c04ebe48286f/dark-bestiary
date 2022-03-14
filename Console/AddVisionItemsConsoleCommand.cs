using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddVisionItemsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly IItemRepository itemRepository;

        public AddVisionItemsConsoleCommand(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.characterManager = characterManager;
            this.itemRepository = itemRepository;
        }

        public string GetSignature()
        {
            return "add_vision_items";
        }

        public string GetDescription()
        {
            return "Give vision items.";
        }

        public string Execute(string input)
        {
            this.characterManager.Character.Entity.GetComponent<InventoryComponent>()
                .Pickup(
                    this.itemRepository.Find(
                        item => item.RarityId == Constants.ItemRarityIdVision));

            return "Done.";
        }
    }
}