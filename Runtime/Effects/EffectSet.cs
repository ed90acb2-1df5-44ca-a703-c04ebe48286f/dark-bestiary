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
        private readonly EffectSetData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        private Queue<Effect> m_Queue;
        private object m_Target;

        public EffectSet(EffectSetData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new EffectSet(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            m_Target = target;
            Apply();
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            m_Target = target;
            Apply();
        }

        private void Apply()
        {
            if (m_Data.Effects.Count == 0)
            {
                TriggerFinished();
                return;
            }

            m_Queue = new Queue<Effect>();

            foreach (var effect in GetEffects())
            {
                m_Queue.Enqueue(effect);
            }

            ApplyNextEffectFromQueue();
        }

        public IEnumerable<Effect> GetEffects()
        {
            return m_Data.Effects.Select(effectId => m_EffectRepository.FindOrFail(effectId)).ToList();
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;

            if (effect.IsFailed || m_Queue.Count == 0)
            {
                TriggerFinished();
                return;
            }

            ApplyNextEffectFromQueue();
        }

        private void ApplyNextEffectFromQueue()
        {
            var effect = Inherit(m_Queue.Dequeue().Clone());
            effect.Finished += OnEffectFinished;
            effect.Apply(Caster, m_Target);
        }
    }
}