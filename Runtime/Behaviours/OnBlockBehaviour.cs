using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnBlockBehaviour : Behaviour
    {
        private readonly EffectBehaviourData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public OnBlockBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            target.GetComponent<DefenseComponent>().AttackBlocked += OnAttackBlocked;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<DefenseComponent>().AttackBlocked -= OnAttackBlocked;
        }

        private void OnAttackBlocked(DefenseComponent defense)
        {
            var effect = m_EffectRepository.FindOrFail(m_Data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, Target);
        }
    }
}