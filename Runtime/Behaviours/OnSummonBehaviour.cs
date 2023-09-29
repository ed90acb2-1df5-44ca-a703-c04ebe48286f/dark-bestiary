using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnSummonBehaviour : Behaviour
    {
        private readonly EffectBehaviourData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public OnSummonBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators, IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            SummonedComponent.AnySummonedComponentInitialized += OnAnySummonedComponentInitialized;
        }

        protected override void OnRemove(GameObject source, GameObject target)
        {
            SummonedComponent.AnySummonedComponentInitialized -= OnAnySummonedComponentInitialized;
        }

        private void OnAnySummonedComponentInitialized(SummonedComponent summonedComponent)
        {
            if (summonedComponent.Master != Target || summonedComponent.gameObject.IsDummy())
            {
                return;
            }

            var effect = m_EffectRepository.Find(m_Data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, m_Data.EventSubject == BehaviourEventSubject.Other ? summonedComponent.gameObject : Target);
        }
    }
}