using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnDealHealBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnDealHealBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            HealthComponent.AnyEntityHealed += OnEntityHealed;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            HealthComponent.AnyEntityHealed -= OnEntityHealed;
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (data.Healer != Target)
            {
                return;
            }

            var clone = this.effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Target);
        }
    }
}