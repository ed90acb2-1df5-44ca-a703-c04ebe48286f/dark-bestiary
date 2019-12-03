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
            List<Validator> validators) : base(data, validators)
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
            this.effectRepository.FindOrFail(this.data.EffectId).Apply(Caster, Target);
        }
    }
}