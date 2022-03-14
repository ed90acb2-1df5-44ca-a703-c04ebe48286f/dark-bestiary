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
        private readonly SearchRandomPointsData data;
        private readonly List<ValidatorWithPurpose> validators;
        private readonly IEffectRepository effectRepository;

        public SearchRandomPoints(SearchRandomPointsData data, List<ValidatorWithPurpose> validators, IEffectRepository effectRepository)
            : base(data, validators)
        {
            this.data = data;
            this.validators = validators;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchRandomPoints(this.data, this.validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var cells = BoardNavigator.Instance
                .WithinCircle(target, this.data.RangeMax)
                .Where(cell => cell.IsWalkable && (this.data.IncludeOccupied || !cell.IsOccupied))
                .Shuffle();

            if (this.data.RangeMin > 0)
            {
                var excluded = BoardNavigator.Instance.WithinCircle(target, this.data.RangeMin);
                cells = cells.Where(c => !excluded.Contains(c));
            }

            cells = cells.Take(this.data.Limit);

            foreach (var cell in cells)
            {
                var effect = this.effectRepository.Find(this.data.EffectId);
                effect.Skill = Skill;
                effect.DamageMultiplier = DamageMultiplier;
                effect.Apply(caster, cell.transform.position);
            }

            TriggerFinished();
        }
    }
}