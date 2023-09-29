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
            return new RandomElementDamageEffect(Data, Validators, EffectRepository);
        }

        protected override DamageType GetDamageType()
        {
            return new List<DamageType>{DamageType.Cold, DamageType.Fire, DamageType.Lightning}.Random();
        }
    }
}