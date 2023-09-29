using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class SearchAreaEffect : Effect
    {
        private readonly SearchAreaEffectData m_Data;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly IEffectRepository m_EffectRepository;

        public SearchAreaEffect(SearchAreaEffectData data, List<ValidatorWithPurpose> validators,
            BoardNavigator boardNavigator, IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_BoardNavigator = boardNavigator;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchAreaEffect(m_Data, Validators, m_BoardNavigator, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var origin = OriginPosition();

            if ((m_Data.Shape == Shape.Line ||
                 m_Data.Shape == Shape.Cone2 ||
                 m_Data.Shape == Shape.Cone3 ||
                 m_Data.Shape == Shape.Cone5) && target == origin)
            {
                target -= caster.GetComponent<ActorComponent>().Model.transform.right * 0.01f;
            }

            List<BoardCell> cells;

            if (m_Data.Shape == Shape.Circle)
            {
                cells = m_BoardNavigator.WithinCircle(target, m_Data.Radius);
            }
            else if (m_Data.Shape == Shape.Cross)
            {
                cells = m_BoardNavigator.WithinCross(target, m_Data.Radius);
            }
            else
            {
                cells = m_BoardNavigator.WithinShape(origin, target, m_Data.Shape, m_Data.Radius);
            }

            if (m_Data.CheckLineOfSight)
            {
                cells = cells.OnLineOfSight(origin).ToList();
            }

            if (m_Data.ExcludeOrigin)
            {
                var excluded = m_BoardNavigator.WithinCircle(origin, 0);
                cells = cells.Where(cell => !excluded.Contains(cell)).ToList();
            }

            if (m_Data.ExcludeTarget)
            {
                var excluded = m_BoardNavigator.WithinCircle(target, 0);
                cells = cells.Where(cell => !excluded.Contains(cell)).ToList();
            }

            var entities = cells
                .SelectMany(cell => cell.GameObjectsInside)
                .Where(entity => !entity.IsDummy() && entity.IsAlive() &&
                                 Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, entity))
                .OrderBy(entity => (entity.transform.position - target).sqrMagnitude)
                .ToList();

            if (m_Data.Limit > 0)
            {
                entities = entities.Take(m_Data.Limit).ToList();
            }

            var effect = m_EffectRepository.FindOrFail(m_Data.EffectId);

            foreach (var entity in entities)
            {
                Inherit(effect.Clone()).Apply(caster, entity);
            }

            TriggerFinished();
        }
    }
}