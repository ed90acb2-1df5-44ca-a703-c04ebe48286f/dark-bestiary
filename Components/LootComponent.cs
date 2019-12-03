using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;

namespace DarkBestiary.Components
{
    public class LootComponent : Component
    {
        private List<Loot> loot;

        public LootComponent Construct(List<Loot> loot)
        {
            this.loot = loot;
            return this;
        }

        public List<Item> RollDrop()
        {
            var level = GetComponent<UnitComponent>().Level;
            var items = this.loot.SelectMany(l => l.RollDrop(level)).ToList();

            foreach (var item in items)
            {
                item.ChangeOwner(gameObject);
            }

            return items;
        }
    }
}