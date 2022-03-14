using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class EffectSet : Effect
    {
        private readonly EffectSetData data;
        private readonly IEffectRepository effectRepository;

        private Queue<Effect> queue;
        private object target;

        public EffectSet(EffectSetData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new EffectSet(this.data, this.Validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            this.target = target;
            Apply();
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            this.target = target;
            Apply();
        }

        private void Apply()
        {
            if (this.data.Effects.Count == 0)
            {
                TriggerFinished();
                return;
            }

            this.queue = new Queue<Effect>();

            foreach (var effect in GetEffects())
            {
                this.queue.Enqueue(effect);
            }

            ApplyNextEffectFromQueue();
        }

        public IEnumerable<Effect> GetEffects()
        {
            return this.data.Effects.Select(effectId => this.effectRepository.FindOrFail(effectId)).ToList();
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;

            if (effect.IsFailed || this.queue.Count == 0)
            {
                TriggerFinished();
                return;
            }

            ApplyNextEffectFromQueue();
        }

        private void ApplyNextEffectFromQueue()
        {
            var effect = Inherit(this.queue.Dequeue().Clone());
            effect.Finished += OnEffectFinished;
            effect.Apply(Caster, this.target);
        }
    }
}