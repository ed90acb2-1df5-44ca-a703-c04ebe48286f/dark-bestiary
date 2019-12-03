using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class WaitEffect : Effect
    {
        private readonly WaitEffectData data;

        public WaitEffect(WaitEffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new WaitEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (Mathf.Approximately(this.data.Seconds, 0))
            {
                TriggerFinished();
                return;
            }

            Timer.Instance.Wait(this.data.Seconds, TriggerFinished);
        }
    }
}