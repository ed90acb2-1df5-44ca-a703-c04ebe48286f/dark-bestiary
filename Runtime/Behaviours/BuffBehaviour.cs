using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Effects;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class BuffBehaviour : Behaviour
    {
        private readonly Effect m_InitialEffect;
        private readonly Effect m_PeriodicEffect;

        public BuffBehaviour(BuffBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_InitialEffect = s_EffectRepository.Find(data.InitialEffectId);
            m_PeriodicEffect = s_EffectRepository.Find(data.PeriodicEffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            if (m_InitialEffect == null)
            {
                return;
            }

            var effect = m_InitialEffect.Clone();
            effect.Origin = target;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }

        protected override void OnTick(GameObject caster, GameObject target)
        {
            if (m_PeriodicEffect == null)
            {
                return;
            }

            var effect = m_PeriodicEffect.Clone();
            effect.Origin = target;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }
    }
}