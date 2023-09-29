using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Events;
using DarkBestiary.Items;

namespace DarkBestiary.Masteries
{
    public class WeaponMastery : Mastery
    {
        private EquipmentComponent m_Equipment;

        public WeaponMastery(MasteryData data, ItemModifier modifier) : base(data, modifier)
        {
        }

        protected override void OnInitialize()
        {
            m_Equipment = Owner.GetComponent<EquipmentComponent>();
            m_Equipment.ItemEquipped += OnItemEquipped;
            m_Equipment.ItemUnequipped += OnItemUnequipped;

            RefreshModifiers();

            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        protected override void OnTerminate()
        {
            m_Equipment.ItemEquipped -= OnItemEquipped;
            m_Equipment.ItemUnequipped -= OnItemUnequipped;

            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
        }

        protected override void RefreshModifiers()
        {
            RemoveModifiers();

            var wearing = WearingItemOfRequiredType();

            if (wearing.Count > 0)
            {
                ApplyModifiers(wearing.Count);
            }
        }

        private void OnItemEquipped(Item item)
        {
            RefreshModifiers();
        }

        private void OnItemUnequipped(Item item)
        {
            RefreshModifiers();
        }

        private List<Item> WearingItemOfRequiredType()
        {
            return m_Equipment.Slots
                .Where(s => !s.IsEmpty && Data.ItemType == s.Item.Type.Type)
                .Select(s => s.Item)
                .ToList();
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Source != Owner)
            {
                return;
            }

            if (WearingItemOfRequiredType().Count == 0)
            {
                return;
            }

            Experience.Add(1);
        }
    }
}