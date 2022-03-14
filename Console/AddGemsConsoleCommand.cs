using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddGemsConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly IItemRepository itemRepository;

        public AddGemsConsoleCommand(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.characterManager = characterManager;
            this.itemRepository = itemRepository;
        }

        public string GetSignature()
        {
            return "add_gems";
        }

        public string GetDescription()
        {
            return "Give gems.";
        }

        public string Execute(string input)
        {
            this.characterManager.Character.Entity.GetComponent<InventoryComponent>()
                .Pickup(
                    this.itemRepository.Find(
                        item => item.TypeId == Constants.ItemTypeIdGem).Select(item => item.SetStack(100)));

            return "Done.";
        }
    }
}