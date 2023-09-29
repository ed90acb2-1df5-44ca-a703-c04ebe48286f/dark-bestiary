using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RemoveBehaviourStackEffect : Effect
    {
        private readonly RemoveBehaviourStackEffectData m_Data;

        public RemoveBehaviourStackEffect(RemoveBehaviourStackEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new RemoveBehaviourStackEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<BehavioursComponent>().RemoveStack(m_Data.BehaviourId, m_Data.StackCount);
            TriggerFinished();
        }
    }
}