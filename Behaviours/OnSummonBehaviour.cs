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
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnSummonBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators, IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
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

            var effect = this.effectRepository.Find(this.data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, this.data.EventSubject == BehaviourEventSubject.Other ? summonedComponent.gameObject : Target);
        }
    }
}