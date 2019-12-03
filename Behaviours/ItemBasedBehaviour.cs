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
        private readonly Behaviour behaviour;
        private readonly ItemCategory requiredCategory;

        private BehavioursComponent behaviours;
        private EquipmentComponent equipment;

        public ItemBasedBehaviour(ItemBasedBehaviourData data,
            IItemCategoryRepository itemCategoryRepository, IBehaviourRepository behaviourRepository,
            List<Validator> validators) : base(data, validators)
        {
            this.behaviour = behaviourRepository.FindOrFail(data.BehaviourId);
            this.requiredCategory = itemCategoryRepository.FindOrFail(data.ItemCategoryId);
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
            var itemsCount = this.equipment.Slots.Count(slot => this.requiredCategory.Contains(slot.Item.Type));

            if (itemsCount == 0)
            {
                return;
            }

            var stackCount = this.behaviours.GetStackCount(this.behaviour.Id);

            if (stackCount == 0)
            {
                this.behaviours.Apply(this.behaviour, Caster);
            }

            this.behaviours.SetStackCount(this.behaviour.Id, itemsCount);
        }

        private void OnItemUnequipped(Item item)
        {
            var itemsCount = this.equipment.Slots.Count(slot => this.requiredCategory.Contains(slot.Item.Type));

            if (itemsCount == 0)
            {
                this.behaviours.RemoveAllStacks(this.behaviour.Id);
            }
            else
            {
                this.behaviours.SetStackCount(this.behaviour.Id, itemsCount);
            }
        }
    }
}