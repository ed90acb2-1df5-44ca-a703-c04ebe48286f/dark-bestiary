using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ShowEffect : Effect
    {
        private readonly EffectData m_Data;

        public ShowEffect(EffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new ShowEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<ActorComponent>().Show();
            TriggerFinished();
        }
    }
}