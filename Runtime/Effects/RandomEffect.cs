using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RandomEffect : Effect
    {
        private readonly RandomEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public RandomEffect(RandomEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new RandomEffect(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            ApplyRandom(caster, target);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            ApplyRandom(caster, target);
        }

        private void ApplyRandom(GameObject caster, object target)
        {
            var effect = m_EffectRepository.Find(m_Data.Effects.Random());
            effect.Skill = Skill;
            effect.DamageMultiplier = DamageMultiplier;
            effect.Origin = Origin;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);

            TriggerFinished();
        }
    }
}