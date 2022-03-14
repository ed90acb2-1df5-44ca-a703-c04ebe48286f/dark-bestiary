using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;

namespace DarkBestiary.Effects
{
    public class RandomElementDamageEffect : DamageEffect
    {
        public RandomElementDamageEffect(DamageEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators, effectRepository)
        {
        }

        protected override Effect New()
        {
            return new RandomElementDamageEffect(this.Data, this.Validators, this.EffectRepository);
        }

        protected override DamageType GetDamageType()
        {
            return new List<DamageType>{DamageType.Cold, DamageType.Fire, DamageType.Lightning}.Random();
        }
    }
}