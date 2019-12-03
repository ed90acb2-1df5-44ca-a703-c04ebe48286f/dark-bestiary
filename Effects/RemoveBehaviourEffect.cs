using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RemoveBehaviourEffect : Effect
    {
        private readonly RemoveBehaviourEffectData data;

        public RemoveBehaviourEffect(RemoveBehaviourEffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new RemoveBehaviourEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<BehavioursComponent>().RemoveAllStacks(this.data.BehaviourId);
            TriggerFinished();
        }
    }
}