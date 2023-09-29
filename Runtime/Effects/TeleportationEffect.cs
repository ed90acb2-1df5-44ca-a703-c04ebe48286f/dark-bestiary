using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class TeleportationEffect : Effect
    {
        private readonly EmptyEffectData m_Data;

        public TeleportationEffect(EmptyEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new TeleportationEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            TriggerFinished();
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            caster.transform.position = target.Snapped();
            TriggerFinished();
        }
    }
}