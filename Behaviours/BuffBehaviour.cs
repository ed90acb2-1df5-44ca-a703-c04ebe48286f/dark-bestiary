using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Effects;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class BuffBehaviour : Behaviour
    {
        private readonly Effect initialEffect;
        private readonly Effect periodicEffect;

        public BuffBehaviour(BuffBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.initialEffect = EffectRepository.Find(data.InitialEffectId);
            this.periodicEffect = EffectRepository.Find(data.PeriodicEffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            if (this.initialEffect == null)
            {
                return;
            }

            var effect = this.initialEffect.Clone();
            effect.Origin = target;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }

        protected override void OnTick(GameObject caster, GameObject target)
        {
            if (this.periodicEffect == null)
            {
                return;
            }

            var effect = this.periodicEffect.Clone();
            effect.Origin = target;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }
    }
}