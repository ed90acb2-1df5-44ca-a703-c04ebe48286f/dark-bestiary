using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddItemConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly IItemRepository itemRepository;

        public AddItemConsoleCommand(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.characterManager = characterManager;
            this.itemRepository = itemRepository;
        }

        public string GetSignature()
        {
            return "add_item";
        }

        public string GetDescription()
        {
            return "Add item. (Format: [itemId] [amount])";
        }

        public string Execute(string input)
        {
            var options = input.Split();

            var itemId = int.Parse(options[0]);
            var count = 1;

            if (options.Length > 1)
            {
                count = int.Parse(options[1]);
            }

            var item = this.itemRepository.FindOrFail(itemId).SetStack(count);

            this.characterManager.Character.Entity.GetComponent<InventoryComponent>().Pickup(item);

            return $"{this.characterManager.Character.Name} received item {item.Name} x{item.StackCount}";
        }
    }
}