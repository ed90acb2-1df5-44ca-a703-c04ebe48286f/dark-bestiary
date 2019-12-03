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
        private readonly Effect effect;
        private readonly SearchAreaEffectData data;
        private readonly List<Validator> validators;
        private readonly IEffectRepository effectRepository;

        public SearchPointsEffect(SearchAreaEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.validators = validators;
            this.effectRepository = effectRepository;
            this.effect = effectRepository.Find(data.EffectId);
        }

        protected override Effect New()
        {
            return new SearchPointsEffect(this.data, this.validators, this.effectRepository);
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

            switch (this.data.Shape)
            {
                case Shape.Circle:
                    cells = BoardNavigator.Instance.WithinCircle(target, this.data.Radius);
                    break;
                case Shape.Cross:
                    cells = BoardNavigator.Instance.WithinCross(target, this.data.Radius);
                    break;
                default:
                    cells = BoardNavigator.Instance.WithinShape(origin, target, this.data.Shape, this.data.Radius);
                    break;
            }

            cells = cells.Where(c => c.IsWalkable).ToList();

            if (this.data.CheckLineOfSight)
            {
                cells = cells.OnLineOfSight(origin).ToList();
            }

            if (this.data.ExcludeOrigin)
            {
                var excluded = BoardNavigator.Instance.WithinCircle(origin, 0);
                cells = cells.Where(c => !excluded.Contains(c)).ToList();
            }

            if (this.data.ExcludeTarget)
            {
                var excluded = BoardNavigator.Instance.WithinCircle(target, 0);
                cells = cells.Where(c => !excluded.Contains(c)).ToList();
            }

            if (this.data.Unoccupied)
            {
                cells = cells.Where(c => !c.IsOccupied).ToList();
            }

            cells = cells.OrderBy(c => (c.transform.position - target).sqrMagnitude).ToList();

            for (var index = 0; index < cells.Count; index++)
            {
                if (this.data.Limit != -1 && index >= this.data.Limit)
                {
                    break;
                }

                var clone = this.effect.Clone();
                clone.Skill = Skill;
                clone.Origin = Origin;
                clone.StackCount = StackCount;
                clone.Apply(caster, cells[index].transform.position);
            }

            TriggerFinished();
        }
    }
}