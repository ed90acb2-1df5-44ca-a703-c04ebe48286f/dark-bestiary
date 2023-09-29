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
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly ApplyBehaviourEffectData m_Data;

        public ApplyBehaviourEffect(ApplyBehaviourEffectData data, List<ValidatorWithPurpose> validators,
            IBehaviourRepository behaviourRepository) : base(data, validators)
        {
            m_Data = data;
            m_BehaviourRepository = behaviourRepository;
        }

        protected override Effect New()
        {
            return new ApplyBehaviourEffect(m_Data, Validators, m_BehaviourRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var behaviour = m_BehaviourRepository.FindOrFail(m_Data.BehaviourId);
            behaviour.StackCount = Mathf.Max(m_Data.Stacks, 1);
            behaviour.CanBeRemovedOnCast = Skill == null;

            target.GetComponent<BehavioursComponent>().ApplyAllStacks(behaviour, caster);

            TriggerFinished();
        }
    }
}