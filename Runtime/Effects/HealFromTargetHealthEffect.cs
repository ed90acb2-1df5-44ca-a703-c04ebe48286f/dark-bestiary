using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class HealFromTargetHealthEffect : Effect
    {
        private readonly HealFromTargetHealthEffectData m_Data;

        public HealFromTargetHealthEffect(HealFromTargetHealthEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new HealFromTargetHealthEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            caster.GetComponent<HealthComponent>().Heal(
                caster, new Healing(target.GetComponent<HealthComponent>().HealthMax * m_Data.Fraction, HealFlags.None, Skill));

            TriggerFinished();
        }
    }
}