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
    public class SearchDummiesEffect : Effect
    {
        private readonly SearchDummiesEffectData data;
        private readonly List<Validator> validators;
        private readonly IEffectRepository effectRepository;

        public SearchDummiesEffect(SearchDummiesEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, new List<Validator>())
        {
            this.data = data;
            this.validators = validators;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchDummiesEffect(this.data, this.validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var entities = BoardNavigator.Instance
                .WithinCircle(target, this.data.Range)
                .SelectMany(cell => cell.GameObjectsInside)
                .Where(entity => entity.IsDummy() && entity.IsAlive() && this.validators.All(v => v.Validate(caster, entity)))
                .OrderBy(entity => (entity.transform.position - caster.transform.position).sqrMagnitude)
                .ToList();

            if (this.data.Limit > 0)
            {
                entities = entities.Take(this.data.Limit).ToList();
            }

            foreach (var entity in entities)
            {
                this.effectRepository.Find(this.data.EffectId).Apply(caster, entity);
            }

            TriggerFinished();
        }
    }
}