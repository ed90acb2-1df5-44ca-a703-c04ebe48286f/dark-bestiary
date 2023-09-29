using System;

namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class CustomTransmutationIngredient : ITransmutationIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly Func<Item, bool> m_Predicate;

        public CustomTransmutationIngredient(string name, string icon, int count, Func<Item, bool> predicate)
        {
            m_Predicate = predicate;
            Name = name;
            Icon = icon;
            Count = count;
        }

        public bool Match(Item item)
        {
            return !item.IsEmpty && m_Predicate(item);
        }
    }
}