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
        private readonly List<Behaviour> m_Behaviours;

        public SetBehaviour(SetBehaviourData data, IBehaviourRepository behaviourRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Behaviours = behaviourRepository.Find(data.Behaviours);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            var behavioursComponent = target.GetComponent<BehavioursComponent>();

            foreach (var behaviour in m_Behaviours)
            {
                behavioursComponent.ApplyAllStacks(behaviour, caster);
            }
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            var behavioursComponent = target.GetComponent<BehavioursComponent>();

            foreach (var behaviour in m_Behaviours)
            {
                behavioursComponent.RemoveStack(behaviour.Id, StackCount);
            }
        }

        protected override void OnStackCountChanged(Behaviour _, int delta)
        {
            foreach (var behaviour in m_Behaviours)
            {
                behaviour.StackCount += delta;
            }
        }
    }
}