using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class IfElseEffect : Effect
    {
        private readonly IfElseEffectData data;
        private readonly List<Validator> validators;
        private readonly IEffectRepository effectRepository;

        public IfElseEffect(IfElseEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, new List<Validator>())
        {
            this.data = data;
            this.validators = validators;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new IfElseEffect(this.data, this.validators, this.effectRepository);
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
            var effect = this.effectRepository.Find(this.validators.All(v => v.Validate(caster, target))
                ? this.data.IfEffectId
                : this.data.ElseEffectId);

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