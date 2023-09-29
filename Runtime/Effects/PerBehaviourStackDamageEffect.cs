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
        private readonly PerBehaviourStackDamageEffectData m_Data;

        public PerBehaviourStackDamageEffect(PerBehaviourStackDamageEffectData data,
            List<ValidatorWithPurpose> validators, IEffectRepository effectRepository) : base(data, validators, effectRepository)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new PerBehaviourStackDamageEffect(m_Data, Validators, EffectRepository);
        }

        protected override float Modify(float amount, GameObject caster, GameObject target)
        {
            var behaviours = m_Data.Subject == DamageSubject.Attacker
                ? caster?.GetComponent<BehavioursComponent>()
                : target?.GetComponent<BehavioursComponent>();

            if (behaviours == null)
            {
                return amount;
            }

            if (m_Data.StatusFlags == StatusFlags.None)
            {
                return amount * behaviours.GetStackCount(m_Data.BehaviourId);
            }

            var stacks = behaviours.Behaviours
                .Where(b => (b.StatusFlags & m_Data.StatusFlags) > 0)
                .Sum(b => b.StackCount);

            return amount * stacks;
        }
    }
}