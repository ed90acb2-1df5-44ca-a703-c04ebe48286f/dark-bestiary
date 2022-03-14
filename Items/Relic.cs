using System;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Items
{
    public class Relic
    {
        public static readonly Relic Empty = new Relic(-1, I18N.Instance.Get("Empty"));

        public event Payload<Relic> Equipped;
        public event Payload<Relic> Unequipped;

        public bool IsEmpty => Id == Empty.Id;
        public string ColoredName => Rarity == null ? Name : $"<color={Rarity.ColorCode}>{Name}</color>";

        public int Id { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public I18NString Lore { get; }
        public bool IsEquipped { get; private set; }
        public string Icon { get; }
        public Rarity Rarity { get; }
        public Behaviour Behaviour { get; }

        public Experience Experience { get; private set; }
        public GameObject Owner { get; set; }

        public Relic(RelicData data, Rarity rarity, Behaviour behaviour)
        {
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Lore= I18N.Instance.Get(data.LoreKey);
            Icon = data.Icon;
            Rarity = rarity;
            Behaviour = behaviour;
        }

        private Relic(int id, I18NString name)
        {
            Id = id;
            Name = name;
        }

        public void Construct(int level, int experience)
        {
            Experience = new Experience(level, 10, experience, Formula);
        }

        public void Equip()
        {
            if (IsEmpty)
            {
                return;
            }

            Owner.GetComponent<BehavioursComponent>().ApplyAllStacks(Behaviour, Owner);

            Experience.LevelUp += OnLevelUp;
            OnLevelUp(Experience);

            IsEquipped = true;

            Equipped?.Invoke(this);
        }

        public void Unequip()
        {
            if (IsEmpty)
            {
                return;
            }

            Owner.GetComponent<BehavioursComponent>().RemoveAllStacks(Behaviour.Id);

            Experience.LevelUp -= OnLevelUp;

            IsEquipped = false;

            Unequipped?.Invoke(this);
        }

        private void OnLevelUp(Experience experience)
        {
            Owner.GetComponent<BehavioursComponent>().SetStackCount(Behaviour.Id, experience.Level);
        }

        private int Formula(int level)
        {
            if (level < 2)
            {
                return 0;
            }

            return (int) (25 * Math.Pow(level, 3f) * (1 + 0.25f * (int) Rarity.Type));
        }
    }
}