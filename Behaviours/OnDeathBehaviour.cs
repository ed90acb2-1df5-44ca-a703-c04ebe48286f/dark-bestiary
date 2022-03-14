using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnDeathBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnDeathBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
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
            if (!this.Validators.ByPurpose(ValidatorPurpose.Other).Validate(Caster, Target))
            {
                return;
            }

            var clone = this.effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, Target);
        }
    }
}