using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class SetBehaviour : Behaviour
    {
        private readonly List<Behaviour> behaviours;

        public SetBehaviour(SetBehaviourData data, IBehaviourRepository behaviourRepository,
            List<Validator> validators) : base(data, validators)
        {
            this.behaviours = behaviourRepository.Find(data.Behaviours);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            var behavioursComponent = target.GetComponent<BehavioursComponent>();

            foreach (var behaviour in this.behaviours)
            {
                behavioursComponent.Apply(behaviour, caster);
            }
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            var behavioursComponent = target.GetComponent<BehavioursComponent>();

            foreach (var behaviour in this.behaviours)
            {
                behavioursComponent.RemoveAllStacks(behaviour.Id);
            }
        }

        protected override void OnStackCountChanged(Behaviour changed)
        {
            foreach (var behaviour in this.behaviours)
            {
                behaviour.StackCount = changed.StackCount;
            }
        }
    }
}