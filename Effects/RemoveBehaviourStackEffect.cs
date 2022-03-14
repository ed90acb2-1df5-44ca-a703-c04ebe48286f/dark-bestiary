using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RemoveBehaviourStackEffect : Effect
    {
        private readonly RemoveBehaviourStackEffectData data;

        public RemoveBehaviourStackEffect(RemoveBehaviourStackEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new RemoveBehaviourStackEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<BehavioursComponent>().RemoveStack(this.data.BehaviourId, this.data.StackCount);
            TriggerFinished();
        }
    }
}