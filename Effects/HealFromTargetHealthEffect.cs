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
        private readonly HealFromTargetHealthEffectData data;

        public HealFromTargetHealthEffect(HealFromTargetHealthEffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new HealFromTargetHealthEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            caster.GetComponent<HealthComponent>().Heal(
                caster, new Healing(target.GetComponent<HealthComponent>().HealthMax * this.data.Fraction));

            TriggerFinished();
        }
    }
}