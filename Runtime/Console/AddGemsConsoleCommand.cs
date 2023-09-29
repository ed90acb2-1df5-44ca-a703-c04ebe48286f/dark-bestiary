using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddGemsConsoleCommand : IConsoleCommand
    {
        private readonly IItemRepository m_ItemRepository;

        public AddGemsConsoleCommand(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
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
            Game.Instance.Character.Entity.GetComponent<InventoryComponent>()
                .Pickup(
                    m_ItemRepository.Find(
                        item => item.TypeId == Constants.c_ItemTypeIdGem).Select(item => item.SetStack(100)));

            return "Done.";
        }
    }
}