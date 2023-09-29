using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class SearchBehindEffect : Effect
    {
        private readonly SearchBehindEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public SearchBehindEffect(SearchBehindEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchBehindEffect(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var cell = BoardNavigator.Instance.BehindTheBackOfOccupying(target);

            if (cell == null || !cell.IsOccupied)
            {
                TriggerFinished();
                return;
            }

            m_EffectRepository.Find(m_Data.EffectId)?.Apply(caster, cell.OccupiedBy);
            TriggerFinished();
        }
    }
}