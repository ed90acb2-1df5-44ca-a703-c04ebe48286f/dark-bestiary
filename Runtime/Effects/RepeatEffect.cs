using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RepeatEffect : Effect
    {
        private readonly RepeatEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        private int m_Counter;

        public RepeatEffect(RepeatEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new RepeatEffect(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            ApplyNext();
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            ApplyNext();
        }

        private void ApplyNext()
        {
            var effect = Inherit(m_EffectRepository.Find(m_Data.EffectId));
            effect.Finished += OnEffectFinished;
            effect.Apply(Caster, Target);
        }

        private void OnEffectFinished(Effect finished)
        {
            finished.Finished -= OnEffectFinished;

            m_Counter++;

            if (m_Counter >= m_Data.Times)
            {
                TriggerFinished();
                return;
            }

            ApplyNext();
        }
    }
}