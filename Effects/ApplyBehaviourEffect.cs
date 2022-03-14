using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ApplyBehaviourEffect : Effect
    {
        private readonly IBehaviourRepository behaviourRepository;
        private readonly ApplyBehaviourEffectData data;

        public ApplyBehaviourEffect(ApplyBehaviourEffectData data, List<ValidatorWithPurpose> validators,
            IBehaviourRepository behaviourRepository) : base(data, validators)
        {
            this.data = data;
            this.behaviourRepository = behaviourRepository;
        }

        protected override Effect New()
        {
            return new ApplyBehaviourEffect(this.data, this.Validators, this.behaviourRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var behaviour = this.behaviourRepository.FindOrFail(this.data.BehaviourId);
            behaviour.StackCount = Mathf.Max(this.data.Stacks, 1);
            behaviour.CanBeRemovedOnCast = Skill == null;

            target.GetComponent<BehavioursComponent>().ApplyAllStacks(behaviour, caster);

            TriggerFinished();
        }
    }
}