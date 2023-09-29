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
    public class OnDeathBehaviour : Behaviour
    {
        private readonly Effect m_Effect;

        public OnDeathBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            target.GetComponent<HealthComponent>().Died += OnDied;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<HealthComponent>().Died -= OnDied;
        }

        private void OnDied(EntityDiedEventData data)
        {
            if (!Validators.ByPurpose(ValidatorPurpose.Other).Validate(Caster, Target))
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, Target);
        }
    }
}