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
        private readonly OnStatusEffectBehaviourData m_Data;
        private readonly Behaviour m_Behaviour;

        private BehavioursComponent m_Behaviours;

        public OnStatusEffectBehaviour(OnStatusEffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Behaviour = Container.Instance.Resolve<IBehaviourRepository>().FindOrFail(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            m_Behaviours = target.GetComponent<BehavioursComponent>();

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

            var statusFlags = m_Behaviours.Behaviours.Aggregate(StatusFlags.None, (current, b) => current | b.StatusFlags);

            if ((statusFlags & m_Data.StatusFlags) == 0)
            {
                m_Behaviours.RemoveAllStacks(m_Behaviour.Id);
            }
            else
            {
                m_Behaviours.ApplyAllStacks(m_Behaviour, Target);
                m_Behaviours.SetStackCount(m_Behaviour.Id, StackCount);
            }
        }
    }
}