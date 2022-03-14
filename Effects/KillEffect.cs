using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class KillEffect : Effect
    {
        private readonly EffectData data;

        public KillEffect(EffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new KillEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<HealthComponent>().Kill(target, new Damage(0, DamageType.Health, WeaponSound.None, DamageFlags.None, DamageInfoFlags.None, Skill));
            TriggerFinished();
        }
    }
}