using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class IfElseEffect : Effect
    {
        private readonly IfElseEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public IfElseEffect(IfElseEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new IfElseEffect(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            ApplyIfElse(caster, target);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            ApplyIfElse(caster, target);
        }

        private void ApplyIfElse(GameObject caster, object target)
        {
            var effectId = Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, target)
                ? m_Data.IfEffectId
                : m_Data.ElseEffectId;

            var effect = m_EffectRepository.Find(effectId);

            if (effect == null)
            {
                TriggerFinished();
                return;
            }

            Inherit(effect);

            effect.Finished += OnEffectFinished;
            effect.Apply(caster, target);
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;
            TriggerFinished();
        }
    }
}