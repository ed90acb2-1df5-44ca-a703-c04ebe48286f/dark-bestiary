using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnCombatStartBehaviour : Behaviour
    {
        private readonly EffectBehaviourData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public OnCombatStartBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Encounter.AnyEncounterStarted += OnAnyEncounterStarted;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Encounter.AnyEncounterStarted -= OnAnyEncounterStarted;
        }

        private void OnAnyEncounterStarted(Encounter encounter)
        {
            if (!(encounter is CombatEncounter))
            {
                return;
            }

            var effect = m_EffectRepository.Find(m_Data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, Caster);
        }
    }
}