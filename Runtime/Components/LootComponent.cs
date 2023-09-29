using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;

namespace DarkBestiary.Components
{
    public class LootComponent : Component
    {
        private List<int> m_Loot;

        public LootComponent Construct(List<int> loot)
        {
            m_Loot = loot;
            return this;
        }

        public List<Item> RollDrop()
        {
            var table = Container.Instance.Instantiate<Loot>();
            var items = m_Loot.SelectMany(table.RollDrop).ToList();

            foreach (var item in items)
            {
                item.ChangeOwner(gameObject);
            }

            return items;
        }
    }
}