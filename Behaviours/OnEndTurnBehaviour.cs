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
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnEndTurnBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
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

            var effect = this.effectRepository.Find(this.data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, entity);
        }
    }
}