using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RestoreResourceEffect : Effect
    {
        private readonly RestoreResourceEffectData data;

        public RestoreResourceEffect(RestoreResourceEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new RestoreResourceEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<ResourcesComponent>().Restore(this.data.ResourceType, this.data.ResourceAmount);
            TriggerFinished();
        }
    }
}