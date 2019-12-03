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
    public class OnTakeHealBehaviour : Behaviour
    {
        private readonly Effect effect;
        private readonly List<Validator> validators;

        public OnTakeHealBehaviour(EffectBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
            this.validators = Container.Instance.Resolve<IValidatorRepository>().Find(data.Validators);
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
            if (this.validators.Any(v => !v.Validate(data.Healer, data.Target)))
            {
                return;
            }

            this.effect.Clone().Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Healer);
        }
    }
}