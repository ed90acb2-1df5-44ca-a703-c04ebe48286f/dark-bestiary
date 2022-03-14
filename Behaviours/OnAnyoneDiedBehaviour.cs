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
    public class OnAnyoneDiedBehaviour : Behaviour
    {
        private readonly OnKillBehaviourData data;
        private readonly Effect effect;

        public OnAnyoneDiedBehaviour(OnKillBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            HealthComponent.AnyEntityDied += OnEntityDied;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            HealthComponent.AnyEntityDied -= OnEntityDied;
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.IsDummy())
            {
                return;
            }

            if (this.data.DamageFlags != DamageFlags.None && (data.Damage.Flags & this.data.DamageFlags) == 0 ||
                this.data.DamageInfoFlags != DamageInfoFlags.None && (data.Damage.InfoFlags & this.data.DamageInfoFlags) == 0)
            {
                return;
            }

            if (!this.Validators.ByPurpose(ValidatorPurpose.Other).Validate(Target, data.Victim))
            {
                return;
            }

            var clone = this.effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, this.data.EventSubject == BehaviourEventSubject.Me ? Target : data.Victim);
        }
    }
}