using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class DispelEffect : Effect
    {
        private readonly DispelEffectData m_Data;

        public DispelEffect(DispelEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new DispelEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var behaviours = target.GetComponent<BehavioursComponent>();

            foreach (var behaviour in behaviours.Behaviours.ToList())
            {
                if ((m_Data.BehaviourFlags == BehaviourFlags.None || (behaviour.Flags & m_Data.BehaviourFlags) == m_Data.BehaviourFlags) &&
                    (m_Data.BehaviourStatusFlags == StatusFlags.None || (behaviour.StatusFlags & m_Data.BehaviourStatusFlags) > 0))
                {
                    behaviours.RemoveAllStacks(behaviour.Id);
                }
            }

            TriggerFinished();
        }
    }
}