using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RandomEffect : Effect
    {
        private readonly RandomEffectData data;
        private readonly IEffectRepository effectRepository;

        public RandomEffect(RandomEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new RandomEffect(this.data, this.Validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            ApplyRandom(caster, target);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            ApplyRandom(caster, target);
        }

        private void ApplyRandom(GameObject caster, object target)
        {
            var effect = this.effectRepository.Find(this.data.Effects.Random());
            effect.Skill = Skill;
            effect.Origin = Origin;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);

            TriggerFinished();
        }
    }
}