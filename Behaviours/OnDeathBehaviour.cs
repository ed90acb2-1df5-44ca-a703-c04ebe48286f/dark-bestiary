using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnDeathBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnDeathBehaviour(EffectBehaviourData data, List<Validator> validators) : base(data, validators)
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
            if (this.Validators.Any(v => !v.Validate(Caster, Target)))
            {
                return;
            }

            this.effect.Clone().Apply(Caster, Target);
        }
    }
}