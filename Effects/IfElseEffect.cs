using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class IfElseEffect : Effect
    {
        private readonly IfElseEffectData data;
        private readonly IEffectRepository effectRepository;

        public IfElseEffect(IfElseEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new IfElseEffect(this.data, this.Validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            ApplyIfElse(caster, target);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            ApplyIfElse(caster, target);
        }

        private void ApplyIfElse(GameObject caster, object target)
        {
            var effectId = this.Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, target)
                ? this.data.IfEffectId
                : this.data.ElseEffectId;

            var effect = this.effectRepository.Find(effectId);

            if (effect == null)
            {
                TriggerFinished();
                return;
            }

            Inherit(effect);

            effect.Finished += OnEffectFinished;
            effect.Apply(caster, target);
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;
            TriggerFinished();
        }
    }
}