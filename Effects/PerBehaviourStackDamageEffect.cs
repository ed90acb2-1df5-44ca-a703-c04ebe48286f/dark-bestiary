using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class PerBehaviourStackDamageEffect : DamageEffect
    {
        private readonly PerBehaviourStackDamageEffectData data;

        public PerBehaviourStackDamageEffect(PerBehaviourStackDamageEffectData data,
            List<ValidatorWithPurpose> validators, IEffectRepository effectRepository) : base(data, validators, effectRepository)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new PerBehaviourStackDamageEffect(this.data, this.Validators, this.EffectRepository);
        }

        protected override float Modify(float amount, GameObject caster, GameObject target)
        {
            var behaviours = this.data.Subject == DamageSubject.Attacker
                ? caster?.GetComponent<BehavioursComponent>()
                : target?.GetComponent<BehavioursComponent>();

            if (behaviours == null)
            {
                return amount;
            }

            if (this.data.StatusFlags == StatusFlags.None)
            {
                return amount * behaviours.GetStackCount(this.data.BehaviourId);
            }

            var stacks = behaviours.Behaviours
                .Where(b => (b.StatusFlags & this.data.StatusFlags) > 0)
                .Sum(b => b.StackCount);

            return amount * stacks;
        }
    }
}