using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class ItemBasedBehaviour : Behaviour
    {
        private readonly Behaviour m_Behaviour;
        private readonly ItemCategory m_RequiredCategory;

        private BehavioursComponent m_Behaviours;
        private EquipmentComponent m_Equipment;

        public ItemBasedBehaviour(ItemBasedBehaviourData data,
            IItemCategoryRepository itemCategoryRepository, IBehaviourRepository behaviourRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Behaviour = behaviourRepository.FindOrFail(data.BehaviourId);
            m_RequiredCategory = itemCategoryRepository.FindOrFail(data.ItemCategoryId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            m_Behaviours = target.GetComponent<BehavioursComponent>();

            m_Equipment = target.GetComponent<EquipmentComponent>();
            m_Equipment.ItemEquipped += OnItemEquipped;
            m_Equipment.ItemUnequipped += OnItemUnequipped;

            OnItemEquipped(null);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            m_Equipment.ItemEquipped -= OnItemEquipped;
            m_Equipment.ItemUnequipped -= OnItemUnequipped;

            m_Behaviours.RemoveAllStacks(m_Behaviour.Id);
        }

        private void OnItemEquipped(Item item)
        {
            var itemsCount = m_Equipment.Slots.Count(slot => m_RequiredCategory.Contains(slot.Item.Type));

            if (itemsCount == 0)
            {
                return;
            }

            var stackCount = m_Behaviours.GetStackCount(m_Behaviour.Id);

            if (stackCount == 0)
            {
                m_Behaviours.ApplyAllStacks(m_Behaviour, Caster);
            }

            m_Behaviours.SetStackCount(m_Behaviour.Id, itemsCount);
        }

        private void OnItemUnequipped(Item item)
        {
            var itemsCount = m_Equipment.Slots.Count(slot => m_RequiredCategory.Contains(slot.Item.Type));

            if (itemsCount == 0)
            {
                m_Behaviours.RemoveAllStacks(m_Behaviour.Id);
            }
            else
            {
                m_Behaviours.SetStackCount(m_Behaviour.Id, itemsCount);
            }
        }
    }
}