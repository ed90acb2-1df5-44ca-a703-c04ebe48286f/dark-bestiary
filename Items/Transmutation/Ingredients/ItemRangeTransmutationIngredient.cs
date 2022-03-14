using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class ItemRangeTransmutationIngredient : ITransmutationIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly IEnumerable<int> items;

        public ItemRangeTransmutationIngredient(string name, string icon, IEnumerable<int> items, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;

            this.items = items;
        }

        public bool Match(Item item)
        {
            return this.items.Contains(item.Id);
        }
    }
}