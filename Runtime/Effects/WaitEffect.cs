using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class WaitEffect : Effect
    {
        private readonly WaitEffectData m_Data;

        public WaitEffect(WaitEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new WaitEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (Mathf.Approximately(m_Data.Seconds, 0))
            {
                TriggerFinished();
                return;
            }

            Timer.Instance.Wait(m_Data.Seconds, TriggerFinished);
        }
    }
}