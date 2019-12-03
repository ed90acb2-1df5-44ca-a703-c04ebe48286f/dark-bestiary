using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class StealBehaviourEffect : Effect
    {
        private readonly EmptyEffectData data;

        public StealBehaviourEffect(EmptyEffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new StealBehaviourEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var targetBehaviours = target.GetComponent<BehavioursComponent>();

            var stealed = targetBehaviours.Behaviours
                .FirstOrDefault(behaviour => behaviour.Flags.HasFlag(BehaviourFlags.Dispellable |
                                                                     BehaviourFlags.Magical |
                                                                     BehaviourFlags.Positive));

            if (stealed != null)
            {
                targetBehaviours.RemoveAllStacks(stealed.Id);
                caster.GetComponent<BehavioursComponent>().Apply(stealed, caster);
            }

            TriggerFinished();
        }
    }
}