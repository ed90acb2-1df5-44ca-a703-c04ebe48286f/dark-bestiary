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
    public class SearchLineEffect : Effect
    {
        private readonly Effect m_Effect;
        private readonly SearchLineEffectData m_Data;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly IEffectRepository m_EffectRepository;

        public SearchLineEffect(SearchLineEffectData data, List<ValidatorWithPurpose> validators,
            BoardNavigator boardNavigator, IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_BoardNavigator = boardNavigator;
            m_EffectRepository = effectRepository;
            m_Effect = effectRepository.Find(data.EffectId);
        }

        protected override Effect New()
        {
            return new SearchLineEffect(m_Data, Validators, m_BoardNavigator, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (m_Effect == null)
            {
                TriggerFinished();
                return;
            }

            var origin = OriginPosition();

            var cells = m_BoardNavigator.WithinLine(origin, (target - origin).normalized, m_Data.Length);

            if (m_Data.CheckLineOfSight)
            {
                cells = cells.OnLineOfSight(origin).ToList();
            }

            var entities = cells.ToEntities()
                .Where(entity => entity.IsAlive() && Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, entity))
                .OrderBy(entity => (entity.transform.position - target).sqrMagnitude)
                .ToList();

            if (m_Data.ExcludeOrigin)
            {
                var excluded = m_BoardNavigator.EntitiesInRadius(origin, 0);
                entities = entities.Where(entity => !excluded.Contains(entity)).ToList();
            }

            foreach (var entity in entities)
            {
                Inherit(m_Effect.Clone()).Apply(caster, entity);
            }

            TriggerFinished();
        }
    }
}