using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class SearchBehindEffect : Effect
    {
        private readonly SearchBehindEffectData data;
        private readonly IEffectRepository effectRepository;

        public SearchBehindEffect(SearchBehindEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchBehindEffect(this.data, this.Validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var cell = BoardNavigator.Instance.BehindTheBackOfOccupying(target);

            if (cell == null || !cell.IsOccupied)
            {
                TriggerFinished();
                return;
            }

            this.effectRepository.Find(this.data.EffectId)?.Apply(caster, cell.OccupiedBy);
            TriggerFinished();
        }
    }
}