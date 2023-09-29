using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnTakeHealBehaviour : Behaviour
    {
        private readonly Effect m_Effect;

        public OnTakeHealBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            target.GetComponent<HealthComponent>().Healed += OnHealed;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<HealthComponent>().Healed -= OnHealed;
        }

        private void OnHealed(EntityHealedEventData data)
        {
            if (!Validators.ByPurpose(ValidatorPurpose.Other).Validate(data.Source, data.Target))
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Source);
        }
    }
}