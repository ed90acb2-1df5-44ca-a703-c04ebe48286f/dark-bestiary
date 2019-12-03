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
        private readonly Behaviour behaviour;

        private BehavioursComponent behaviours;
        private EquipmentComponent equipment;

        public DualWieldBehaviour(DualWieldBehaviourData data, IBehaviourRepository behaviourRepository,
            List<Validator> validators) : base(data, validators)
        {
            this.behaviour = behaviourRepository.Find(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            this.behaviours = target.GetComponent<BehavioursComponent>();

            this.equipment = target.GetComponent<EquipmentComponent>();
            this.equipment.ItemEquipped += OnItemEquipped;
            this.equipment.ItemUnequipped += OnItemUnequipped;

            OnItemEquipped(null);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            this.equipment.ItemEquipped -= OnItemEquipped;
            this.equipment.ItemUnequipped -= OnItemUnequipped;

            this.behaviours.RemoveAllStacks(this.behaviour.Id);
        }

        private void OnItemEquipped(Item item)
        {
            if (!this.equipment.IsDualWielding())
            {
                return;
            }

            this.behaviours.Apply(this.behaviour, Target);
        }

        private void OnItemUnequipped(Item item)
        {
            if (this.equipment.IsDualWielding())
            {
                return;
            }

            this.behaviours.RemoveAllStacks(this.behaviour.Id);
        }
    }
}