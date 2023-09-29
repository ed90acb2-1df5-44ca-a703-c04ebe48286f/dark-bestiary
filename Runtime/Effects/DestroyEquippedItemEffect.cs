using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class DestroyEquippedItemEffect : Effect
    {
        private readonly DestroyEquippedItemEffectData m_Data;

        public DestroyEquippedItemEffect(DestroyEquippedItemEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new DestroyEquippedItemEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var equipment = target.GetComponent<EquipmentComponent>();
            var toDestroy = equipment.Slots.FirstOrDefault(s => s.Item.Id == m_Data.ItemId)?.Item;

            if (toDestroy == null)
            {
                TriggerFinished();
                return;
            }

            equipment.Unequip(toDestroy);
            toDestroy.Inventory.Remove(toDestroy);

            TriggerFinished();
        }
    }
}