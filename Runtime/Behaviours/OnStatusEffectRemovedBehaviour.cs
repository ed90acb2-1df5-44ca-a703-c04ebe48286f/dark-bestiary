using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnStatusEffectRemovedBehaviour : Behaviour
    {
        private readonly OnStatusEffectRemovedBehaviourData m_Data;
        private readonly Effect m_Effect;

        public OnStatusEffectRemovedBehaviour(OnStatusEffectRemovedBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BehavioursComponent.AnyBehaviourRemoved += OnAnyBehaviourRemoved;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BehavioursComponent.AnyBehaviourRemoved -= OnAnyBehaviourRemoved;
        }

        private void OnAnyBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.Target != Target)
            {
                return;
            }

            if ((behaviour.StatusFlags & m_Data.StatusFlags) == 0)
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Target, EventSubject == BehaviourEventSubject.Me ? Target : Caster);
        }
    }
}