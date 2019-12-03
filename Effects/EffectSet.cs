using System.Collections.Generic;
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

        public EffectSet(EffectSetData data, List<Validator> validators,
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

            foreach (var effectId in this.data.Effects)
            {
                this.queue.Enqueue(this.effectRepository.FindOrFail(effectId));
            }

            ApplyNextEffectFromQueue();
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
            var clone = this.queue.Dequeue().Clone();
            clone.Finished += OnEffectFinished;
            clone.Origin = Origin;
            clone.Skill = Skill;
            clone.StackCount = StackCount;
            clone.Apply(Caster, this.target);
        }
    }
}