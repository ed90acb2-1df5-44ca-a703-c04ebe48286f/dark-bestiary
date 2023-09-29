using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnEndTurnBehaviour : Behaviour
    {
        private readonly EffectBehaviourData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public OnEndTurnBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            CombatEncounter.AnyCombatTurnEnded += OnAnyCombatTurnEnded;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            CombatEncounter.AnyCombatTurnEnded -= OnAnyCombatTurnEnded;
        }

        private void OnAnyCombatTurnEnded(GameObject entity)
        {
            if (entity != Target || CombatEncounter.Active.Queue.Contains(entity))
            {
                return;
            }

            var effect = m_EffectRepository.Find(m_Data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, entity);
        }
    }
}