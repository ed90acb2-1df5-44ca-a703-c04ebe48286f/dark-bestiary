using System;

namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class CustomTransmutationIngredient : ITransmutationIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly Func<Item, bool> predicate;

        public CustomTransmutationIngredient(string name, string icon, int count, Func<Item, bool> predicate)
        {
            this.predicate = predicate;
            Name = name;
            Icon = icon;
            Count = count;
        }

        public bool Match(Item item)
        {
            return !item.IsEmpty && this.predicate(item);
        }
    }
}