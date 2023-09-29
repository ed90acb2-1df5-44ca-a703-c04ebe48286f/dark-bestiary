using System;
using DarkBestiary.Components;
using DarkBestiary.Data;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary
{
    public class Food
    {
        public event Action<Food> Applied;
        public event Action<Food> Removed;

        public int Id { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public FoodType Type { get; }
        public string Icon { get; }
        public int Price { get; }
        public Behaviour Behaviour { get; }
        public bool IsApplied { get; private set; }

        public Food(FoodData data, Behaviour behaviour)
        {
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Type = data.Type;
            Icon = data.Icon;
            Price = data.Price;
            Behaviour = behaviour;
        }

        public void Apply(BehavioursComponent behaviours)
        {
            behaviours.ApplyAllStacks(Behaviour, behaviours.gameObject);
            IsApplied = true;

            Applied?.Invoke(this);
        }

        public void Remove(BehavioursComponent behaviours)
        {
            behaviours.RemoveAllStacks(Behaviour.Id);
            IsApplied = false;

            Removed?.Invoke(this);
        }
    }
}