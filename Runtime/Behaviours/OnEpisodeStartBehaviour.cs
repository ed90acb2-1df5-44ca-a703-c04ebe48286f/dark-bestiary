using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Scenarios;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnEpisodeStartBehaviour : Behaviour
    {
        private readonly EffectBehaviourData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public OnEpisodeStartBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Episode.AnyEpisodeStarted -= OnAnyEpisodeStarted;
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            if (!Target.gameObject.activeSelf)
            {
                return;
            }

            var effect = m_EffectRepository.Find(m_Data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, Caster);
        }
    }
}