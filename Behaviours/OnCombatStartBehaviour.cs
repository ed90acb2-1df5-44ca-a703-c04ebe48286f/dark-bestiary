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
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnCombatStartBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
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

            var effect = this.effectRepository.Find(this.data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, Caster);
        }
    }
}