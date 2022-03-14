using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.Masteries
{
    public class WeaponMastery : Mastery
    {
        private EquipmentComponent equipment;

        public WeaponMastery(MasteryData data, ItemModifier modifier) : base(data, modifier)
        {
        }

        protected override void OnInitialize()
        {
            this.equipment = Owner.GetComponent<EquipmentComponent>();
            this.equipment.ItemEquipped += OnItemEquipped;
            this.equipment.ItemUnequipped += OnItemUnequipped;

            RefreshModifiers();

            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        protected override void OnTerminate()
        {
            this.equipment.ItemEquipped -= OnItemEquipped;
            this.equipment.ItemUnequipped -= OnItemUnequipped;

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
            return this.equipment.Slots
                .Where(s => !s.IsEmpty && this.Data.ItemType == s.Item.Type.Type)
                .Select(s => s.Item)
                .ToList();
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Attacker != Owner)
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