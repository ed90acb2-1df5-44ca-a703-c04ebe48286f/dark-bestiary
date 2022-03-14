using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RunCooldownEffect : Effect
    {
        private readonly RunCooldownEffectData data;

        public RunCooldownEffect(RunCooldownEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new RunCooldownEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            target.GetComponent<SpellbookComponent>().FindOnActionBar(this.data.SkillId).RunCooldown();
            TriggerFinished();
        }
    }
}