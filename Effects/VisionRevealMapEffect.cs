using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Visions;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class VisionRevealMapEffect : Effect
    {
        private readonly VisionRevealMapEffectData data;

        public VisionRevealMapEffect(VisionRevealMapEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new VisionRevealMapEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (VisionManager.Instance == null)
            {
                TriggerFinished();
                return;
            }

            VisionManager.Instance.RevealMap(this.data.Range);
            TriggerFinished();
        }
    }
}