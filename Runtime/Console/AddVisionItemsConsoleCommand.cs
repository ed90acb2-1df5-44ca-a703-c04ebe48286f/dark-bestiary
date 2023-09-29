using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddVisionItemsConsoleCommand : IConsoleCommand
    {
        private readonly IItemRepository m_ItemRepository;

        public AddVisionItemsConsoleCommand(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
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
            Game.Instance.Character.Entity.GetComponent<InventoryComponent>()
                .Pickup(
                    m_ItemRepository.Find(
                        item => item.RarityId == Constants.c_ItemRarityIdVision));

            return "Done.";
        }
    }
}