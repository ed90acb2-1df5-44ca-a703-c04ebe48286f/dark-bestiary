using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnStatusEffectBehaviour : Behaviour
    {
        private readonly OnStatusEffectBehaviourData data;
        private readonly Behaviour behaviour;

        private BehavioursComponent behaviours;

        public OnStatusEffectBehaviour(OnStatusEffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.behaviour = Container.Instance.Resolve<IBehaviourRepository>().FindOrFail(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            this.behaviours = target.GetComponent<BehavioursComponent>();

            BehavioursComponent.AnyBehaviourApplied += OnAnyBehaviourRemovedOrApplied;
            BehavioursComponent.AnyBehaviourRemoved += OnAnyBehaviourRemovedOrApplied;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourRemovedOrApplied;
            BehavioursComponent.AnyBehaviourRemoved -= OnAnyBehaviourRemovedOrApplied;
        }

        private void OnAnyBehaviourRemovedOrApplied(Behaviour behaviour)
        {
            if (behaviour.Target != Target)
            {
                return;
            }

            var statusFlags = this.behaviours.Behaviours.Aggregate(StatusFlags.None, (current, b) => current | b.StatusFlags);

            if ((statusFlags & this.data.StatusFlags) == 0)
            {
                this.behaviours.RemoveAllStacks(this.behaviour.Id);
            }
            else
            {
                this.behaviours.ApplyAllStacks(this.behaviour, Target);
                this.behaviours.SetStackCount(this.behaviour.Id, StackCount);
            }
        }
    }
}