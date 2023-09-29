using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RandomWaitEffect : Effect
    {
        private readonly RandomWaitEffectData m_Data;

        public RandomWaitEffect(RandomWaitEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new RandomWaitEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Timer.Instance.Wait(Rng.Range(m_Data.Min, m_Data.Max), TriggerFinished);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            Timer.Instance.Wait(Rng.Range(m_Data.Min, m_Data.Max), TriggerFinished);
        }
    }
}