using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RepeatEffect : Effect
    {
        private readonly RepeatEffectData data;
        private readonly IEffectRepository effectRepository;

        private int counter;

        public RepeatEffect(RepeatEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new RepeatEffect(this.data, this.Validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            ApplyNext();
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            ApplyNext();
        }

        private void ApplyNext()
        {
            var effect = Inherit(this.effectRepository.Find(this.data.EffectId));
            effect.Finished += OnEffectFinished;
            effect.Apply(Caster, Target);
        }

        private void OnEffectFinished(Effect finished)
        {
            finished.Finished -= OnEffectFinished;

            this.counter++;

            if (this.counter >= this.data.Times)
            {
                TriggerFinished();
                return;
            }

            ApplyNext();
        }
    }
}