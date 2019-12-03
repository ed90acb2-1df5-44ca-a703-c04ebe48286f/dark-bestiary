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
        private readonly SearchPerimeterEffectData data;
        private readonly IEffectRepository effectRepository;

        public SearchPerimeterEffect(SearchPerimeterEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchPerimeterEffect(this.data, this.Validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var effect = this.effectRepository.FindOrFail(this.data.EffectId);

            foreach (var cell in GetPerimeterCells())
            {
                effect.Clone().Apply(caster, cell.transform.position);
            }

            TriggerFinished();
        }

        private IEnumerable<BoardCell> GetPerimeterCells()
        {
            var side = this.data.PickRandomSide
                ? this.data.Sides.ToEnumerable().Random()
                : this.data.Sides;

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

            return this.data.Limit > 0 ? cells.Shuffle().Take(this.data.Limit) : cells;
        }
    }
}