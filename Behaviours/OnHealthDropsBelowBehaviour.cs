using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnHealthDropsBelowBehaviour : Behaviour
    {
        private readonly OnHealthDropsBelowBehaviourData data;
        private readonly IEffectRepository effectRepository;
        private HealthComponent health;

        public OnHealthDropsBelowBehaviour(OnHealthDropsBelowBehaviourData data,
            IEffectRepository effectRepository, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            this.health = target.GetComponent<HealthComponent>();
            this.health.Damaged += OnDamaged;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            this.health.Damaged -= OnDamaged;
            this.health = null;
        }

        private void OnDamaged(EntityDamagedEventData data)
        {
            if (this.health.HealthFraction >= this.data.Fraction)
            {
                return;
            }

            var effect = this.effectRepository.FindOrFail(this.data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, this.data.EventSubject == BehaviourEventSubject.Me ? Target : data.Attacker);
        }
    }
}