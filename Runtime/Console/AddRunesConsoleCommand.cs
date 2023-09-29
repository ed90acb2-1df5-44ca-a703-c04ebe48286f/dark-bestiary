using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddRunesConsoleCommand : IConsoleCommand
    {
        private readonly IItemRepository m_ItemRepository;

        public AddRunesConsoleCommand(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public string GetSignature()
        {
            return "add_runes";
        }

        public string GetDescription()
        {
            return "Give runes.";
        }

        public string Execute(string input)
        {
            var runes = m_ItemRepository.Find(item =>
                item.TypeId == Constants.c_ItemTypeIdRune ||
                item.TypeId == Constants.c_ItemTypeIdMinorRune ||
                item.TypeId == Constants.c_ItemTypeIdMajorRune
            ).Select(x => x.SetStack(100));

            Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(runes);

            return "Done.";
        }
    }
}