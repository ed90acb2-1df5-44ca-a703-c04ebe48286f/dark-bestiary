using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddRunesConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly IItemRepository itemRepository;

        public AddRunesConsoleCommand(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.characterManager = characterManager;
            this.itemRepository = itemRepository;
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
            var runes = this.itemRepository.Find(item =>
                item.TypeId == Constants.ItemTypeIdRune ||
                item.TypeId == Constants.ItemTypeIdMinorRune ||
                item.TypeId == Constants.ItemTypeIdMajorRune
            ).Select(x => x.SetStack(100));

            this.characterManager.Character.Entity.GetComponent<InventoryComponent>().Pickup(runes);

            return "Done.";
        }
    }
}