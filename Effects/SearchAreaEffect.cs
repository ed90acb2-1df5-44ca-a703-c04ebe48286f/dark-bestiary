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
        private readonly SearchAreaEffectData data;
        private readonly List<Validator> validators;
        private readonly BoardNavigator boardNavigator;
        private readonly IEffectRepository effectRepository;

        public SearchAreaEffect(SearchAreaEffectData data, List<Validator> validators,
            BoardNavigator boardNavigator, IEffectRepository effectRepository) : base(data, new List<Validator>())
        {
            this.data = data;
            this.validators = validators;
            this.boardNavigator = boardNavigator;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchAreaEffect(this.data, this.validators, this.boardNavigator, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var origin = OriginPosition();

            if ((this.data.Shape == Shape.Line ||
                 this.data.Shape == Shape.Cone2 ||
                 this.data.Shape == Shape.Cone3 ||
                 this.data.Shape == Shape.Cone5) && target == origin)
            {
                target -= caster.GetComponent<ActorComponent>().Model.transform.right * 0.01f;
            }

            List<BoardCell> cells;

            if (this.data.Shape == Shape.Circle)
            {
                cells = this.boardNavigator.WithinCircle(target, this.data.Radius);
            }
            else if (this.data.Shape == Shape.Cross)
            {
                cells = this.boardNavigator.WithinCross(target, this.data.Radius);
            }
            else
            {
                cells = this.boardNavigator.WithinShape(origin, target, this.data.Shape, this.data.Radius);
            }

            if (this.data.CheckLineOfSight)
            {
                cells = cells.OnLineOfSight(origin).ToList();
            }

            if (this.data.ExcludeOrigin)
            {
                var excluded = this.boardNavigator.WithinCircle(origin, 0);
                cells = cells.Where(cell => !excluded.Contains(cell)).ToList();
            }

            if (this.data.ExcludeTarget)
            {
                var excluded = this.boardNavigator.WithinCircle(target, 0);
                cells = cells.Where(cell => !excluded.Contains(cell)).ToList();
            }

            var entities = cells
                .SelectMany(cell => cell.GameObjectsInside)
                .Where(entity => !entity.IsDummy() && entity.IsAlive() &&
                                 this.validators.All(validator => validator.Validate(caster, entity)))
                .OrderBy(entity => (entity.transform.position - target).sqrMagnitude)
                .ToList();

            if (this.data.Limit > 0)
            {
                entities = entities.Take(this.data.Limit).ToList();
            }

            var effect = this.effectRepository.FindOrFail(this.data.EffectId);

            foreach (var entity in entities)
            {
                Inherit(effect.Clone()).Apply(caster, entity);
            }

            TriggerFinished();
        }
    }
}