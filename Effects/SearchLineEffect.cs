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
        private readonly Effect effect;
        private readonly SearchLineEffectData data;
        private readonly List<Validator> validators;
        private readonly BoardNavigator boardNavigator;
        private readonly IEffectRepository effectRepository;

        public SearchLineEffect(SearchLineEffectData data, List<Validator> validators,
            BoardNavigator boardNavigator, IEffectRepository effectRepository) : base(data, new List<Validator>())
        {
            this.data = data;
            this.validators = validators;
            this.boardNavigator = boardNavigator;
            this.effectRepository = effectRepository;
            this.effect = effectRepository.Find(data.EffectId);
        }

        protected override Effect New()
        {
            return new SearchLineEffect(this.data, this.validators, this.boardNavigator, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (this.effect == null)
            {
                TriggerFinished();
                return;
            }

            var origin = OriginPosition();

            var cells = this.boardNavigator.WithinLine(origin, (target - origin).normalized, this.data.Length);

            if (this.data.CheckLineOfSight)
            {
                cells = cells.OnLineOfSight(origin).ToList();
            }

            var entities = cells.ToEntities()
                .Where(entity => entity.IsAlive() && this.validators.All(validator => validator.Validate(caster, entity)))
                .OrderBy(entity => (entity.transform.position - target).sqrMagnitude)
                .ToList();

            if (this.data.ExcludeOrigin)
            {
                var excluded = this.boardNavigator.EntitiesInRadius(origin, 0);
                entities = entities.Where(entity => !excluded.Contains(entity)).ToList();
            }

            foreach (var entity in entities)
            {
                Inherit(this.effect.Clone()).Apply(caster, entity);
            }

            TriggerFinished();
        }
    }
}