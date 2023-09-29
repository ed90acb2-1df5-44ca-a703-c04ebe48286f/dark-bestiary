using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RunCooldownEffect : Effect
    {
        private readonly RunCooldownEffectData m_Data;

        public RunCooldownEffect(RunCooldownEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new RunCooldownEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<SpellbookComponent>().Get(m_Data.SkillId).RunCooldown();
            TriggerFinished();
        }
    }
}