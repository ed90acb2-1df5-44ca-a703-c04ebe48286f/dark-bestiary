using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class BuffBehaviour : Behaviour
    {
        private readonly Effect initialEffect;
        private readonly Effect periodicEffect;
        private readonly Effect onExpireEffect;
        private readonly Effect onRemoveEffect;

        public BuffBehaviour(BuffBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            var effectRepository = Container.Instance.Resolve<IEffectRepository>();

            this.initialEffect = effectRepository.Find(data.InitialEffectId);
            this.periodicEffect = effectRepository.Find(data.PeriodicEffectId);
            this.onExpireEffect = effectRepository.Find(data.OnExpireEffectId);
            this.onRemoveEffect = effectRepository.Find(data.OnRemoveEffectId);
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

        protected override void OnExpire(GameObject caster, GameObject target)
        {
            if (this.onExpireEffect == null)
            {
                return;
            }

            var effect = this.onExpireEffect.Clone();
            effect.Origin = target;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }

        protected override void OnRemoved(GameObject caster, GameObject target)
        {
            if (this.onRemoveEffect == null)
            {
                return;
            }

            var effect = this.onRemoveEffect.Clone();
            effect.Origin = target;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }
    }
}