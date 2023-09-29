using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class SearchRandomPoints : Effect
    {
        private readonly SearchRandomPointsData m_Data;
        private readonly List<ValidatorWithPurpose> m_Validators;
        private readonly IEffectRepository m_EffectRepository;

        public SearchRandomPoints(SearchRandomPointsData data, List<ValidatorWithPurpose> validators, IEffectRepository effectRepository)
            : base(data, validators)
        {
            m_Data = data;
            m_Validators = validators;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchRandomPoints(m_Data, m_Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var cells = BoardNavigator.Instance
                .WithinCircle(target, m_Data.RangeMax)
                .Where(cell => cell.IsWalkable && (m_Data.IncludeOccupied || !cell.IsOccupied))
                .Shuffle();

            if (m_Data.RangeMin > 0)
            {
                var excluded = BoardNavigator.Instance.WithinCircle(target, m_Data.RangeMin);
                cells = cells.Where(c => !excluded.Contains(c));
            }

            cells = cells.Take(m_Data.Limit);

            foreach (var cell in cells)
            {
                var effect = m_EffectRepository.Find(m_Data.EffectId);
                effect.Skill = Skill;
                effect.DamageMultiplier = DamageMultiplier;
                effect.Apply(caster, cell.transform.position);
            }

            TriggerFinished();
        }
    }
}