using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ReduceCooldownsEffect : Effect
    {
        private readonly ReduceCooldownsEffectData m_Data;

        public ReduceCooldownsEffect(ReduceCooldownsEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new ReduceCooldownsEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            foreach (var slot in target.GetComponent<SpellbookComponent>().Slots)
            {
                slot.Skill.ReduceCooldown(m_Data.Amount);
            }

            TriggerFinished();
        }
    }
}