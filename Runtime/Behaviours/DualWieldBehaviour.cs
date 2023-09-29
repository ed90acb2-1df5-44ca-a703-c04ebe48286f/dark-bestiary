using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class DualWieldBehaviour : Behaviour
    {
        private readonly Behaviour m_Behaviour;

        private BehavioursComponent m_Behaviours;
        private EquipmentComponent m_Equipment;

        public DualWieldBehaviour(DualWieldBehaviourData data, IBehaviourRepository behaviourRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Behaviour = behaviourRepository.Find(data.BehaviourId);
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
            if (!m_Equipment.IsDualWielding())
            {
                return;
            }

            m_Behaviours.ApplyAllStacks(m_Behaviour, Target);
        }

        private void OnItemUnequipped(Item item)
        {
            if (m_Equipment.IsDualWielding())
            {
                return;
            }

            m_Behaviours.RemoveAllStacks(m_Behaviour.Id);
        }
    }
}