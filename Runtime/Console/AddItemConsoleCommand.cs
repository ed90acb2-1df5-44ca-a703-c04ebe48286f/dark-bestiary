using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddItemConsoleCommand : IConsoleCommand
    {
        private readonly IItemRepository m_ItemRepository;

        public AddItemConsoleCommand(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
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

            var item = m_ItemRepository.FindOrFail(itemId).SetStack(count);

            Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(item);

            return $"{Game.Instance.Character.Name} received item {item.Name} x{item.StackCount}";
        }
    }
}