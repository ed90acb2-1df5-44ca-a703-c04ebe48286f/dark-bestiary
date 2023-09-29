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
    public class SearchPointsEffect : Effect
    {
        private readonly Effect m_Effect;
        private readonly SearchAreaEffectData m_Data;
        private readonly List<ValidatorWithPurpose> m_Validators;
        private readonly IEffectRepository m_EffectRepository;

        public SearchPointsEffect(SearchAreaEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_Validators = validators;
            m_EffectRepository = effectRepository;
            m_Effect = effectRepository.Find(data.EffectId);
        }

        protected override Effect New()
        {
            return new SearchPointsEffect(m_Data, m_Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var origin = OriginPosition();

            // TODO: Separate shapes from lines/cones
            List<BoardCell> cells;

            switch (m_Data.Shape)
            {
                case Shape.Circle:
                    cells = BoardNavigator.Instance.WithinCircle(target, m_Data.Radius);
                    break;
                case Shape.Cross:
                    cells = BoardNavigator.Instance.WithinCross(target, m_Data.Radius);
                    break;
                default:
                    cells = BoardNavigator.Instance.WithinShape(origin, target, m_Data.Shape, m_Data.Radius);
                    break;
            }

            cells = cells.Where(c => c.IsWalkable).ToList();

            if (m_Data.CheckLineOfSight)
            {
                cells = cells.OnLineOfSight(origin).ToList();
            }

            if (m_Data.ExcludeOrigin)
            {
                var excluded = BoardNavigator.Instance.WithinCircle(origin, 0);
                cells = cells.Where(c => !excluded.Contains(c)).ToList();
            }

            if (m_Data.ExcludeTarget)
            {
                var excluded = BoardNavigator.Instance.WithinCircle(target, 0);
                cells = cells.Where(c => !excluded.Contains(c)).ToList();
            }

            if (m_Data.Unoccupied)
            {
                cells = cells.Where(c => !c.IsOccupied).ToList();
            }

            cells = cells.OrderBy(c => (c.transform.position - target).sqrMagnitude).ToList();

            for (var index = 0; index < cells.Count; index++)
            {
                if (m_Data.Limit != -1 && index >= m_Data.Limit)
                {
                    break;
                }

                var clone = m_Effect.Clone();
                clone.Skill = Skill;
                clone.DamageMultiplier = DamageMultiplier;
                clone.Origin = Origin;
                clone.StackCount = StackCount;
                clone.Apply(caster, cells[index].transform.position);
            }

            TriggerFinished();
        }
    }
}