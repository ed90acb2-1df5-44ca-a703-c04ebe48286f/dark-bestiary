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
    public class SearchPerimeterEffect : Effect
    {
        private readonly SearchPerimeterEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public SearchPerimeterEffect(SearchPerimeterEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchPerimeterEffect(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var effect = m_EffectRepository.FindOrFail(m_Data.EffectId);

            foreach (var cell in GetPerimeterCells())
            {
                effect.Clone().Apply(caster, cell.transform.position);
            }

            TriggerFinished();
        }

        private IEnumerable<BoardCell> GetPerimeterCells()
        {
            var side = m_Data.PickRandomSide
                ? m_Data.Sides.ToEnumerable().Random()
                : m_Data.Sides;

            var cells = new List<BoardCell>();

            if (side.HasFlag(Side.Top))
            {
                cells.AddRange(BoardNavigator.Instance.PerimeterTop().Walkable());
            }

            if (side.HasFlag(Side.Right))
            {
                cells.AddRange(BoardNavigator.Instance.PerimeterRight().Walkable());
            }

            if (side.HasFlag(Side.Bottom))
            {
                cells.AddRange(BoardNavigator.Instance.PerimeterBottom().Walkable());
            }

            if (side.HasFlag(Side.Left))
            {
                cells.AddRange(BoardNavigator.Instance.PerimeterLeft().Walkable());
            }

            return m_Data.Limit > 0 ? cells.Shuffle().Take(m_Data.Limit) : cells;
        }
    }
}