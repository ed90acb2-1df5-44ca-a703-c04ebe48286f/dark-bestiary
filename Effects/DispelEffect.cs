using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class DispelEffect : Effect
    {
        private readonly DispelEffectData data;

        public DispelEffect(DispelEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new DispelEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var behaviours = target.GetComponent<BehavioursComponent>();

            foreach (var behaviour in behaviours.Behaviours.ToList())
            {
                if ((this.data.BehaviourFlags == BehaviourFlags.None || (behaviour.Flags & this.data.BehaviourFlags) == this.data.BehaviourFlags) &&
                    (this.data.BehaviourStatusFlags == StatusFlags.None || (behaviour.StatusFlags & this.data.BehaviourStatusFlags) > 0))
                {
                    behaviours.RemoveAllStacks(behaviour.Id);
                }
            }

            TriggerFinished();
        }
    }
}