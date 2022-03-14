using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RandomWaitEffect : Effect
    {
        private readonly RandomWaitEffectData data;

        public RandomWaitEffect(RandomWaitEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new RandomWaitEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Timer.Instance.Wait(RNG.Range(this.data.Min, this.data.Max), TriggerFinished);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            Timer.Instance.Wait(RNG.Range(this.data.Min, this.data.Max), TriggerFinished);
        }
    }
}