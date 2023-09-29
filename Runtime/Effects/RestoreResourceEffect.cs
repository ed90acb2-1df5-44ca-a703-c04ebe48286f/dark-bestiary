using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RestoreResourceEffect : Effect
    {
        private readonly RestoreResourceEffectData m_Data;

        public RestoreResourceEffect(RestoreResourceEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new RestoreResourceEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<ResourcesComponent>().Restore(m_Data.ResourceType, m_Data.ResourceAmount);
            TriggerFinished();
        }
    }
}