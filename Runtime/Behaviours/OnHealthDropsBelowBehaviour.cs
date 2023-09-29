using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Events;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnHealthDropsBelowBehaviour : Behaviour
    {
        private readonly OnHealthDropsBelowBehaviourData m_Data;
        private readonly IEffectRepository m_EffectRepository;
        private HealthComponent m_Health;

        public OnHealthDropsBelowBehaviour(OnHealthDropsBelowBehaviourData data,
            IEffectRepository effectRepository, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            m_Health = target.GetComponent<HealthComponent>();
            m_Health.Damaged += OnDamaged;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            m_Health.Damaged -= OnDamaged;
            m_Health = null;
        }

        private void OnDamaged(EntityDamagedEventData data)
        {
            if (m_Health.HealthFraction >= m_Data.Fraction)
            {
                return;
            }

            var effect = m_EffectRepository.FindOrFail(m_Data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, m_Data.EventSubject == BehaviourEventSubject.Me ? Target : data.Source);
        }
    }
}