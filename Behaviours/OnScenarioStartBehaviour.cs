using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Scenarios;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnScenarioStartBehaviour : Behaviour
    {
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnScenarioStartBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Scenario.AnyScenarioStarted += OnAnyScenarioStarted;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Scenario.AnyScenarioStarted -= OnAnyScenarioStarted;
        }

        private void OnAnyScenarioStarted(Scenario scenario)
        {
            Timer.Instance.Wait(0.5f, () =>
            {
                var effect = this.effectRepository.Find(this.data.EffectId);
                effect.StackCount = StackCount;
                effect.Apply(Caster, Caster);
            });
        }
    }
}