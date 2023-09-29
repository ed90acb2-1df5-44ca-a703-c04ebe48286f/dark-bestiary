using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RemoveBehaviourEffect : Effect
    {
        private readonly RemoveBehaviourEffectData m_Data;

        public RemoveBehaviourEffect(RemoveBehaviourEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new RemoveBehaviourEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<BehavioursComponent>().RemoveAllStacks(m_Data.BehaviourId);
            TriggerFinished();
        }
    }
}