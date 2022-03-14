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
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnBlockBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
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
            var effect = this.effectRepository.FindOrFail(this.data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, Target);
        }
    }
}